using System;
using System.Runtime.CompilerServices;

namespace Plugins.ToolKits
{
    public static partial class Thrower
    {
        public static void IfTrue<TException>(Func<bool> predicate, string message, bool displayFilePath = false,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
            where TException : Exception
        {
            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (!predicate.Invoke())
            {
                return;
            }

            string throwExceptionMessage = FormatMessage(callerFilePath, callerLineNumber, displayFilePath);
            throw (TException)Activator.CreateInstance(typeof(TException), $"{message}{throwExceptionMessage}");
        }

        public static void IfFalse<TException>(Func<bool> predicate, string message, bool displayFilePath = false,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
            where TException : Exception
        {
            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (predicate.Invoke())
            {
                return;
            }

            string throwExceptionMessage = FormatMessage(callerFilePath, callerLineNumber, displayFilePath);
            throw (TException)Activator.CreateInstance(typeof(TException), $"{message}{throwExceptionMessage}");
        }


        public static void IfTrue<TException>(bool condition, string message, bool displayFilePath = false,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
            where TException : Exception
        {
            if (!condition)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            string throwExceptionMessage = FormatMessage(callerFilePath, callerLineNumber, displayFilePath);
            throw (TException)Activator.CreateInstance(typeof(TException), $"{message}{throwExceptionMessage}");
        }

        public static void IfFalse<TException>(bool condition, string message, bool displayFilePath = false,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
            where TException : Exception
        {
            if (condition)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            string throwExceptionMessage = FormatMessage(callerFilePath, callerLineNumber, displayFilePath);
            throw (TException)Activator.CreateInstance(typeof(TException), $"{message}{throwExceptionMessage}");
        }



        public static void Throw<TException>(this TException exception, bool displayFilePath = false,
           [CallerFilePath] string callerFilePath = null,
           [CallerLineNumber] int callerLineNumber = 0)
           where TException : Exception
        {
            if (exception is null)
            {
                return;
            }

            string throwExceptionMessage = FormatMessage(callerFilePath, callerLineNumber, displayFilePath);
            throw new ThrowerException(throwExceptionMessage, exception);
        }


    }
}