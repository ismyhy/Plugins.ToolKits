using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Plugins.ToolKits.EventKits
{
    public abstract class EasyEventBase : IDisposable
    {
        private readonly IDictionary<EasyEventHandle, EasyEventInvoker> easyEventsContainer =
            new ConcurrentDictionary<EasyEventHandle, EasyEventInvoker>();

        public bool HasEvents => easyEventsContainer.Count > 0;

        public int Count => easyEventsContainer.Count;

        public void Dispose()
        {
            foreach (KeyValuePair<EasyEventHandle, EasyEventInvoker> item in easyEventsContainer)
            {
                item.Key.Dispose();
            }
        }

        public Task DisposeAsync()
        {
            return Task.Run(() => Dispose());
        }

        public override string ToString()
        {
            return $"Event Count: {Count} ";
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
            return Task.Factory.StartNew(() =>
            {
                if (action == null)
                {
                    return;
                }

                easyEventsContainer.AsParallel().ForAll(item => { action?.Invoke(item.Value); });
            }, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }

        public void Dispose(EasyEventHandle easyEventHandle)
        {
            if (easyEventHandle == null)
            {
                throw new ArgumentException("Token is Error");
            }

            easyEventHandle.Dispose();
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
            if (easyEventsContainer.ContainsKey(easyEventInvoker.EasyEventHandle))
            {
                easyEventsContainer.Remove(easyEventInvoker.EasyEventHandle);
            }
        }
    }
}