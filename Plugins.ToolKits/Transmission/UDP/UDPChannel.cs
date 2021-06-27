
using Plugins.ToolKits.Transmission.Protocol;
using Plugins.ToolKits.Transmission.UDP;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Plugins.ToolKits.Transmission
{
    [DebuggerDisplay("Local:{Context.Get<System.Net.IPEndPoint>(UDPChannelKeys.LocalIPEndPoint)}   Remote:{Context.Get<System.Net.IPEndPoint>(UDPChannelKeys.RemoteIPEndPoint)}")]
    public abstract partial class UDPChannel : IUDPChannel
    {
        public bool IsRunning { get; private set; }
        public readonly ContextContainer Context = new ContextContainer();
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(0, 1);
        private IPEndPoint remoteEndPoint;
        protected UDPChannel(IPEndPoint localEndPoint)
        {
            if (localEndPoint == null)
            {
                throw new ArgumentNullException(nameof(localEndPoint));
            }
            Client = new UdpClient(localEndPoint);

            Context.Set<Func<ProtocolPacket, IPEndPoint, SyncProtocol>>(TransmissionKeys.MessageSender, AddSenderQueue);
        }


        protected UDPChannel(IPEndPoint localEndPoint, IPEndPoint remoteEndPoint) : this(localEndPoint)
        {
            if (remoteEndPoint == null)
            {
                throw new ArgumentNullException(nameof(remoteEndPoint));
            }
            Context.Set(TransmissionKeys.RemoteIPEndPoint, remoteEndPoint);

        }

        public void Close()
        {
            Dispose();
        }

        public void Dispose()
        {
            IsRunning = false;

            if (Context.TryGet<UdpClient>(TransmissionKeys.UdpClient, out UdpClient udpClient))
            {
                udpClient?.Close();
                Context.RemoveKey(TransmissionKeys.UdpClient);
            }
            Context.ToObjectCollection().OfType<IDisposable>().ForEach(c => Invoker.RunIgnore<Exception>(c.Dispose));
            Context?.Dispose();
            SyncHandles?.Clear();
        }
        ~UDPChannel()
        {
            Dispose();
        }

        public int Send(byte[] buffer, int offset, int length, PacketSetting setting = null)
        {

            ProtocolPacket packet = ProtocolPacket.BuildPacket(buffer, offset, length, setting);
            //packet.RefreshCounter();

            var p = AddSenderQueue(packet, remoteEndPoint);

            var sendCount = p.Wait();
            return sendCount;//PacketSender(packet, remoteEndPoint, setting?.MillisecondsTimeout ?? -1);
        }

        public Task<int> SendAsync(byte[] buffer, int offset, int length, PacketSetting setting = null)
        {
            CancellationToken token = setting?.CancellationToken ?? CancellationToken.None;
            return Task.Factory.StartNew(() =>
            {
                return ((IUDPChannel)this).Send(buffer, offset, length, setting);
            }, token, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }

        #region Run

        public IUDPChannel RunAsync()
        {
            InnerSender();
            ConcurrentDictionary<int, ISession> Sessions = new();
            Context.TryGet<List<IPAddress>>(TransmissionKeys.JoinMulticastGroup, out List<IPAddress> list);
            Context.TryGet(TransmissionKeys.RemoteIPEndPoint, out remoteEndPoint);
            list?.ForEach(x => Client.JoinMulticastGroup(x));

            IsRunning = true;
            Client.BeginReceive(ReceiveCallback, Client);

            return this;

            void ReceiveCallback(IAsyncResult iar)
            {
                if (iar.AsyncState is not UdpClient udpClient || !IsRunning)
                {
                    return;
                }

                if (!iar.IsCompleted)
                {
                    udpClient.BeginReceive(ReceiveCallback, udpClient);
                    return;
                }
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
                        AddSenderQueue(protocol, remoteEndPoint);
                        //PacketSender(protocol, remoteEndPoint);
                    }, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
                }

                udpClient.BeginReceive(ReceiveCallback, udpClient);

                if (SyncHandles.TryGetValue(protocol.Counter, out var waitHandle))
                {
                    waitHandle?.Set();
                    return;
                }

                int key = receivedEndPoint.Address.GetHashCode() ^ receivedEndPoint.Port;

                Recived(Sessions.GetOrAdd(key, i => new UDPSession(Context)
                {
                    RemoteEndPoint = receivedEndPoint
                }), dataBuffer);
            }
        }

        protected virtual void Recived(ISession session, byte[] buffer)
        {
            Context.TryGet<bool>(TransmissionKeys.AsynchronousExecutionCallback, out bool acceptAsync);
            Context.TryGet(TransmissionKeys.ReceiveFunc, out Action<ISession, byte[]> receiveFunc);
            if (acceptAsync)
            {
                Task.Factory.StartNew(() =>
                {
                    receiveFunc?.Invoke(session, buffer);
                }, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
            }
            else
            {
                receiveFunc?.Invoke(session, buffer);
            }
        }

        protected virtual void Exception(Exception exception)
        { 
        }
        private SyncProtocol AddSenderQueue(ProtocolPacket packet, IPEndPoint remoteEndPoint)
        {
            SyncProtocol protocol = new SyncProtocol(packet, remoteEndPoint);
            SyncHandle2s.Enqueue(protocol);
            lock (this)
            {
                if (semaphore.CurrentCount != 1)
                {
                    semaphore.Release(1);
                }
            }
            return protocol;
        }

        private void InnerSender()
        {
            new Thread(() =>
            {
                while (true)
                {
                    if (!SyncHandle2s.TryDequeue(out var protocol))
                    { 
                        semaphore.Wait(-1);
                        continue;
                    }
                    try
                    {
                        if (protocol.ReportArrived)
                        {
                            SyncHandles[protocol.Counter] = protocol;
                        }
                        protocol.SendCount = Client.Send(protocol.Buffer, protocol.Buffer.Length, protocol.RemoteEndPoint);
                    }
                    catch (Exception e)
                    {
                        Exception(e);
                    }
                    finally
                    {
                        if (!protocol.ReportArrived)
                            protocol.Set();
                    }
                }
            })
            {
                IsBackground = true,
                Name = $"{DateTime.Now.Ticks}",
            }.Start();
        }


        private readonly ConcurrentQueue<SyncProtocol> SyncHandle2s = new ConcurrentQueue<SyncProtocol>();

        private readonly ConcurrentDictionary<long, SyncProtocol> SyncHandles = new ConcurrentDictionary<long, SyncProtocol>();

        #endregion

        #region

        public virtual byte[] DecompressBuffer(byte[] buffer, int offset, int length)
        {
            Func<byte[], int, int, byte[]> func = Context.Get<Func<byte[], int, int, byte[]>>(nameof(TransmissionKeys.Decompress));

            return func(buffer, offset, length);
        }

        public virtual byte[] CompressBuffer(byte[] buffer, int offset, int length)
        {
            Func<byte[], int, int, byte[]> func = Context.Get<Func<byte[], int, int, byte[]>>(nameof(TransmissionKeys.Compress));

            return func(buffer, offset, length);
        }

        #endregion


        #region  Base

        public bool MulticastLoopback
        {
            get => Client.MulticastLoopback;
            set => Client.MulticastLoopback = value;
        }

        public bool DontFragment
        {
            get => Client.DontFragment;
            set => Client.DontFragment = value;
        }
        public short Ttl
        {
            get => Client.Ttl;
            set => Client.Ttl = value;
        }
        public bool EnableBroadcast
        {
            get => Client.EnableBroadcast;
            set => Client.EnableBroadcast = value;
        }
        public bool ExclusiveAddressUse
        {
            get => Client.ExclusiveAddressUse;
            set => Client.ExclusiveAddressUse = value;
        }
        public UdpClient Client { get; private set; }

        public void Connect(IPEndPoint endPoint)
        {
            Client.Connect(endPoint);
        }

        public void JoinMulticastGroup(IPAddress multicastAddr, int timeToLive)
        {
            Client.JoinMulticastGroup(multicastAddr, timeToLive);
        }
        public void JoinMulticastGroup(IPAddress multicastAddr, IPAddress localAddress)
        {
            Client.JoinMulticastGroup(multicastAddr, localAddress);
        }
        public void JoinMulticastGroup(IPAddress multicastAddr)
        {
            Client.JoinMulticastGroup(multicastAddr);
        }
        public void JoinMulticastGroup(int ifindex, IPAddress multicastAddr)
        {
            Client.JoinMulticastGroup(ifindex, multicastAddr);
        }
        public void DropMulticastGroup(IPAddress multicastAddr, int ifindex)
        {
            Client.DropMulticastGroup(multicastAddr, ifindex);
        }
        public void DropMulticastGroup(IPAddress multicastAddr)
        {
            Client.DropMulticastGroup(multicastAddr);
        }
        public void AllowNatTraversal(bool allowed)
        {
            Client.AllowNatTraversal(allowed);
        }
        #endregion

        //internal int PacketSender(ProtocolPacket packet, IPEndPoint remoteEndPoint, int millisecondsTimeout = -1)
        //{
        //    EventWaitHandle awaiter = null;

        //    byte[] buffer = packet.ToBuffer();

        //    if (packet.ReportArrived)
        //    {
        //        SyncHandles[packet.Counter] = awaiter = new ManualResetEvent(false);
        //    }
        //    semaphore.Wait(-1);

        //    int sendCount;
        //    try
        //    {
        //        sendCount = Client.Send(buffer, buffer.Length, remoteEndPoint);
        //    }
        //    catch (Exception)
        //    {
        //        semaphore.Release(1);
        //        return 0;
        //    }
        //    if (!packet.ReportArrived)
        //    {
        //        semaphore.Release(1);
        //        return sendCount;
        //    }
        //    awaiter?.WaitOne(millisecondsTimeout);
        //    semaphore.Release(1);
        //    SyncHandles.TryRemoveAsync(packet.Counter);
        //    return sendCount;
        //}
    }

}
