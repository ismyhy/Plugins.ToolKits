using Plugins.ToolKits.Extensions;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Plugins.ToolKits.Communication.Base
{
    internal enum ConnectMode
    {
        Udp,
        Tcp
    }

    internal sealed class EasyBase : IDisposable
    {
        private readonly ConnectMode _connectMode;
        private readonly IDictionary<long, ResultWaiter> _syncHandles = new ConcurrentDictionary<long, ResultWaiter>();


        internal Func<byte[], IPEndPoint, Task> SendFunctionAsync;

        internal Func<IEasySession, byte[], Task> Received;

        internal EasyBase(ConnectOptions options, ConnectMode mode)
        {
            ConnectOptions = options ?? throw new ArgumentNullException(nameof(options));
            _connectMode = mode;
        }

        internal ConnectOptions ConnectOptions { get; }

        public void Dispose()
        {
            Received = null;
            _syncHandles.ForEachAsync(i => i.Value.Dispose());
        }


        internal async void ReceivedHandler(byte[] receiveBytes, int offset, int length,
            IPEndPoint endPoint = null)
        {
            ICollection<ProtocolPacket> buffers = ProtocolPacket.ParseBuffers(receiveBytes, offset, length);

            foreach (ProtocolPacket buffer in buffers)
            {
                if (endPoint != null)
                {
                    buffer.Ip = endPoint.Address.GetAddressBytes();
                    buffer.Port = endPoint.Port;
                }

                await InnerChannelReceived(buffer);
            }
        }


        /// <param name="protocol"></param>
        private async Task InnerChannelReceived(ProtocolPacket protocol)
        {
            if (protocol.ReportArrived && protocol.PacketMode != PacketMode.Arrived)
            {
                ProtocolPacket p = protocol.CopyTo(new ProtocolPacket());
                p.Data = new byte[0];
                p.HasResponse = false;
                p.PacketMode = PacketMode.Arrived;
                p.UsingRemoteEndPoint = true;
                SendAsync(p).NoAwaiter();
            }

            if (_syncHandles.TryGetValue(protocol.Counter, out ResultWaiter syncHandle))
            {
                syncHandle?.Set(protocol);
            }

            if (protocol.PacketMode != PacketMode.Request)
            {
                return;
            }

            Task CallBack(ProtocolPacket packet, int millisecondsTimeout = -1)
            {
                IPEndPoint remoteEndPoint = new IPEndPoint(new IPAddress(packet.Ip), packet.Port);
                packet.Ip = ConnectOptions.LocalEndPoint.Address.GetAddressBytes();
                packet.Port = ConnectOptions.LocalEndPoint.Port;
                packet.UsingRemoteEndPoint = true;


                packet.ReportArrived = _connectMode == ConnectMode.Udp;

                SendAsync(packet, remoteEndPoint, millisecondsTimeout).NoAwaiter();
                return Task.Delay(0);
            }

            ProtocolPacket newPro = protocol.CopyTo(new ProtocolPacket());
            newPro.HasResponse = false;
            newPro.PacketMode = PacketMode.Response;

            EasySession session = new EasySession(newPro, CallBack);

            if (Received != null)
            {
                await Received(session, protocol.Data);
            }
        }

        internal Task<byte[]> SendAsync(ProtocolPacket packet, IPEndPoint remoteEndPoint = null,
            int millisecondsTimeout = -1)
        {
            return Task.Factory.StartNew(() =>
            {
                ResultWaiter awaiter = null;

                byte[] buffer = packet.ToBuffer();

                IPEndPoint remotePoint = remoteEndPoint ?? new IPEndPoint(new IPAddress(packet.Ip), packet.Port);

                if (packet.ReportArrived || packet.HasResponse)
                {
                    _syncHandles[packet.Counter] = awaiter = new ResultWaiter();
                }

                SendFunctionAsync(buffer, remotePoint).NoAwaiter();

                if (packet.ReportArrived)
                {
                    if (packet.PacketMode == PacketMode.Arrived)
                    {
                        return new byte[0];
                    }


                    int waitCount = 0;

                    while (!awaiter.WaitArrive(millisecondsTimeout))
                    {
                        if (waitCount++ >= 3)
                        {
                            throw new TimeoutException("Waiting for command delivery timeout");
                        }


                        SendFunctionAsync(buffer, remotePoint).NoAwaiter();
                    }
                }

                if (!packet.HasResponse)
                {
                    _syncHandles.TryRemove(packet.Counter);
                    return new byte[0];
                }

                byte[] resultBuffer = awaiter?.WaitResponse(millisecondsTimeout);

                _syncHandles.TryRemove(packet.Counter);

                return resultBuffer;
            }, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }

        internal IPEndPoint GetLocalEndPoint()
        {
            bool ArrayEquals<T>(IReadOnlyList<T> array, IReadOnlyList<T> array2, int removeLength)
            {
                bool flag = true;
                for (int j = 0, k = array2.Count - removeLength; j < k; j++)
                {
                    flag &= Equals(array2[j], array[j]);
                }

                return flag;
            }

            List<IPAddress> ips = Dns.GetHostAddresses(Dns.GetHostName()).Where(i => i.AddressFamily == AddressFamily.InterNetwork)
                .ToList();
            byte[] bytes = ConnectOptions.RemoteEndPoint.Address.GetAddressBytes();

            byte[] defaultAddress = new byte[] { 127, 0, 0, 1 };

            if (!ArrayEquals(bytes, defaultAddress, 0))
            {
                IPAddress f = ips.FirstOrDefault(i => ArrayEquals(i.GetAddressBytes(), bytes, 1));

                if (f is null)
                {
                    throw new COMException("The correct IP is not configured");
                }

                defaultAddress = f.GetAddressBytes();
            }

            return new IPEndPoint(new IPAddress(defaultAddress), ConnectOptions.GetAvailablePort());
        }
    }
}