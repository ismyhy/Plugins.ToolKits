using System;

namespace Plugins.ToolKits
{
    public class CheckerException : Exception
    {
        internal CheckerException(string message) : base(message)
        {
        }

        internal CheckerException(string targetName, string message) : base(message)
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