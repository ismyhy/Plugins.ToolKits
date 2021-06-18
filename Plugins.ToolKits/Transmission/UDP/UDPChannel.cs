
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Plugins.ToolKits.Transmission
{
    [DebuggerDisplay("Local:{Context.Get<System.Net.IPEndPoint>(UDPChannelKeys.LocalIPEndPoint)}   Remote:{Context.Get<System.Net.IPEndPoint>(UDPChannelKeys.RemoteIPEndPoint)}")]
    public abstract partial class UDPChannel : IUDPBuilder, IUDPConfig, IUDPChannel
    {

        public static IUDPConfig Create()
        {
            return new Plugins.ToolKits.Transmission.UDP.UDPChannel();
        }

        public bool IsRunning { get; private set; }

        protected UDPChannel()
        {
            Context.Set(UDPChannelKeys.Semaphore, new Semaphore(1, 1));
            Context.Set(UDPChannelKeys.UDPChannel, this);
        }


        private readonly ContextContainer Context = new ContextContainer();

        void IUDPChannel.Close()
        {
            Dispose();
        }

        public void Dispose()
        {
            IsRunning = false;

            if (Context.TryGet<UdpClient>(UDPChannelKeys.UdpClient, out UdpClient udpClient))
            {
                udpClient?.Close();
                Context.RemoveKey(UDPChannelKeys.UdpClient);
                Context.RemoveKey(UDPChannelKeys.UDPChannel);
            }
            Context.ToObjectCollection().OfType<IDisposable>().ForEach(c => Invoker.RunIgnore<Exception>(c.Dispose));
            Context?.Dispose();
            SyncHandles?.Clear();
        }
        ~UDPChannel()
        {
            Dispose();
        }

        int IUDPChannel.Send(byte[] buffer, int offset, int length, PacketSetting setting = null)
        {
            if (!Context.TryGet(UDPChannelKeys.RemoteIPEndPoint, out IPEndPoint endPoint))
            {
                throw new ArgumentNullException(nameof(UDPChannelKeys.RemoteIPEndPoint));
            }

            ProtocolPacket packet = TransmissionAssist.BuildPacket(buffer, offset, length, setting);
            packet.RefreshCounter();
            return ClientSender(packet, endPoint, setting?.MillisecondsTimeout ?? -1);
        }



        Task<int> IUDPChannel.SendAsync(byte[] buffer, int offset, int length, PacketSetting setting = null)
        {
            CancellationToken token = setting?.CancellationToken ?? CancellationToken.None;
            return Task.Factory.StartNew(() =>
            {
                return ((IUDPChannel)this).Send(buffer, offset, length, setting);
            }, token, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }




        #region Run

        IUDPChannel IUDPBuilder.RunAsync()
        {
            if (!Context.TryGet<UdpClient>(UDPChannelKeys.UdpClient, out UdpClient udpClient))
            {
                throw new ArgumentNullException(nameof(UDPChannelKeys.UdpClient));
            }

            ConcurrentDictionary<int, IUDPSession> Sessions = new();

            Context.TryGet<bool>(UDPChannelKeys.AsynchronousExecutionCallback, out bool acceptAsync);
            Context.TryGet(UDPChannelKeys.ReceiveFunc, out Action<IUDPSession, byte[]> receiveFunc);
            Context.TryGet<List<IPAddress>>(UDPChannelKeys.JoinMulticastGroup, out List<IPAddress> list);

            list?.ForEach(x => udpClient.JoinMulticastGroup(x));

            IsRunning = true;

            udpClient.BeginReceive(ReceiveCallback, udpClient);

            return this;

            void ReceiveCallback(IAsyncResult iar)
            {
                if (iar.AsyncState is not UdpClient udpClient || !IsRunning)
                {
                    return;
                }

                if (iar.IsCompleted)
                {
                    IPEndPoint receivedEndPoint = null;

                    byte[] receiveBytes = udpClient.EndReceive(iar, ref receivedEndPoint);

                    ProtocolPacket protocol = ProtocolPacket.FromBuffer(receiveBytes, 0, receiveBytes.Length);

                    byte[] dataBuffer = protocol.Data;
                    if (protocol.ReportArrived)
                    {
                        Task.Factory.StartNew(() =>
                        {
                            byte[] ipAddress = receivedEndPoint?.Address?.GetAddressBytes() ?? new byte[] { 127, 0, 0, 1 }; ;
                            IPEndPoint remoteEndPoint = new IPEndPoint(new IPAddress(ipAddress), receivedEndPoint?.Port ?? 0);

                            protocol.Data = new byte[0];
                            protocol.Offset = 0;
                            protocol.DataLength = 0;
                            protocol.UsingRemoteEndPoint = true;
                            protocol.ReportArrived = false;
                            ClientSender(protocol, remoteEndPoint);
                        }, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
                    }

                    udpClient.BeginReceive(ReceiveCallback, udpClient);

                    if (SyncHandles.TryGetValue(protocol.Counter, out EventWaitHandle waitHandle))
                    {
                        waitHandle?.Set();
                        return;
                    }

                    int key = receivedEndPoint.Address.GetHashCode() ^ receivedEndPoint.Port;
                    IUDPSession session = Sessions.GetOrAdd(key, i => new UDPSession(Context)
                    {
                        RemoteEndPoint = receivedEndPoint
                    });

                    if (acceptAsync)
                    {
                        Task.Factory.StartNew(() =>
                        {
                            receiveFunc?.Invoke(session, dataBuffer);
                        }, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
                    }
                    else
                    {
                        receiveFunc?.Invoke(session, dataBuffer);
                    }
                    return;

                }

                Invoker.RunIgnore<Exception>(() => udpClient.BeginReceive(ReceiveCallback, udpClient));

            }
        }


        internal int ClientSender(ProtocolPacket packet, IPEndPoint remoteEndPoint, int millisecondsTimeout = -1)
        {

            if (!Context.TryGet<UdpClient>(UDPChannelKeys.UdpClient, out UdpClient udpClient))
            {
                throw new ArgumentNullException(nameof(UDPChannelKeys.UdpClient));
            }

            if (!Context.TryGet<Semaphore>(UDPChannelKeys.Semaphore, out Semaphore semaphore))
            {
                throw new ArgumentNullException(nameof(UDPChannelKeys.Semaphore));
            }

            EventWaitHandle awaiter = null;

            byte[] buffer = packet.ToBuffer();

            if (packet.ReportArrived)
            {
                SyncHandles[packet.Counter] = awaiter = new ManualResetEvent(false);
            }
            semaphore.WaitOne(-1);

            int sendCount;
            try
            {
                sendCount = udpClient.Send(buffer, buffer.Length, remoteEndPoint);
            }
            catch (Exception)
            {
                semaphore.Release(1);
                return 0;
            }
            if (!packet.ReportArrived)
            {
                semaphore.Release(1);
                return sendCount;
            }
            awaiter?.WaitOne(millisecondsTimeout);
            semaphore.Release(1);
            SyncHandles.TryRemove(packet.Counter);

            return sendCount;
        }



        private readonly IDictionary<long, EventWaitHandle> SyncHandles = new ConcurrentDictionary<long, EventWaitHandle>();



        #endregion

    }


    public class PacketSetting
    {
        public int MillisecondsTimeout { get; set; } = -1;

        public bool ReportArrived { get; set; }

        public bool Compress { get; set; }

        public CancellationToken CancellationToken { get; set; } = CancellationToken.None;
    }


    internal class UdpClient : System.Net.Sockets.UdpClient
    {
        public UdpClient(IPEndPoint localEP) : base(localEP)
        {
        }

        public bool IsActive => base.Active;
    }
}
