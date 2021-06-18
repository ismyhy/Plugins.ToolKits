using Plugins.ToolKits.ContextKit;

using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Plugins.ToolKits.Transmission
{
    public interface IUDPSession: IDisposable
    {
        Task<int> SendAsync(byte[] buffer, int offset, int length, PacketSetting setting = null);

        int Send(byte[] buffer, int offset, int length, PacketSetting setting = null);
    }


    [DebuggerDisplay("{RemoteEndPoint}")]
    internal class UDPSession : IUDPSession
    {
        ContextContainer Context;
        public UDPSession(ContextContainer context)
        {
            Context = context;
        }

        public IPEndPoint RemoteEndPoint { get; internal set; }

        public void Dispose()
        {
            Context = null;
        }

        public int Send(byte[] buffer, int offset, int length, PacketSetting setting = null)
        {
            if (!Context.TryGet<UDPChannel>(UDPChannelKeys.UDPChannel, out UDPChannel udpClient))
            {
                throw new ArgumentNullException(nameof(UDPChannelKeys.UdpClient));
            }
             
            var packet = TransmissionAssist.BuildPacket(buffer, offset, length, setting);
            packet.RefreshCounter();
            var sendCount = udpClient.ClientSender(packet, RemoteEndPoint, setting?.MillisecondsTimeout??-1);

            return sendCount;
        }

        public Task<int> SendAsync(byte[] buffer, int offset, int length, PacketSetting setting = null)
        {
            var token = setting?.CancellationToken ?? CancellationToken.None;
            return Task.Factory.StartNew(() =>
            {
                return  Send(buffer, offset, length, setting);
            }, token, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }
    }

}
