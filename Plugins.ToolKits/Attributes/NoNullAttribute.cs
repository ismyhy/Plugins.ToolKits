using System;

namespace Plugins.ToolKits
{

    /// <summary>
    /// The tag value cannot be empty
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.GenericParameter)]
    public class NotNullAttribute : Attribute
    {
    }
}