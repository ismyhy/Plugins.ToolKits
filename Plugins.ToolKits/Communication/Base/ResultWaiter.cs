using System;
using System.Threading;
using System.Threading.Tasks;

namespace Plugins.ToolKits.Communication.Base
{
    internal sealed class ResultWaiter : IDisposable
    {
        private readonly EventWaitHandle _arrivedResetEvent = new ManualResetEvent(false);
        private readonly EventWaitHandle _responseResetEvent = new ManualResetEvent(false);

        private bool _arrivedReset, _responseReset;

        private byte[] _buffer;

        public void Dispose()
        {
            _arrivedResetEvent?.Dispose();
            _responseResetEvent?.Dispose();
        }

        public Task DisposeAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                _arrivedResetEvent?.Dispose();
                _responseResetEvent?.Dispose();
            }, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }

        public bool WaitArrive(int millisecondsTimeout = -1)
        {
            if (_arrivedReset)
            {
                _arrivedResetEvent.Dispose();
                return true;
            }

            if (!_arrivedResetEvent.WaitOne(millisecondsTimeout))
            {
                return false;
            }

            _arrivedResetEvent.Dispose();
            return true;

        }

        public bool Set(ProtocolPacket paBase)
        {
            switch (paBase.PacketMode)
            {
                case PacketMode.Arrived:
                    return _arrivedReset = _arrivedResetEvent.Set();
                case PacketMode.Response:
                    _buffer = paBase.Data;
                    return _responseReset = _responseResetEvent.Set();
                default:
                    return false;
            }
        }


        public byte[] WaitResponse(int millisecondsTimeout = -1)
        {
            if (_responseReset)
            {
                _responseResetEvent.Dispose();
                return _buffer;
            }

            if (!_responseResetEvent.WaitOne(millisecondsTimeout))
            {
                throw new TimeoutException("Waiting for delivery timeout");
            }

            _responseResetEvent.Dispose();
            return _buffer;
        }
    }
}