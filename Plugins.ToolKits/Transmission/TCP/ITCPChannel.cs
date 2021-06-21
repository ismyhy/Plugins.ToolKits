using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Plugins.ToolKits.Transmission.TCP
{
    public interface ITCPChannel
    {
        ITCPChannel RunAsync();

        Task<int> SendAsync(byte[] buffer, int offset, int length, PacketSetting setting = null);

        int Send(byte[] buffer, int offset, int length, PacketSetting setting = null);

        void Close();
    }

    public class TCPChannel
    {
        public readonly ContextContainer Context = new ContextContainer();
        public TcpClient TcpClient { get; private set; }
        public TCPChannel(IPEndPoint localEndPoint, IPEndPoint remoteEndPoint)
        {
            if (localEndPoint == null)
            {
                throw new ArgumentNullException(nameof(localEndPoint));
            }

            if (remoteEndPoint == null)
            {
                throw new ArgumentNullException(nameof(remoteEndPoint));
            }
            Context.Set(TransmissionKeys.RemoteIPEndPoint, remoteEndPoint);
            TcpClient = new TcpClient(localEndPoint);
        }


        #region Base

        public int SendTimeout
        {
            get => TcpClient.SendTimeout;
            set => TcpClient.SendTimeout = value;
        }

        public int ReceiveTimeout
        {
            get => TcpClient.ReceiveTimeout;
            set => TcpClient.ReceiveTimeout = value;
        }
        public int SendBufferSize
        {
            get => TcpClient.SendBufferSize;
            set => TcpClient.SendBufferSize = value;
        }

        public int ReceiveBufferSize
        {
            get => TcpClient.ReceiveBufferSize;
            set => TcpClient.ReceiveBufferSize = value;
        }

        public bool ExclusiveAddressUse
        {
            get => TcpClient.ExclusiveAddressUse;
            set => TcpClient.ExclusiveAddressUse = value;
        }

        public bool Connected => TcpClient.Connected;
        public LingerOption LingerState
        {
            get => TcpClient.LingerState;
            set => TcpClient.LingerState = value;
        }

        public bool NoDelay
        {
            get => TcpClient.NoDelay;
            set => TcpClient.NoDelay = value;
        }
         
        public void Connect()
        { 
            IPEndPoint remoteEndPoint = Context.Get<IPEndPoint>(TransmissionKeys.RemoteIPEndPoint);
            TcpClient.Connect(remoteEndPoint);
        }
        #endregion
    }



}
