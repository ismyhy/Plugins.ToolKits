
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace Plugins.ToolKits
{
    public sealed class RunTimer : IDisposable
    {
        private static readonly IDictionary<object, RunTimer> timerLongs =
            new ConcurrentDictionary<object, RunTimer>();

        private readonly Stopwatch stopwatch;

        public RunTimer([NotNull] object token)
        {
            Token = token ?? throw new ArgumentNullException(nameof(token));
            NowTimer = DateTime.Now;
            stopwatch = Stopwatch.StartNew();
        }

        public bool IsRunning => stopwatch.IsRunning;


        public object Token { get; }

        public DateTime NowTimer { get; }

        public static string DateTimeString => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

        public static string TimeString => DateTime.Now.ToString("HH:mm:ss.fff");

        public static string DateString => DateTime.Now.ToString("yyyy-MM-dd");

        public void Dispose()
        {
            if (IsRunning)
            {
                stopwatch.Stop();
            }
        }

        public TimeSpan GetTimeSpan()
        {
            if (IsRunning)
            {
                stopwatch.Stop();
            }

            return stopwatch.Elapsed;
        }

        public long GetTotalMilliseconds()
        {
            if (IsRunning)
            {
                stopwatch.Stop();
            }

            return stopwatch.ElapsedMilliseconds;
        }

        public long GetTotalTicks()
        {
            if (IsRunning)
            {
                stopwatch.Stop();
            }

            return stopwatch.ElapsedTicks;
        }

        public override string ToString()
        {
            return $"Elapsed Time:{stopwatch.ElapsedMilliseconds} ms";
        }

        public static RunTimer StartNew()
        {
            return new RunTimer(Guid.NewGuid());
        }


        public static void SetTimer([NotNull] object token)
        {
            if (token is null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            timerLongs[token] = new RunTimer(token);
        }

        public static TimeSpan GetTimeSpan([NotNull] object token, bool removeTokenAfterRead = true)
        {
            if (token is null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            if (!timerLongs.TryGetValue(token, out RunTimer timer))
            {
                throw new NotSupportedException($"Token:{token} not set ");
            }

            if (removeTokenAfterRead)
            {
                timerLongs.Remove(token);
            }

            return timer.GetTimeSpan();
        }

        public static double GetTotalMilliseconds([NotNull] object token, bool removeTokenAfterRead = true)
        {
            if (token is null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            if (!timerLongs.TryGetValue(token, out RunTimer timer))
            {
                throw new NotSupportedException($"Token:{token} not set ");
            }

            if (removeTokenAfterRead)
            {
                timerLongs.Remove(token);
            }

            return timer.GetTotalMilliseconds();
        }

        public static RunTimer Run(Action action)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            RunTimer timer = RunTimer.StartNew();
            try
            {
                action();
            }
            finally
            {
                timer.stopwatch.Stop();
            }

            return timer;
        }

    }
}