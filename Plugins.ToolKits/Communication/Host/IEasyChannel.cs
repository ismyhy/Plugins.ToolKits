using System.Threading.Tasks;

namespace Plugins.ToolKits.Communication.Host
{
    public interface IEasyChannel
    {
        Task StopAsync();
        Task<byte[]> SendAsync(byte[] buffer, PacketOptions options, int millisecondsTimeout = -1);
        Task<byte[]> SendAsync(byte[] buffer, int millisecondsTimeout = -1);
    }
}