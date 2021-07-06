
using System;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Plugins.ToolKits
{
    public static partial class Thrower
    {
        public static void IsNull<TObject>(this TObject target, string targetName, bool displayFilePath = false,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0) where TObject : class
        {
            if (target != null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(targetName))
            {
                throw new ArgumentNullException(nameof(targetName));
            }


            string message = FormatMessage(callerFilePath, callerLineNumber, displayFilePath);

            BreakInDebuggerIfAttached();

            throw new ThrowerException(targetName, $"{targetName} is Null{message}");
        }


        public static void IsNull<TObject>(Expression<Func<TObject>> targetExpression, bool displayFilePath = false,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0) where TObject : class
        {
            if (targetExpression is null)
            {
                throw new ArgumentNullException(nameof(targetExpression));
            }

            if (targetExpression.Compile().Invoke() != null)
            {
                return;
            }

            string targetName = targetExpression.GetMemberName();

            string message = FormatMessage(callerFilePath, callerLineNumber, displayFilePath);

            BreakInDebuggerIfAttached();

            throw new ThrowerException(targetName, $"{targetName} is Null{message}");
        }


        public static void IfTrue<TObject>(Expression<Func<TObject>> targetExpression,
            [NotNull] Func<TObject, bool> checkFunc, bool displayFilePath = false,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            if (targetExpression is null)
            {
                throw new ArgumentNullException(nameof(targetExpression));
            }

            if (checkFunc is null)
            {
                throw new ArgumentNullException(nameof(checkFunc));
            }

            TObject target = targetExpression.Compile().Invoke();
            string targetName = targetExpression.GetMemberName();

            if (!checkFunc.Invoke(target))
            {
                return;
            }

            string message = FormatMessage(callerFilePath, callerLineNumber, displayFilePath);

            BreakInDebuggerIfAttached();

            throw new ThrowerException(targetName, $"{targetName} is Error{message}");
        }

        public static void IfFalse<TObject>(Expression<Func<TObject>> targetExpression,
            [NotNull] Func<TObject, bool> checkFunc, bool displayFilePath = false,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            if (targetExpression is null)
            {
                throw new ArgumentNullException(nameof(targetExpression));
            }

            if (checkFunc is null)
            {
                throw new ArgumentNullException(nameof(checkFunc));
            }

            TObject target = targetExpression.Compile().Invoke();
            string targetName = targetExpression.GetMemberName();

            if (checkFunc.Invoke(target))
            {
                return;
            }

            string message = FormatMessage(callerFilePath, callerLineNumber, displayFilePath);

            BreakInDebuggerIfAttached();

            throw new ThrowerException(targetName, $"{targetName} is Error{message}");
        }


        public static void IfTrue(Func<bool> predicate, string message, bool displayFilePath = false,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
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

            string message1 = FormatMessage(callerFilePath, callerLineNumber, displayFilePath);

            BreakInDebuggerIfAttached();

            throw new ThrowerException($"{message}{message1}");
        }

        public static void IfFalse(Func<bool> predicate, string message, bool displayFilePath = false,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
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

            string message1 = FormatMessage(callerFilePath, callerLineNumber, displayFilePath);

            BreakInDebuggerIfAttached();

            throw new ThrowerException($"{message}{message1}");
        }


        public static void IfTrue(bool condition, string message, bool displayFilePath = false,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            if (!condition)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            string message1 = FormatMessage(callerFilePath, callerLineNumber, displayFilePath);

            BreakInDebuggerIfAttached();

            throw new ArgumentException($"{message}{message1}");
        }

        public static void IfFalse(bool condition, string message, bool displayFilePath = false,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            if (!condition)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException(nameof(message));
            }


            string message1 = FormatMessage(callerFilePath, callerLineNumber, displayFilePath);

            BreakInDebuggerIfAttached();

            throw new ArgumentException($"{message}{message1}");
        }


        public static Exception FindRootException(this Exception exception)
        {
            Exception ex = exception;

            while (ex.InnerException is Exception e)
            {
                ex = e;
            }

            return ex;
        }

        [Conditional("DEBUG")]
        private static void BreakInDebuggerIfAttached()
        {
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
        }

        private static string FormatMessage(string callerFilePath, int callerLineNumber, bool displayFilePath = false)
        {
            const string message = "{1}On Line:{0} {1}Of the File:{2}";

            string path = displayFilePath ? callerFilePath : Path.GetFileName(callerFilePath);

            return string.Format(message, callerLineNumber, Environment.NewLine, path);
        }
    }
}