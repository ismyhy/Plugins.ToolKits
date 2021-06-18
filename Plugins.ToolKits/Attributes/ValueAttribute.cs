using System;

namespace Plugins.ToolKits.Attributes
{
    /// <summary>
    /// Value tag
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Enum)]
    public class ValueAttribute : Attribute
    {
        /// <summary>
        /// Constructors
        /// </summary>
        /// <param name="value"></param>
        public ValueAttribute([NotNull] object value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// The value
        /// </summary>
        public object Value { get; }

    }
}