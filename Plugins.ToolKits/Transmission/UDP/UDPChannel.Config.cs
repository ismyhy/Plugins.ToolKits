using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UdpClient = Plugins.ToolKits.Transmission.UdpClient;
namespace Plugins.ToolKits.Transmission
{
    internal class UDPChannelKeys
    {
        public const string LocalIPEndPoint = "LocalIPEndPoint";
        public const string RemoteIPEndPoint = "RemoteIPEndPoint";
        public const string ReceiveFunc = "ReceiveFunc";
        public const string UdpClient = "UdpClient";
        public const string AsynchronousExecutionCallback = "AsynchronousExecutionCallback";
        public const string Semaphore= "Semaphore";
        public const string UDPChannel = "UDPChannel";
        public const string JoinMulticastGroup= "JoinMulticastGroup";
    }

    public partial class UDPChannel
    {
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
            if(!Context.TryGet<List<IPAddress>>(UDPChannelKeys.JoinMulticastGroup,out var list))
            {
                Context.Set(nameof(UdpClient.JoinMulticastGroup), list=new List<IPAddress>());
            } 
            list.Add(iPAddress);
            return this;
        }

        IUDPConfig IUDPConfig.UseReceiveCallback(Action<IUDPSession, byte[]> receiveCallback)
        {
            Context.Set(UDPChannelKeys.ReceiveFunc, receiveCallback);
            return this;
        }
        IUDPConfig IUDPConfig.UseLocalIPEndPoint(IPAddress localIp, int localPort)
        {
            Context.Set(UDPChannelKeys.LocalIPEndPoint, new IPEndPoint(localIp, localPort));
            return this;
        }
        IUDPConfig IUDPConfig.UseRemoteIPEndPoint(IPAddress remoteIp, int remotePort)
        {
            Context.Set(UDPChannelKeys.RemoteIPEndPoint, new IPEndPoint(remoteIp, remotePort));
            return this;
        }

        public IUDPConfig UseAsynchronousExecutionCallback(bool asynchronousExecutionCallback)
        {
            Context.Set(UDPChannelKeys.AsynchronousExecutionCallback, asynchronousExecutionCallback);
            return this;
        }

        IUDPBuilder IUDPConfig.Build()
        {
            if (!Context.TryGet<IPEndPoint>(UDPChannelKeys.LocalIPEndPoint, out IPEndPoint endPoint))
            {
                endPoint = new IPEndPoint(IPAddress.Any, TransmissionAssist.GetAvailablePort());
            }

            UdpClient udpClient = new UdpClient(endPoint);

            if (Context.TryGet<bool>(nameof(UdpClient.DontFragment), out bool dontFragment))
            {
                udpClient.DontFragment = dontFragment;
            }
            if (Context.TryGet<bool>(nameof(UdpClient.EnableBroadcast), out bool EnableBroadcast))
            {
                udpClient.EnableBroadcast = EnableBroadcast;
            }
            if (Context.TryGet<bool>(nameof(UdpClient.ExclusiveAddressUse), out bool ExclusiveAddressUse))
            {
                udpClient.ExclusiveAddressUse = ExclusiveAddressUse;
            }
            if (Context.TryGet<bool>(nameof(UdpClient.MulticastLoopback), out bool MulticastLoopback))
            {
                udpClient.MulticastLoopback = MulticastLoopback;
            }
            if (Context.TryGet<short>(nameof(UdpClient.Ttl), out short Ttl))
            {
                udpClient.Ttl = Ttl;
            }

            Context.Set(UDPChannelKeys.UdpClient, udpClient);

            return this;
        }
    }
}
