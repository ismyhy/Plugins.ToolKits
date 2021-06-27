using System;
using System.Net;
using System.Net.Sockets;

namespace Plugins.ToolKits.Transmission.TCP
{
    public class TCPHost
    {
        public TcpListener Host { get; private set; }

        public TCPHost(IPEndPoint localEndPoint)
        {
            if (localEndPoint == null)
            {
                throw new ArgumentNullException(nameof(localEndPoint));
            }
            Host = new TcpListener(localEndPoint);
        }
    }

}
