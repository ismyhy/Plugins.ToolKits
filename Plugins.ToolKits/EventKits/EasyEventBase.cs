using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Plugins.ToolKits.EventKits
{
    [DebuggerDisplay("Event Count: {easyEventsContainer.Count}")]
    public abstract class EasyEventBase : IDisposable
    {
        private readonly ConcurrentDictionary<EasyEventHandle, EasyEventInvoker> easyEventsContainer =
            new ConcurrentDictionary<EasyEventHandle, EasyEventInvoker>();

        public bool HasEvents => easyEventsContainer.Count > 0;
         
        public void Dispose()
        {
            foreach (KeyValuePair<EasyEventHandle, EasyEventInvoker> item in easyEventsContainer)
            {
                item.Key.Dispose();
            }
            easyEventsContainer.Clear();
        }

        public Task DisposeAsync()
        {
            return Task.Factory.StartNew(() => Dispose(),CancellationToken.None,TaskCreationOptions.DenyChildAttach,TaskScheduler.Default);
        }
         
        internal void RunActions(Action<EasyEventInvoker> action)
        {
            if (action == null)
            {
                return;
            }

            foreach (KeyValuePair<EasyEventHandle, EasyEventInvoker> item in easyEventsContainer)
            {
                action.Invoke(item.Value);
            }
        }

        internal Task RunActionsAsync(Action<EasyEventInvoker> action)
        {
            if (action == null)
            {
                return Task.Delay(0);
            }
            return Task.Factory.StartNew(() =>
            {
                easyEventsContainer.AsParallel().ForAll(item => action?.Invoke(item.Value));

            }, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }
 

        protected EasyEventHandle InnerAdd(object action, object eventToken = null)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            EasyEventInvoker disposable = new EasyEventInvoker(action, this, eventToken);
            easyEventsContainer[disposable.EasyEventHandle] = disposable;

            return disposable.EasyEventHandle;
        }

        internal void RemoveEasyEvent(EasyEventInvoker easyEventInvoker)
        { 
            easyEventsContainer.TryRemove(easyEventInvoker.EasyEventHandle, out var _);
        }
    }
}