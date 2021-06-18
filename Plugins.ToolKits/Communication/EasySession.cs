using Plugins.ToolKits.Communication.Base;

using System;
using System.Threading.Tasks;

namespace Plugins.ToolKits.Communication
{
    public interface IEasySession : IDisposable
    {
        Task SendAsync(byte[] buffer, int millisecondsTimeout = -1);

        Task SendAsync(byte[] buffer, PacketOptions options, int millisecondsTimeout = -1);
    }


    internal sealed class EasySession : IEasySession
    {
        private readonly ProtocolPacket _basePacket;
        private Func<ProtocolPacket, int, Task> _sendFunc;

        public EasySession(ProtocolPacket packet, Func<ProtocolPacket, int, Task> func)
        {
            _basePacket = packet;
            _sendFunc = func;
        }

        public void Dispose()
        {
            _basePacket?.Dispose();
            _sendFunc = null;
        }

        public async Task SendAsync([NotNull] byte[] buffer, int millisecondsTimeout = -1)
        {
            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }
            _basePacket.Data = buffer;

            await _sendFunc.Invoke(_basePacket, millisecondsTimeout);
        }


        public async Task SendAsync([NotNull] byte[] buffer, PacketOptions options, int millisecondsTimeout = -1)
        {
            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }
            _basePacket.Data = buffer;
            _basePacket.PacketMode = PacketMode.Request;
            _basePacket.HasResponse = options?.HasResponse ?? true;
            _basePacket.IsCompress = options?.CompressBuffer ?? false;
            await _sendFunc.Invoke(_basePacket, millisecondsTimeout);
        }

        public override string ToString()
        {
            return $"{string.Join(".", _basePacket.Ip)}:{_basePacket.Port}";
        }
    }
}