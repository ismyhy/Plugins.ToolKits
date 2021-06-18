using Plugins.ToolKits.Attributes;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace Plugins.ToolKits
{
    public sealed class EasyTimer : IDisposable
    {
        private static readonly IDictionary<object, EasyTimer> TimerLongs =
            new ConcurrentDictionary<object, EasyTimer>();

        private readonly Stopwatch _stopwatch;

        public EasyTimer([NotNull] object token)
        {
            Token = token ?? throw new ArgumentNullException(nameof(token));
            NowTimer = DateTime.Now;
            _stopwatch = Stopwatch.StartNew();
        }

        public bool IsRunning => _stopwatch.IsRunning;


        public object Token { get; }

        public DateTime NowTimer { get; }

        public static string DateTimeString => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

        public static string TimeString => DateTime.Now.ToString("HH:mm:ss.fff");

        public static string DateString => DateTime.Now.ToString("yyyy-MM-dd");

        public void Dispose()
        {
            if (IsRunning)
            {
                _stopwatch.Stop();
            }
        }

        public TimeSpan GetTimeSpan()
        {
            if (IsRunning)
            {
                _stopwatch.Stop();
            }

            return _stopwatch.Elapsed;
        }

        public long GetTotalMilliseconds()
        {
            if (IsRunning)
            {
                _stopwatch.Stop();
            }

            return _stopwatch.ElapsedMilliseconds;
        }

        public long GetTotalTicks()
        {
            if (IsRunning)
            {
                _stopwatch.Stop();
            }

            return _stopwatch.ElapsedTicks;
        }

        public override string ToString()
        {
            return $"Elapsed Time:{_stopwatch.ElapsedMilliseconds} ms";
        }

        public static EasyTimer StartNew()
        {
            return new EasyTimer(Guid.NewGuid());
        }


        public static void SetTimer([NotNull] object token)
        {
            if (token is null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            TimerLongs[token] = new EasyTimer(token);
        }

        public static TimeSpan GetTimeSpan([NotNull] object token, bool removeTokenAfterRead = true)
        {
            if (token is null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            if (!TimerLongs.TryGetValue(token, out EasyTimer timer))
            {
                throw new NotSupportedException($"Token:{token} not set ");
            }

            if (removeTokenAfterRead)
            {
                TimerLongs.Remove(token);
            }

            return timer.GetTimeSpan();
        }

        public static double GetTotalMilliseconds([NotNull] object token, bool removeTokenAfterRead = true)
        {
            if (token is null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            if (!TimerLongs.TryGetValue(token, out EasyTimer timer))
            {
                throw new NotSupportedException($"Token:{token} not set ");
            }

            if (removeTokenAfterRead)
            {
                TimerLongs.Remove(token);
            }

            return timer.GetTotalMilliseconds();
        }

        public static EasyTimer Run(Action action)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            EasyTimer timer = EasyTimer.StartNew();
            try
            {
                action();
            }
            finally
            {
                timer._stopwatch.Stop();
            }

            return timer;
        }

    }
}