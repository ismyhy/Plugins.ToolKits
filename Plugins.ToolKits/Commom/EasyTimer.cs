
using System;
using System.ComponentModel;

namespace Plugins.ToolKits
{

    public sealed class EasyTimer : IDisposable
    {
        public static EasyTimer Share => new EasyTimer();

        private readonly System.Timers.Timer timer;
        private Action callbackAction;
        private Action<object, System.Timers.ElapsedEventArgs> callbackAction2;
        public bool IsRunning { get; private set; }
        public EasyTimer()
        {
            timer = new System.Timers.Timer();
            timer.AutoReset = true;
            timer.Elapsed += Timer_Elapsed;
        }

        public void Dispose()
        {
            IsRunning = false;
            timer.Elapsed -= Timer_Elapsed;
            timer.Stop();
            timer.Dispose();
        }

        public EasyTimer UesCallback(Action callbackAction)
        {
            if (callbackAction == null)
            {
                throw new ArgumentNullException(nameof(callbackAction));
            }
            this.callbackAction = callbackAction;
            return this;
        }
        public EasyTimer UesCallback(Action<object, System.Timers.ElapsedEventArgs> callbackAction)
        {
            if (callbackAction == null)
            {
                throw new ArgumentNullException(nameof(callbackAction));
            }
            callbackAction2 = callbackAction;
            return this;
        }

        public EasyTimer UseAutoReset(bool autoReset)
        {
            timer.AutoReset = autoReset;
            return this;
        }
        public EasyTimer UseInterval(int milliseconds)
        {
            timer.Interval = milliseconds;
            return this;
        }
        public EasyTimer UseSynchronizingObject(ISynchronizeInvoke synchronizingObject)
        {
            if (synchronizingObject == null)
            {
                throw new ArgumentNullException(nameof(synchronizingObject));
            }
            timer.SynchronizingObject = synchronizingObject;
            return this;
        } 
        
        public EasyTimer UseSite(ISite site)
        {
            if (site == null)
            {
                throw new ArgumentNullException(nameof(site));
            }
            timer.Site = site;
            return this;
        }
        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            callbackAction?.Invoke();
            callbackAction2?.Invoke(sender, e);
        }

        public EasyTimer RunAsync()
        {
            timer.Start();
            IsRunning = true;
            return this;
        }

        public void Exit()
        {
            IsRunning = false;
            timer.Stop();
        }
    }
}