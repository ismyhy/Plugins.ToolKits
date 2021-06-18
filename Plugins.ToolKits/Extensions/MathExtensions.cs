using System;

namespace Plugins.ToolKits
{
    public static partial class MathExtensions
    {
        public static bool IsZero(this double doubleValue, double contrastAccuracy = double.Epsilon)
        {
            if (doubleValue is double.NaN)
            {
                return false;
            }


            double value = Math.Abs(doubleValue - contrastAccuracy);

            if (value > 0)
            {
                return false;
            }

            return true;
        }

        public static bool IsZero(this float floatValue, float contrastAccuracy = float.Epsilon)
        {
            if (floatValue is float.NaN)
            {
                return false;
            }

            float value = Math.Abs(floatValue - contrastAccuracy);

            if (value > 0)
            {
                return false;
            }

            return true;
        }
        public static bool IsZero(this decimal decimalValue, decimal contrastAccuracy = decimal.Zero)
        {

            decimal value = Math.Abs(decimalValue - contrastAccuracy);

            if (value > 0)
            {
                return false;
            }

            return true;
        }

        public static int FromRange(this int value, int minValue, int maxValue)
        {
            return value < minValue ? minValue : value > maxValue ? maxValue : value;
        }

        public static byte FromRange(this byte value, byte minValue, byte maxValue)
        {
            return value < minValue ? minValue : value > maxValue ? maxValue : value;
        }
        public static short FromRange(this short value, short minValue, short maxValue)
        {
            return value < minValue ? minValue : value > maxValue ? maxValue : value;
        }
        public static float FromRange(this float value, float minValue, float maxValue)
        {
            return value < minValue ? minValue : value > maxValue ? maxValue : value;
        }
        public static decimal FromRange(this decimal value, decimal minValue, decimal maxValue)
        {
            return value < minValue ? minValue : value > maxValue ? maxValue : value;
        }
        public static double FromRange(this double value, double minValue, double maxValue)
        {
            return value < minValue ? minValue : value > maxValue ? maxValue : value;
        }
        public static long FromRange(this long value, long minValue, long maxValue)
        {
            return value < minValue ? minValue : value > maxValue ? maxValue : value;
        }
    }
}