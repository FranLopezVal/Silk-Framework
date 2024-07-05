using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Security.Authentication;
using System.Net.Security;

using Silk.Core;
using Silk.http;
using Silk.Core.server;
using Silk.Log;
using System.Reflection;
using Silk.Database;
using System.Text.Json;

namespace Silk
{

    /// <summary>
    /// Class that represents a server that listens for incoming connections.
    /// <para/>
    /// Requires a SilkConfiguration object to be created.
    /// <see cref="SilkConfiguration"/>
    /// </summary>
    public class SilkServer
    {

        private readonly TcpListener? _server = null;
        private readonly RouterManager _routeHandler;
        private readonly X509Certificate2? _certificate;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        private readonly SilkConfiguration _silkConfiguration;
        //private IDatabaseConnection? _databaseConnection;

        private bool _isRunning = false;

        internal RouterManager RouterManager => _routeHandler;
        internal SilkConfiguration SilkConfiguration => _silkConfiguration;

        public bool IsSecure {  get; private set; }
        public bool IsRunning => _isRunning;


        /// <summary>
        /// Calls the default constructor and loads the configuration from the default file.
        /// File in the path: ".../Data/config.json"
        /// </summary>
        public SilkServer() : this(LoadData()) { }

        /// <summary>
        /// Entry point for the server.
        /// <param name="sConf">
        /// Configuration object for the server.
        /// </param>
        /// </summary>
        public SilkServer(SilkConfiguration sConf) : 
            this(new SilkEndpoint(sConf.Server.Ip ?? "localhost", sConf.Server.Port ?? 80), 
                sConf.Certificate.CertPath!, sConf.Certificate.KeyPath!) {
           
            _silkConfiguration = sConf;
            SilkLogServerData._config = sConf;
            SilkLogServerData._initTime = DateTime.Now;
            SilkLogServerData._serverName = "Silk";
            SilkLogServerData._serverVersion = Assembly.GetAssembly(typeof(SilkServer))?.GetName().Version?.ToString();
            SilkLogServerData._enableSilkMonitor = sConf.Loggin.EnableMonitor ?? true;

            if (SilkLogServerData._enableSilkMonitor)
            {
                SilkMonitor.Instance.Init(this);
            }

            Silk.ConsoleLogo();
        }

        private SilkServer(SilkEndpoint endpoint, string certificatePath, string certificatePassword)
        {
            _server = new TcpListener(IPAddress.Parse(endpoint.ip), endpoint.port);
            _routeHandler = new RouterManager();

            if (!string.IsNullOrEmpty(certificatePath) && !string.IsNullOrEmpty(certificatePassword))
            {
                if (!System.IO.File.Exists(certificatePath))
                {
                    Cls.Warning("Certificate file not found.");
                    IsSecure = false;
                } else
                {
                   _certificate = new X509Certificate2(certificatePath, certificatePassword);
                    IsSecure = _certificate.Verify();
                }
            } 

            LoadPlugins(_silkConfiguration?.Database.Name);
        }

        private void LoadPlugins(string? prefdb)
        {
            DatabasePlugin.LoadPlugins("plugins/db/");

            if (prefdb == null || prefdb == "") return;

            DatabasePlugin.GetDatabaseConnection(prefdb);
        }

        private static SilkConfiguration LoadData()
        {
            try
            {
                var url = URLS.ConfigFile;
                if (System.IO.File.Exists(url))
                {
                    string json = System.IO.File.ReadAllText(url);
                    SilkConfiguration conf = JsonSerializer.Deserialize<SilkConfiguration>(json);
                    return conf;
                }
            }
            catch (JsonException ex)
            {
                Cls.Error("Error loading configuration file: " + ex.Message);
                throw;
            }
            return Silk.DefaultConfiguration;
        }
       
        /// <summary>
        /// Initializes the server and starts listening for incoming connections.
        /// Use a Task asynchronusly to listen for incoming connections.
        /// <para/>
        /// See Task class <see cref="Task"/>
        /// </summary>
        public async Task StartAsync()
        {
            _server?.Start(); 
            _isRunning = true;
            Cls.Log("Server started.");
            while (!_cts.Token.IsCancellationRequested)
            {
                TcpClient client = await _server!.AcceptTcpClientAsync();
                if (_silkConfiguration.UniqueThread)
                    await Task.Run(async () => await HandleClientAsync(client));
                else
                    Task.Run(() => HandleClientAsync(client));
                
            }
        }

        /// <summary>
        /// Stops the server.
        /// I think it not require such a long description. :)
        /// </summary>
        public void Stop()
        {
            _cts.Cancel();
            _server?.Stop();
            _isRunning = false;
            Cls.Log("Server stopped.");
        }


        /// <summary>
        /// Handles the client connection.
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private async Task HandleClientAsync(TcpClient client)
        {
            using (client)
            {
                var remoteEndPoint = client.Client.RemoteEndPoint as IPEndPoint;
                IPAddress clientIpAddress = remoteEndPoint?.Address!;
                int clientPort = remoteEndPoint?.Port ?? 0;

                SilkConnection conn = new SilkConnection(client, clientIpAddress?.ToString()!, clientPort, null);

                var stream = client.GetStream();
                if (IsSecure)
                {
                    using (var sslStream = new SslStream(stream, false))
                    {
                        if (_certificate == null)
                        {
                            throw new InvalidOperationException("Certificate is not set.");
                        }
                        await sslStream.AuthenticateAsServerAsync(_certificate, clientCertificateRequired: false, enabledSslProtocols: SslProtocols.Tls12, checkCertificateRevocation: true);

                        byte[] buffer = new byte[1024];
                        int bytesRead = await sslStream.ReadAsync(buffer, 0, buffer.Length);

                        if (bytesRead > 0)
                        {
                            string strReq = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                            var request = Request.Parse(strReq, conn);

                            Response? res = _routeHandler.HandleRequest(request);

                            if (res == null)
                            {
                                // If it is null, it means that handleRequest has found a streaming route and has returned a null, because it handles the response on its own.
                                return;
                            }

                            if (string.IsNullOrEmpty(res.Content))
                            {
                                res.Content = Response.StatusCodeNotFound;
                            }

                            byte[] responseBytes = res.GetBytes();
                            if (!request.Url!.Contains("/api/monitor/"))
                                Cls.Log("Client: " + clientIpAddress + " [HTTPS] Request: " + request.Url);
                            await sslStream.WriteAsync(responseBytes, 0, responseBytes.Length);
                        }
                    }
                } 
                else
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                    if (bytesRead > 0)
                    {
                        string strReq = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                        var request = Request.Parse(strReq, conn);

                        Response res = _routeHandler.HandleRequest(request);

                        if (res == null)
                        {
                            // If it is null, it means that handleRequest has found a streaming route and has returned a null, because it handles the response on its own.
                            return;
                        }

                        if (string.IsNullOrEmpty(res.Content))
                        {
                            res.Content = Response.StatusCodeNotFound;
                        }

                        byte[] responseBytes = res.GetBytes();

                        // Do not log monitor requests
                        if (!request.Url!.Contains("/api/monitor/"))
                            Cls.Log("Client: " + clientIpAddress + " [HTTP] Request: " + request.Url);

                        await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
                    }
                }
            }
        }
    }
}
