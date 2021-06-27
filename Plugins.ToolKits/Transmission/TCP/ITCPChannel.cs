using Plugins.ToolKits.Transmission.Protocol;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Plugins.ToolKits.Transmission.TCP
{


    public interface ITCPChannel
    {
        ITCPChannel RunAsync();

        Task<int> SendAsync(byte[] buffer, int offset, int length, PacketSetting setting = null);

        int Send(byte[] buffer, int offset, int length, PacketSetting setting = null);

        void Close();
    }

    public class TCPChannel : ITCPChannel
    {
        public readonly ContextContainer Context = new ContextContainer();
        public TcpClient TcpClient { get; private set; }
        public TCPChannel(IPEndPoint localEndPoint, IPEndPoint remoteEndPoint)
        {
            if (localEndPoint == null)
            {
                throw new ArgumentNullException(nameof(localEndPoint));
            }

            if (remoteEndPoint == null)
            {
                throw new ArgumentNullException(nameof(remoteEndPoint));
            }
            Context.Set(TransmissionKeys.RemoteIPEndPoint, remoteEndPoint);
            TcpClient = new TcpClient(localEndPoint);
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





        #region Base

        public int SendTimeout
        {
            get => TcpClient.SendTimeout;
            set => TcpClient.SendTimeout = value;
        }

        public int ReceiveTimeout
        {
            get => TcpClient.ReceiveTimeout;
            set => TcpClient.ReceiveTimeout = value;
        }
        public int SendBufferSize
        {
            get => TcpClient.SendBufferSize;
            set => TcpClient.SendBufferSize = value;
        }

        public int ReceiveBufferSize
        {
            get => TcpClient.ReceiveBufferSize;
            set => TcpClient.ReceiveBufferSize = value;
        }

        public bool ExclusiveAddressUse
        {
            get => TcpClient.ExclusiveAddressUse;
            set => TcpClient.ExclusiveAddressUse = value;
        }

        public bool Connected => TcpClient.Connected;
        public LingerOption LingerState
        {
            get => TcpClient.LingerState;
            set => TcpClient.LingerState = value;
        }

        public bool NoDelay
        {
            get => TcpClient.NoDelay;
            set => TcpClient.NoDelay = value;
        }

        #endregion


        #region

        public ITCPChannel RunAsync()
        {
            IPEndPoint remoteEndPoint = Context.Get<IPEndPoint>(TransmissionKeys.RemoteIPEndPoint);
            TcpClient.Connect(remoteEndPoint);
            byte[] bufferPool = new byte[TcpClient.ReceiveBufferSize];
            TCPSession session = new TCPSession(Context)
            {
                RemoteEndPoint = remoteEndPoint
            };

            NetworkStream stream = TcpClient.GetStream();
            stream.BeginRead(bufferPool, 0, bufferPool.Length, HandleDataReceived, stream);

            return this;

            void HandleDataReceived(IAsyncResult iar)
            {
                if (iar.AsyncState is not NetworkStream stream1 || !Connected)
                {
                    return;
                }

                int receivedLength = stream1.EndRead(iar);

                ICollection<ProtocolPacket> protocols = ProtocolPacket.FromBuffers(bufferPool, 0, receivedLength);

                stream1.BeginRead(bufferPool, 0, bufferPool.Length, HandleDataReceived, stream1);

                foreach (ProtocolPacket protocol in protocols)
                {
                    byte[] dataBuffer = protocol.Data;
                    if (protocol.ReportArrived)
                    {
                        Task.Factory.StartNew(() =>
                        {
                            protocol.Data = new byte[0];
                            protocol.Offset = 0;
                            protocol.DataLength = 0;
                            protocol.UsingRemoteEndPoint = true;
                            protocol.ReportArrived = false;
                            PacketSender(protocol, remoteEndPoint);
                        }, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
                    }


                    if (SyncHandles.TryGetValue(protocol.Counter, out EventWaitHandle waitHandle))
                    {
                        waitHandle?.Set();
                        return;
                    }

                    Recived(session, dataBuffer);
                }

            }

        }
        private readonly IDictionary<long, EventWaitHandle> SyncHandles = new ConcurrentDictionary<long, EventWaitHandle>();

        public Task<int> SendAsync(byte[] buffer, int offset, int length, PacketSetting setting = null)
        {
            return Task.Factory.StartNew(() =>
            {
                return Send(buffer, offset, length, setting);
            }, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }

        public int Send(byte[] buffer, int offset, int length, PacketSetting setting = null)
        {
            IPEndPoint remoteEndPoint = Context.Get<IPEndPoint>(TransmissionKeys.RemoteIPEndPoint);

            ProtocolPacket packet = ProtocolPacket.BuildPacket(buffer, offset, length, setting);

            return PacketSender(packet, remoteEndPoint, setting?.MillisecondsTimeout ?? -1);
        }


        private int PacketSender(ProtocolPacket protocol, IPEndPoint remoteEndPoint, int millisecondsTimeout = -1)
        {
            if (!Context.TryGet<Semaphore>(TransmissionKeys.Semaphore, out Semaphore semaphore))
            {
                throw new ArgumentNullException(nameof(TransmissionKeys.Semaphore));
            }

            EventWaitHandle awaiter = null;

            byte[] buffer = protocol.ToBuffer();

            if (protocol.ReportArrived)
            {
                SyncHandles[protocol.Counter] = awaiter = new ManualResetEvent(false);
            }
            semaphore.WaitOne(-1);

            int sendCount = buffer.Length;
            try
            {
                NetworkStream stream = TcpClient.GetStream();
                stream.Write(buffer, 0, sendCount);
            }
            catch (Exception)
            {
                semaphore.Release(1);
                return 0;
            }

            if (!protocol.ReportArrived)
            {
                semaphore.Release(1);
                return sendCount;
            }
            awaiter?.WaitOne(millisecondsTimeout);
            semaphore.Release(1);
            SyncHandles.TryRemove(protocol.Counter);

            return sendCount;
        }


        public void Close()
        {
            throw new NotImplementedException();
        }

        #endregion
    }


    internal class TCPSession : ISession
    {
        private readonly ContextContainer Context;
        public TCPSession(ContextContainer context)
        {
            Context = context;
        }
        public IPEndPoint RemoteEndPoint { get; set; }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public int Send(byte[] buffer, int offset, int length, PacketSetting setting = null)
        {
            throw new NotImplementedException();
        }

        public Task<int> SendAsync(byte[] buffer, int offset, int length, PacketSetting setting = null)
        {
            throw new NotImplementedException();
        }
    }

}
