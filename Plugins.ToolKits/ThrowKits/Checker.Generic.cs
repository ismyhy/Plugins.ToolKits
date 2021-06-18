using System;
using System.Runtime.CompilerServices;

namespace Plugins.ToolKits
{
    public static partial class Checker
    {
        public static void True<TException>(Func<bool> predicate, string message, bool displayFilePath = false,
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

        public static void False<TException>(Func<bool> predicate, string message, bool displayFilePath = false,
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


        public static void True<TException>(bool condition, string message, bool displayFilePath = false,
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

        public static void False<TException>(bool condition, string message, bool displayFilePath = false,
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


        public static void Ignore<TException>(Action action, Action<TException> exceptionCallback = null)
            where TException : Exception
        {
            if (action is null)
            {
                return;
            }

            try
            {
                action();
            }
            catch (TException exception)
            {
                exceptionCallback?.Invoke(exception);
            }
        }
    }
}