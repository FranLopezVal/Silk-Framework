using Silk.Log;
using System.Net.Sockets;

namespace Silk.Core
{
    /// <summary>
    /// Contains data about the endpoint to connect to.
    /// too contains the certificate if it is a secure connection.
    /// </summary>
    public class SilkConnection
    {
        private SilkEndpoint _ep;
        private Certificate? _cert;
        private TcpClient? _client;

        public SilkEndpoint Endpoint
        {
            get { return _ep; }
        }

        public Certificate? Certificate
        {
            get { return _cert; }
        }

        public TcpClient? Client
        {
            get { return _client; }
        }

        public SilkConnection(TcpClient client, string ip, int port, Certificate? cert)
        {
            _ep = new SilkEndpoint(ip,port);
            _cert = cert;
            _client = client;
        }
    }

    public class Certificate
    {
        private string _cert;
        internal Certificate(string _cert)
        {
            this._cert = _cert;
        }        
    }
}
