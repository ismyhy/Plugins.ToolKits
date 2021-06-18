using Plugins.ToolKits.Communication.Base;
using Plugins.ToolKits.Communication.Host;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Plugins.ToolKits.Communication
{
    public abstract class EasyTcp<THost> : IEasyChannel, IEasyHost where THost : EasyTcp<THost>
    {
        private readonly byte[] _bufferPool = new byte[1024 * 1024 * 64];
        private readonly EasyBase _easyBase;

        private readonly IDictionary<EasyTcp<THost>, DateTime> _tcpChannels = new ConcurrentDictionary<EasyTcp<THost>, DateTime>();

        private readonly Semaphore semaphore = new Semaphore(1, 1);

        private TcpListener _listener;

        protected EasyTcp(ConnectOptions options)
        {
            _easyBase = new EasyBase(options, ConnectMode.Tcp)
            {
                Received = Received,
                SendFunctionAsync = SendAsync
            };
            ConnectOptions = options ?? throw new ArgumentNullException(nameof(options));
        }

        protected internal ConnectOptions ConnectOptions { get; }


        public Task StopAsync()
        {
            return Task.Factory.StartNew(Dispose, CancellationToken.None, TaskCreationOptions.DenyChildAttach,
                TaskScheduler.Default);
        }


        public virtual void Dispose()
        {
            _easyBase.Dispose();
            _listener?.Stop();
            ((IDisposable)_tcpClient).Dispose();
            _tcpChannels.ForEach(i => i.Key?.Dispose());
        }


        private Task SendAsync(byte[] buffer, IPEndPoint remoteEndPoint)
        {
            semaphore.WaitOne(-1);
            _tcpClient.GetStream().BeginWrite(buffer, 0, buffer.Length, ar =>
            {
                ((TcpClient)ar.AsyncState).GetStream().EndWrite(ar);
                semaphore.Release(1);
            }, _tcpClient);
            return Task.Delay(0);
        }


        private ProtocolPacket CreatePacket(byte[] buffer, PacketOptions options)
        {
            ProtocolPacket packet = new ProtocolPacket
            {
                Ip = ((IPEndPoint)_tcpClient.Client.LocalEndPoint).Address.GetAddressBytes(),
                Port = ((IPEndPoint)_tcpClient.Client.LocalEndPoint).Port,
                Data = buffer,
                PacketMode = PacketMode.Request,
                IsCompress = options?.CompressBuffer ?? false,
                HasResponse = options?.HasResponse ?? true,
                ReportArrived = false
            };
            return packet;
        }

        #region Client

        private Action<EasyTcp<THost>> _removeAction;

        private TcpClient _tcpClient;

        public IEasyChannel RunAsClientAsync()
        {
            if (_tcpClient is null)
            {
                IPEndPoint localEndPoint = ConnectOptions.LocalEndPoint ?? _easyBase.GetLocalEndPoint();
                _tcpClient = new TcpClient(localEndPoint);
                _tcpClient.Connect(ConnectOptions.RemoteEndPoint);
            }

            _tcpClient.ReceiveBufferSize = _bufferPool.Length;
            _tcpClient.SendBufferSize = _bufferPool.Length;
            NetworkStream stream = _tcpClient.GetStream();
            stream.BeginRead(_bufferPool, 0, _bufferPool.Length, HandleDataReceived, stream);
            return this;
        }

        /// <summary>
        ///     数据接受回调函数
        /// </summary>
        /// <param name="ar"></param>
        private void HandleDataReceived(IAsyncResult ar)
        {
            NetworkStream stream1 = (NetworkStream)ar.AsyncState;
            int receivedLength;
            try
            {
                receivedLength = stream1.EndRead(ar);
                _easyBase.ReceivedHandler(_bufferPool, 0, receivedLength);
                stream1.BeginRead(_bufferPool, 0, _bufferPool.Length, HandleDataReceived, stream1);
            }
            catch
            {
                receivedLength = 0;
            }

            if (receivedLength == 0)
            {
                _removeAction?.Invoke(this);
            }
        }


        async Task<byte[]> IEasyChannel.SendAsync([NotNull] byte[] buffer, PacketOptions options, int millisecondsTimeout)
        {
            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }
            ProtocolPacket packet = CreatePacket(buffer, options);

            return await _easyBase.SendAsync(packet, ConnectOptions.RemoteEndPoint, millisecondsTimeout);
        }

        Task<byte[]> IEasyChannel.SendAsync([NotNull] byte[] buffer, int millisecondsTimeout)
        {
            return ((IEasyChannel)this).SendAsync(buffer, null, millisecondsTimeout);
        }

        public Task<byte[]> SendAsync([NotNull] IEasyChannel channel, [NotNull] byte[] buffer, int millisecondsTimeout = -1)
        {
            return channel.SendAsync(buffer, null, millisecondsTimeout);
        }

        #endregion


        #region Server

        async Task<byte[]> IEasyHost.SendAsync([NotNull] byte[] buffer, PacketOptions options, int millisecondsTimeout)
        {
            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            ProtocolPacket packet = CreatePacket(buffer, options);

            _tcpChannels.Keys.AsParallel().ForAll(async i =>
            {
                await i._easyBase.SendAsync(packet, ConnectOptions.RemoteEndPoint, millisecondsTimeout);
            });

            return await Task.FromResult(new byte[0]);
        }

        Task<byte[]> IEasyHost.SendAsync(byte[] buffer, int millisecondsTimeout)
        {
            return ((IEasyHost)this).SendAsync(buffer, null, millisecondsTimeout);
        }

        public IEasyHost RunAsServerAsync()
        {
            _listener = new TcpListener(ConnectOptions.LocalEndPoint);
            _listener.Start();
            _listener.BeginAcceptTcpClient(HandleTcpClientAccepted, _listener);
            return this;
        }

        private void HandleTcpClientAccepted(IAsyncResult ar)
        {
            TcpClient client = _listener.EndAcceptTcpClient(ar);

            THost channel = (THost)Activator.CreateInstance(typeof(THost), new ConnectOptions
            {
                RemoteEndPoint = client.Client.RemoteEndPoint as IPEndPoint,
                LocalEndPoint = client.Client.LocalEndPoint as IPEndPoint
            });

            channel._tcpClient = client;
            channel._removeAction = channel2 => _tcpChannels.TryRemove(channel2);
            channel.RunAsClientAsync();
            _tcpChannels[channel] = DateTime.Now;

            Task.Factory.StartNew(() => AcceptChannel(channel));

            _listener.BeginAcceptTcpClient(HandleTcpClientAccepted, ar.AsyncState);
        }


        protected virtual void AcceptChannel(IEasyChannel channel)
        {
        }

        protected virtual Task Received(IEasySession session, byte[] buffer)
        {
            return Task.Delay(0);
        }

        #endregion
    }
}