using Plugins.ToolKits.Transmission.Protocol;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Plugins.ToolKits.Transmission.UDP
{
    internal class UDPChannelKeys
    {
        public const string LocalIPEndPoint = "LocalIPEndPoint";
        public const string RemoteIPEndPoint = "RemoteIPEndPoint";
        public const string ReceiveFunc = "ReceiveFunc";
        public const string UdpClient = "UdpClient";
        public const string AsynchronousExecutionCallback = "AsynchronousExecutionCallback";
        public const string Semaphore = "Semaphore";
        public const string UDPChannel = "UDPChannel";
        public const string JoinMulticastGroup = "JoinMulticastGroup";
        public const string MessageSender= "MessageSender";
        public const string Decompress= "Decompress";
        public const string Compress = "Compress";
    }
    public class UDPConfig : IUDPConfig
    {
        private readonly ContextContainer Context=new ContextContainer();   
        public UDPConfig()
        {
            Context.Set<Func<byte[], int, int, byte[]>>(nameof(UDPChannelKeys.Decompress), ProtocolPacket.Decompress);
            Context.Set<Func<byte[], int, int, byte[]>>(nameof(UDPChannelKeys.Compress), ProtocolPacket.Compress);
        }

        IUDPConfig IUDPConfig.UseDontFragment(bool dontFragment)
        {
            Context.Set(nameof(UdpClient.DontFragment), dontFragment);
            return this;
        }

        IUDPConfig IUDPConfig.UseEnableBroadcast(bool enableBroadcast)
        {
            Context.Set(nameof(UdpClient.EnableBroadcast), enableBroadcast);
            return this;
        }

        IUDPConfig IUDPConfig.UseExclusiveAddress(bool exclusiveAddressUse)
        {
            Context.Set(nameof(UdpClient.ExclusiveAddressUse), exclusiveAddressUse);
            return this;
        }

        IUDPConfig IUDPConfig.UseMulticastLoopback(bool multicastLoopback)
        {
            Context.Set(nameof(UdpClient.MulticastLoopback), multicastLoopback);
            return this;
        }
        IUDPConfig IUDPConfig.UseTtl(short ttl)
        {
            Context.Set(nameof(UdpClient.Ttl), ttl);
            return this;
        }

        public IUDPConfig UseJoinMulticastGroup(IPAddress iPAddress)
        {
            if(iPAddress == null)
            {
                throw new ArgumentNullException(nameof(iPAddress)); 
            }

            if (!Context.TryGet<List<IPAddress>>(UDPChannelKeys.JoinMulticastGroup, out List<IPAddress> list))
            {
                Context.Set(nameof(UdpClient.JoinMulticastGroup), list = new List<IPAddress>());
            }
            list.Add(iPAddress);
            return this;
        }

        IUDPConfig IUDPConfig.UseReceiveCallback(Action<ISession, byte[]> receiveCallback)
        {
            if(receiveCallback == null)
            {
                throw new ArgumentNullException(nameof(receiveCallback));
            }

            Context.Set(UDPChannelKeys.ReceiveFunc, receiveCallback);
            return this;
        }
        IUDPConfig IUDPConfig.UseLocalIPEndPoint(IPAddress localIp, int localPort)
        {
            if (localIp == null)
            {
                throw new ArgumentNullException(nameof(localIp));
            }

            if(localPort <= 0)
            {
                throw new ArgumentException(nameof(localPort));
            }

            Context.Set(UDPChannelKeys.LocalIPEndPoint, new IPEndPoint(localIp, localPort));
            return this;
        }
        IUDPConfig IUDPConfig.UseRemoteIPEndPoint(IPAddress remoteIp, int remotePort)
        {
            if (remoteIp == null)
            {
                throw new ArgumentNullException(nameof(remoteIp));
            }

            if (remotePort <= 0)
            {
                throw new ArgumentException(nameof(remotePort));
            }

            Context.Set(UDPChannelKeys.RemoteIPEndPoint, new IPEndPoint(remoteIp, remotePort));
            return this;
        }

        public IUDPConfig UseAsynchronousExecutionCallback(bool asynchronousExecutionCallback)
        {
            Context.Set(UDPChannelKeys.AsynchronousExecutionCallback, asynchronousExecutionCallback);
            return this;
        }

        IUDPChannel IUDPConfig.Build()
        {
            if (!Context.TryGet<IPEndPoint>(UDPChannelKeys.LocalIPEndPoint, out IPEndPoint endPoint))
            {
                endPoint = new IPEndPoint(IPAddress.Any, TransmissionAssist.GetAvailablePort());
            }

            UDPChannel udpChannel = new Plugins.ToolKits.Transmission.UDP.UDPChannel(endPoint);

            var type = typeof(UdpClient);
            foreach (var item in Context.AllKey)
            { 
                type.GetProperty(item)?.SetValue(udpChannel, Context.Get<object>(item));
            } 
            Context.CopyTo(udpChannel.Context);

            udpChannel.Context.Set(UDPChannelKeys.UDPChannel, udpChannel); 
            return udpChannel;
        }

        public void Dispose()
        {
             
        }

        public IUDPConfig UseDecompressFunc(Func<byte[], int, int, byte[]> decompressFunc)
        {
            if (decompressFunc == null)
            {
                throw new ArgumentNullException(nameof(decompressFunc));
            }
            Context.Set<Func<byte[], int, int, byte[]>>(nameof(UDPChannelKeys.Decompress), decompressFunc);
            return this;
        }

        public IUDPConfig UseCompressFunc(Func<byte[], int, int, byte[]> compressFunc)
        {
            if (compressFunc == null)
            {
                throw new ArgumentNullException(nameof(compressFunc));
            } 
            Context.Set<Func<byte[], int, int, byte[]>>(nameof(UDPChannelKeys.Compress), compressFunc);
            return this;
        }
    }
}
