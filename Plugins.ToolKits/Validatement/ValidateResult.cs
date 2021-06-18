using System;

namespace Plugins.ToolKits.Validatement
{
    public readonly struct ValidateResult : IEquatable<ValidateResult>
    {
        private static readonly ValidateResult _Valid = new ValidateResult(true);
        private static readonly ValidateResult _Invalid = new ValidateResult(false);

        private ValidateResult(bool result)
        {
            Flag = result;
        }

        private readonly bool Flag;

        public static readonly ValidateResult Valid = _Valid;

        public static readonly ValidateResult Invalid = _Invalid;


        public override string ToString()
        {
            return Flag ? nameof(Valid) : nameof(Invalid);
        }


        public static bool operator ==(ValidateResult result1, ValidateResult result2)
        {
            return result1.Flag == result2.Flag;
        }

        public static bool operator !=(ValidateResult result1, ValidateResult result2)
        {
            return result1.Flag != result2.Flag;
        }


        public override bool Equals(object obj)
        {
            if (obj is ValidateResult result)
            {
                return Flag == result.Flag;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Flag ? 1 : 0;
        }


        public bool Equals(ValidateResult other)
        {
            return Flag == other.Flag;
        }
    }
}