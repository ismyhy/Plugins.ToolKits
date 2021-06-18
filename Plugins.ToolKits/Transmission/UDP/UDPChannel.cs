using Plugins.ToolKits.ContextKit;
using Plugins.ToolKits.Extensions;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Plugins.ToolKits.Transmission
{
    public abstract partial class UDPChannel : IUDPBuilder, IUDPConfig, IUDPChannel
    {
        public static IUDPConfig Create()
        {
            return new Plugins.ToolKits.Transmission.UDP.UDPChannel();
        }

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
            if (!Context.TryGet<UdpClient>(UDPChannelKeys.UdpClient, out UdpClient udpClient))
            {
                throw new ArgumentNullException(nameof(UDPChannelKeys.UdpClient));
            }

            Context.ToObjectCollection().OfType<IDisposable>().ForEach(i => Invoker.RunIgnore<Exception>(i.Dispose));

            Context.Clear();
            udpClient?.Close();
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
            Context.TryGet<bool>(UDPChannelKeys.JoinMulticastGroup, out bool joinMulticastGroup);

            Context.TryGet<bool>(UDPChannelKeys.AsynchronousExecutionCallback, out bool acceptAsync);

            Context.TryGet(UDPChannelKeys.ReceiveFunc, out Action<IUDPSession, byte[]> receiveFunc);

            udpClient.BeginReceive(ReceiveCallback, udpClient);

            if (joinMulticastGroup)
            {
                Context.TryGet<IPEndPoint>(UDPChannelKeys.RemoteIPEndPoint, out IPEndPoint remoteEndPoint);
                ProtocolPacket p = new ProtocolPacket
                {
                    JoinMulticastGroup = true
                };
                ClientSender(p, remoteEndPoint);
            }


            return this;

            void ReceiveCallback(IAsyncResult iar)
            {
                if (!(iar.AsyncState is UdpClient udpClient))
                {
                    return;
                }

                if (iar.IsCompleted)
                {
                    IPEndPoint receivedRemoteEndPoint = null;
                    byte[] receiveBytes = udpClient.EndReceive(iar, ref receivedRemoteEndPoint);

                    ProtocolPacket protocol = ProtocolPacket.FromBuffer(receiveBytes, 0, receiveBytes.Length);


                    if (protocol.ReportArrived || protocol.JoinMulticastGroup)
                    {
                        Task.Factory.StartNew(() =>
                        {
                            byte[] ipAddress = receivedRemoteEndPoint?.Address?.GetAddressBytes() ?? new byte[] { 127, 0, 0, 1 }; ;
                            IPEndPoint remoteEndPoint = new IPEndPoint(new IPAddress(ipAddress), receivedRemoteEndPoint?.Port ?? 0);
                            ProtocolPacket p = protocol.CopyTo(new ProtocolPacket());
                            p.JoinMulticastGroup = false;
                            p.Data = new byte[0];
                            p.Offset = 0;
                            p.DataLength = 0;
                            p.UsingRemoteEndPoint = true;
                            p.ReportArrived = false;
                            ClientSender(p, remoteEndPoint);
                        }, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
                    }
                    udpClient.BeginReceive(ReceiveCallback, udpClient);

                    if (protocol.JoinMulticastGroup)
                    {
                        if (!Context.TryGet(UDPChannelKeys.UdpClient, out UdpClient udpClient2))
                        {
                            throw new ArgumentNullException(nameof(UDPChannelKeys.UdpClient));
                        }
                        udpClient2.JoinMulticastGroup(receivedRemoteEndPoint.Address);
                        return;
                    }


                    if (SyncHandles.TryGetValue(protocol.Counter, out EventWaitHandle waitHandle))
                    {
                        waitHandle?.Set();
                        return;
                    }

                    UDPSession session = new UDPSession(Context)
                    {
                        RemoteEndPoint = receivedRemoteEndPoint
                    };
                    if (acceptAsync)
                    {
                        Task.Factory.StartNew(() =>
                        {
                            receiveFunc?.Invoke(session, protocol.Data);
                        }, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);

                    }
                    else
                    {
                        receiveFunc?.Invoke(session, protocol.Data);
                    }
                    return;

                }

                udpClient.BeginReceive(ReceiveCallback, udpClient);
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

            //IPEndPoint remotePoint = remoteEndPoint ?? new IPEndPoint(new IPAddress(packet.Ip), packet.Port);

            if (packet.ReportArrived || packet.JoinMulticastGroup)
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
            if (!packet.ReportArrived && !packet.JoinMulticastGroup)
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

}
