using System.Text;

namespace Silk.Core
{
    /// <summary>
    /// Data about the endpoint to connect to.
    /// not is a real endpoint, just a data structure for Silk server.
    /// </summary>
    public class SilkEndpoint
    {
        private string _ip;
        private byte[] _addr;
        private int _port;

        public SilkEndpoint(string ip, int port)
        {
            _ip = ip;
            _addr = SilkEndpoint.convertToAddr(ip);
            _port = port;
        }

        private static byte[] convertToAddr (string ip)
        {
           return Encoding.ASCII.GetBytes(ip);
        }

        public string ip
        {
            get { return _ip; }
        }

        public byte[] addr
        {
            get { return _addr; }
        }

        public int port
        {
            get { return _port; }
        }
    }
}
