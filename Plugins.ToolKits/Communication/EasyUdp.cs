using Plugins.ToolKits.Communication.Base;
using Plugins.ToolKits.Communication.Host;

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Plugins.ToolKits.Communication
{
    public abstract class EasyUdp : IEasyChannel, IEasyHost, IDisposable
    {
        private readonly EasyBase _easyBase;
        private UdpClient _client;
        private IPEndPoint _localEndPoint, _sendIpEndPoint;

        protected EasyUdp([NotNull] ConnectOptions options)
        {
            ConnectOptions = options ?? throw new ArgumentNullException(nameof(options));

            _easyBase = new EasyBase(options, ConnectMode.Udp)
            {
                Received = Received,
                SendFunctionAsync = SendAsync
            };
        }


        protected internal ConnectOptions ConnectOptions { get; }

        public void Dispose()
        {
            _easyBase?.Dispose();
            _client.Close();
            ((IDisposable)_client).Dispose();
            _client = null;
        }

        protected virtual Task Received(IEasySession session, byte[] buffer)
        {
            return Task.Delay(0);
        }

        private void ReceiveCallback(IAsyncResult iar)
        {
            if (!(iar.AsyncState is UdpClient udpClient))
            {
                return;
            }

            if (iar.IsCompleted)
            {
                IPEndPoint receivedRemoteEndPoint = new IPEndPoint(_localEndPoint.Address, _localEndPoint.Port);
                byte[] receiveBytes = udpClient.EndReceive(iar, ref receivedRemoteEndPoint);
                _easyBase.ReceivedHandler(receiveBytes, 0, receiveBytes.Length, receivedRemoteEndPoint);
            }


            udpClient.BeginReceive(ReceiveCallback, udpClient);
        }


        #region Run

        public IEasyChannel RunAsClientAsync()
        {
            _localEndPoint = ConnectOptions.LocalEndPoint ?? _easyBase.GetLocalEndPoint();

            _sendIpEndPoint = ConnectOptions.RemoteEndPoint;
            _client = new UdpClient(_localEndPoint);
            _client.BeginReceive(ReceiveCallback, _client);
            return this;
        }


        public IEasyHost RunAsServerAsync()
        {
            _localEndPoint = new IPEndPoint(ConnectOptions.LocalEndPoint.Address, ConnectOptions.LocalEndPoint.Port);

            _client = new UdpClient(_localEndPoint);

            _client.BeginReceive(ReceiveCallback, _client);
            return this;
        }

        public Task StopAsync()
        {
            return Task.Factory.StartNew(Dispose, CancellationToken.None, TaskCreationOptions.DenyChildAttach,
                TaskScheduler.Default);
        }

        public async Task<byte[]> SendAsync([NotNull] byte[] buffer, int millisecondsTimeout)
        {
            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            return await SendAsync(buffer, null, millisecondsTimeout);
        }

        public async Task<byte[]> SendAsync(IEasyChannel channel, [NotNull] byte[] buffer, int millisecondsTimeout = -1)
        {
            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            return await channel.SendAsync(buffer, millisecondsTimeout);
        }

        public async Task<byte[]> SendAsync([NotNull] byte[] buffer, PacketOptions options, int millisecondsTimeout)
        {
            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            ProtocolPacket packet = CreatePacket(buffer, options);

            return await _easyBase.SendAsync(packet, _sendIpEndPoint, millisecondsTimeout);
        }

        #endregion

        #region Base

        private ProtocolPacket CreatePacket(byte[] buffer, PacketOptions options)
        {
            return new ProtocolPacket
            {
                Ip = _localEndPoint.Address.GetAddressBytes(),
                Port = _localEndPoint.Port,
                Data = buffer,
                PacketMode = PacketMode.Request,
                IsCompress = options?.CompressBuffer ?? false,
                HasResponse = options?.HasResponse ?? true
            };
        }

        private Task SendAsync(byte[] buffer, IPEndPoint remoteEndPoint)
        {
            return _client.SendAsync(buffer, buffer.Length, remoteEndPoint);
        }

        #endregion
    }
}