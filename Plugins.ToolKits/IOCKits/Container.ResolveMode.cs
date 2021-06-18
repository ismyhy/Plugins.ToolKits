using System.ComponentModel;

namespace Plugins.ToolKits.IOCKits
{
    public enum ResolveMode : byte
    {
        [Description("SingleTon")]
        Global,
        [Description("New Instance")]
        NewInstance
    }
}