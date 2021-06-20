using System;
using System.Net;
namespace Plugins.ToolKits.Transmission
{
    public interface IUDPConfig : IDisposable
    {
        IUDPConfig UseReceiveCallback(Action<ISession, byte[]> receiveCallback);
        IUDPConfig UseExclusiveAddress(bool exclusiveAddressUse);
        IUDPConfig UseEnableBroadcast(bool enableBroadcast);
        IUDPConfig UseTtl(short ttl);
        IUDPConfig UseDontFragment(bool dontFragment);
        IUDPConfig UseMulticastLoopback(bool multicastLoopback);
        IUDPConfig UseJoinMulticastGroup(IPAddress iPAddress);
        IUDPConfig UseAsynchronousExecutionCallback(bool asynchronousExecutionCallback);
        IUDPConfig UseLocalIPEndPoint(IPAddress localIp, int localPort);
        IUDPConfig UseRemoteIPEndPoint(IPAddress remoteIp, int remotePort);
         
        IUDPConfig UseDecompressFunc(Func<byte[],int,int,byte[]> decompressFunc);
         
        IUDPConfig UseCompressFunc(Func<byte[], int, int, byte[]> compressFunc);
         
        IUDPChannel Build();
    }
}
