using Plugins.ToolKits.Transmission.Protocol;
using System;
using System.Net;
using System.Net.Sockets;

namespace Plugins.ToolKits.Transmission.TCP
{
    public interface ITCPChannelConfig
    {
        ITCPChannelConfig UseSendTimeout(int SendTimeout);
        ITCPChannelConfig UseReceiveTimeout(int ReceiveTimeout);
        ITCPChannelConfig UseSendBufferSize(int SendBufferSize); 
        ITCPChannelConfig UseReceiveBufferSize(int ReceiveBufferSize);
        ITCPChannelConfig UseExclusiveAddressUse(int ExclusiveAddressUse);
        ITCPChannelConfig UseLingerState([NotNull]LingerOption lingerState);
        ITCPChannelConfig UseNoDelay(bool NoDelay);
        ITCPChannelConfig UseRemoteIPEndPoint(IPAddress remoteIp, int remotePort);
        ITCPChannelConfig UseLocalIPEndPoint(IPAddress localIp, int localPort); 
        ITCPChannelConfig UseDecompressFunc(Func<byte[], int, int, byte[]> decompressFunc);
        ITCPChannelConfig UseCompressFunc(Func<byte[], int, int, byte[]> compressFunc);
        ITCPChannel Build();
    }

    internal static class TCPConfigKeys
    {
        public const string LocalIPEndPoint = "LocalIPEndPoint";
        public const string RemoteIPEndPoint = "RemoteIPEndPoint";
        public const string ReceiveFunc = "ReceiveFunc"; 
        public const string AsynchronousExecutionCallback = "AsynchronousExecutionCallback";
        public const string Semaphore = "Semaphore"; 
        public const string JoinMulticastGroup = "JoinMulticastGroup";
        public const string MessageSender = "MessageSender";
        public const string Decompress = "Decompress";
        public const string Compress = "Compress";   
        public const string TcpClient = "TcpClient";
    }

    internal class TCPChannelConfig : ITCPChannelConfig
    {
        private readonly ContextContainer Context=new ContextContainer();


        public TCPChannelConfig()
        {
            Context.Set<Func<byte[], int, int, byte[]>>(nameof(TCPConfigKeys.Decompress), ProtocolPacket.Decompress);
            Context.Set<Func<byte[], int, int, byte[]>>(nameof(TCPConfigKeys.Compress), ProtocolPacket.Compress);

        }


        public ITCPChannel Build()
        {
            if (!Context.TryGet<IPEndPoint>(TCPConfigKeys.LocalIPEndPoint,out var localIpPoint))
            {
                throw new Exception("The correct local port is not configured");
            }             
            if (!Context.TryGet<IPEndPoint>(TCPConfigKeys.RemoteIPEndPoint, out var a2))
            {
                throw new Exception("The correct remote port is not configured");
            }
             
            Context.TryGet<IPEndPoint>(TCPConfigKeys.RemoteIPEndPoint, out var remoteIpPoint);

            return null;

            //var tcpChannel = new TCPChannel();
            //Context.CopyTo(tcpChannel.Context);

            //TcpClient tcpClient = new TcpClient(localIpPoint);
            //var type = typeof(TcpClient);

            //foreach (var item in Context.AllKey)
            //{ 
            //    type.GetProperty(item)?.SetValue(tcpClient, Context.Get<object>(item));
            //}

            //tcpChannel.Context.Set(TCPConfigKeys.TcpClient, tcpClient);
           
            //return tcpChannel;
        }

        public ITCPChannelConfig UseCompressFunc(Func<byte[], int, int, byte[]> compressFunc)
        {
            if (compressFunc is null)
            {
                throw new ArgumentNullException(nameof(compressFunc));
            }
            Context.Set(nameof(TCPConfigKeys.Compress), compressFunc);
            return this;
        }

        public ITCPChannelConfig UseDecompressFunc(Func<byte[], int, int, byte[]> decompressFunc)
        {
            if(decompressFunc  is null)
            {
                throw new ArgumentNullException(nameof(decompressFunc));
            }
            Context.Set(nameof(TCPConfigKeys.Decompress), decompressFunc);
            return this;

        }
        public ITCPChannelConfig UseRemoteIPEndPoint(IPAddress remoteIp, int remotePort)
        {
            if (remoteIp == null)
            {
                throw new ArgumentNullException(nameof(remoteIp));
            }

            if (remotePort <= 0)
            {
                throw new ArgumentException(nameof(remotePort));
            }

            var ipEndPoint = new IPEndPoint(remoteIp, remotePort);

            Context.Set(nameof(TCPConfigKeys.RemoteIPEndPoint), ipEndPoint);
            return this;
        }
        public ITCPChannelConfig UseLocalIPEndPoint(IPAddress localIp, int localPort)
        {
            if(localIp == null)
            {
                throw new ArgumentNullException(nameof(localIp));
            } 

            if (localPort <= 0)
            {
                throw new ArgumentException(nameof(localPort));
            }

            var ipEndPoint=new IPEndPoint(localIp, localPort);

            Context.Set(nameof(TCPConfigKeys.LocalIPEndPoint), ipEndPoint);
            return this;
        }


        public ITCPChannelConfig UseExclusiveAddressUse(int exclusiveAddressUse)
        {
            Context.Set(nameof(TcpClient.ExclusiveAddressUse), exclusiveAddressUse);
            return this;
        }

        public ITCPChannelConfig UseLingerState([NotNull] LingerOption lingerState)
        {
            Context.Set(nameof(TcpClient.LingerState), lingerState);
            return this;
        }

  
        public ITCPChannelConfig UseNoDelay(bool noDelay)
        {
            Context.Set(nameof(TcpClient.NoDelay), noDelay);
            return this;
        }

        public ITCPChannelConfig UseReceiveBufferSize(int receiveBufferSize)
        {
            Context.Set(nameof(TcpClient.ReceiveBufferSize), receiveBufferSize);
            return this;
        }

        public ITCPChannelConfig UseReceiveTimeout(int receiveTimeout)
        {
            Context.Set(nameof(TcpClient.ReceiveTimeout), receiveTimeout);
            return this;
        }

        public ITCPChannelConfig UseSendBufferSize(int sendBufferSize)
        {
            Context.Set(nameof(TcpClient.SendBufferSize), sendBufferSize);
            return this;
        }

        public ITCPChannelConfig UseSendTimeout(int sendTimeout)
        {
            Context.Set(nameof(TcpClient.SendTimeout),sendTimeout);
            return this;
        }

    
    }

}
