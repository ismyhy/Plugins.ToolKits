namespace Plugins.ToolKits.Communication
{
    public sealed class PacketOptions
    {
        public bool HasResponse { get; set; } = true;

        public bool CompressBuffer { get; set; }
    }
}