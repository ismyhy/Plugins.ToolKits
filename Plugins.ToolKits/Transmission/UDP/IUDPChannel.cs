using System;
using System.Net;
using System.Threading.Tasks;
using UdpClient = Plugins.ToolKits.Transmission.UdpClient;
namespace Plugins.ToolKits.Transmission
{


    public interface IUDPBuilder
    {
        IUDPChannel RunAsync();
    }


    public interface IUDPConfig : IDisposable
    {
        IUDPConfig UseReceiveCallback(Action<IUDPSession, byte[]> receiveCallback);
        IUDPConfig UseExclusiveAddress(bool exclusiveAddressUse);
        IUDPConfig UseEnableBroadcast(bool enableBroadcast);
        IUDPConfig UseTtl(short ttl);
        IUDPConfig UseDontFragment(bool dontFragment);
        IUDPConfig UseMulticastLoopback(bool multicastLoopback); 
        IUDPConfig UseJoinMulticastGroup(IPAddress iPAddress);
        IUDPConfig UseAsynchronousExecutionCallback(bool asynchronousExecutionCallback);
        IUDPConfig UseLocalIPEndPoint(IPAddress localIp, int localPort);
        IUDPConfig UseRemoteIPEndPoint(IPAddress remoteIp, int remotePort);

        IUDPBuilder Build();
    }

    public interface IUDPChannel :  IDisposable
    {  
        Task<int> SendAsync(byte[] buffer, int offset, int length, PacketSetting setting=null);

        int Send(byte[] buffer, int offset, int length, PacketSetting setting = null);

        void Close();
    }
}
