
using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
namespace Plugins.ToolKits.Transmission
{
    public interface ISession : IDisposable
    {
        IPEndPoint RemoteEndPoint { get; }

        Task<int> SendAsync(byte[] buffer, int offset, int length, PacketSetting setting = null);

        int Send(byte[] buffer, int offset, int length, PacketSetting setting = null);
    }

}
