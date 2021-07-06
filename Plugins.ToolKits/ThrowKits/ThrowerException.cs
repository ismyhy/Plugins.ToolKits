using System;

namespace Plugins.ToolKits
{
    public class ThrowerException
        : Exception
    {
        internal ThrowerException(string message) : base(message)
        {
        }
        internal ThrowerException(string message, Exception innerException) : base(message, innerException)
        {
        }

        internal ThrowerException(string targetName, string message) : base(message)
        {
            TargetName = targetName;
        }

        public string TargetName { get; }


        public override string ToString()
        {
            return $"{TargetName}{Environment.NewLine}{Message}";
        }
    }
}