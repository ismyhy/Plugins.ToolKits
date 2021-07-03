
using Plugins.ToolKits.Transmission.Protocol;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
namespace Plugins.ToolKits.Transmission.UDP
{
    internal class UDPChannel : Plugins.ToolKits.Transmission.UDPChannel
    {
        internal UDPChannel(IPEndPoint localPoint) : base(localPoint)
        {

        }
    }


    [DebuggerDisplay("{RemoteEndPoint}")]
    internal class UDPSession : ISession
    {
        private ContextContainer Context;
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

            var isCompress = setting?.IsCompressBuffer ?? false;
            if (isCompress)
            {
                var exist = Context.TryGet(TransmissionKeys.Compress, out Func<byte[], int, int, byte[]> func);
                if (exist)
                {
                    buffer = func(buffer, offset, length);
                    offset = 0;
                    length = buffer.Length;
                }
            }

            ProtocolPacket packet = ProtocolPacket.BuildPacket(buffer, offset, length, setting);
            
            var senderAction = Context.Get<Action<ProtocolPacket, IPEndPoint>>(TransmissionKeys.MessageSender);
            senderAction(packet, RemoteEndPoint);
            return packet.Wait();
        }

        public Task<int> SendAsync(byte[] buffer, int offset, int length, PacketSetting setting = null)
        {
            CancellationToken token = setting?.CancellationToken ?? CancellationToken.None;
            return Task.Factory.StartNew(() =>
            {
                return Send(buffer, offset, length, setting);
            }, token, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }
    }

}
