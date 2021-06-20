using System.Threading;

namespace Plugins.ToolKits.Transmission
{
    public class PacketSetting
    {
        public int MillisecondsTimeout { get; set; } = -1;

        public bool ReportArrived { get; set; }

        public bool IsCompressBuffer { get; set; }

        public CancellationToken CancellationToken { get; set; } = CancellationToken.None;
    }
}
