namespace Silk.Core.server
{
    /// <summary>
    /// Data about the server to connect to.
    /// </summary>
    public class SilkConfiguration
    {
        public ServerConfig Server { get; set; } = new ServerConfig();
        public DatabaseConfig Database { get; set; } = new DatabaseConfig();
        public LogginConfig Loggin { get; set; } = new LogginConfig();
        public Certificate Certificate { get; set; } = new Certificate();

        public bool UniqueThread { get; set; } = false;
        public string Locale { get; set; } = "en-US";
    }


    public class ServerConfig
    {
        public string? Ip { get; set; }
        public int? Port { get; set; }
    }

    public class DatabaseConfig
    {
        public string? Host { get; set; }
        public int? Port { get; set; }
        public string? Name { get; set; } = null;
        public string? Connection { get; set; } = null;
    }

    public class LogginConfig
    {
        public bool? EnableMonitor { get; set; } = true;
        public bool? EnableConsole { get; set; } = true;
    }

    public class  Certificate
    {
        public string? KeyPath { get; set; } = null;
        public string? CertPath { get; set; } = null;
    }

}



/*
 *  OLD CONFIGURATION SYS
        /// <summary>
        /// Ip to listen to.
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// Port to listen to.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// The path to the certificate Ssl, if it has one.
        /// is not required if you dont need https.
        /// </summary>
        public string? CertificatePath { get; set; }

        /// <summary>
        /// The password for the certificate Ssl, if it has one.
        /// is not required if you dont need https.
        /// </summary>
        public string? CertificatePassword { get; set; }


        /// <summary>
        /// If true, the server will handle each request in a unique thread.
        /// </summary>
        public bool HandleUniqueThread { get; set; }

        /// <summary>
        /// Set a default target database for the server.
        /// </summary>
        public string DefaultDatabase { get; set; }

        /// <summary>
        /// Enable server monitor.
        /// You can access the monitor at /silk/monitor
        /// </summary>
        public bool EnableMonitor { get; set; }*/