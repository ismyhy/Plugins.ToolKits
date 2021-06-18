using Plugins.ToolKits.Attributes;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Plugins.ToolKits.Extensions
{
    public class TaskWaiter
    {
    }


    public static class TaskExtensions
    {
        public static void NoAwaiter([NotNull] this Task task)
        {
        }

        public static void NoAwaiter<TType>([NotNull] this Task<TType> task)
        {
        }

        public static Task LongRun([NotNull] Action action)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            return Task.Factory.StartNew(() =>
                {
                    Thread.CurrentThread.IsBackground = true;
                    action?.Invoke();
                }, CancellationToken.None, TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
        }

        public static Task LongRun<TResult>([NotNull] Func<TResult> action)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            return Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
        }

        public static Task InvokeAsync([NotNull] this Action action,
            TaskCreationOptions creationOptions = TaskCreationOptions.DenyChildAttach)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            return Task.Factory.StartNew(action, CancellationToken.None, creationOptions,
                TaskScheduler.Default);
        }


        public static Task<TResult> InvokeAsync<TResult>([NotNull] this Func<TResult> action,
            TaskCreationOptions creationOptions = TaskCreationOptions.DenyChildAttach)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            return Task.Factory.StartNew(action, CancellationToken.None, creationOptions,
                TaskScheduler.Default);
        }

        public static Task InvokeAsync([NotNull] this Action action, TimeSpan delayTimeSpan,
            TaskCreationOptions creationOptions = TaskCreationOptions.DenyChildAttach)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            return Task.Factory.StartNew(() =>
                {
                    Task.Delay(delayTimeSpan).GetAwaiter().GetResult();
                    action();
                }, CancellationToken.None, creationOptions,
                TaskScheduler.Default);
        }


        public static Task<TResult> InvokeAsync<TResult>([NotNull] this Func<TResult> action, TimeSpan delayTimeSpan,
            TaskCreationOptions creationOptions = TaskCreationOptions.DenyChildAttach)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            return Task.Factory.StartNew(() =>
                {
                    Task.Delay(delayTimeSpan).GetAwaiter().GetResult();
                    return action();
                }, CancellationToken.None, creationOptions,
                TaskScheduler.Default);
        }


        public static Task ForEachAsync<TType>([NotNull] this IEnumerable<TType> collection,
            [NotNull] Action<TType> action)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (collection is null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            if (collection.Any() == false)
            {
                return Task.Delay(0);
            }

            return Task.Factory.StartNew(() =>
                {
                    foreach (TType item in collection)
                    {
                        action(item);
                    }
                }, CancellationToken.None,
                TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }


        public static Task AddItemsAsync<T>([NotNull] this ICollection<T> origin, [NotNull] IEnumerable<T> target)
        {
            if (origin is null)
            {
                throw new ArgumentNullException(nameof(origin));
            }

            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (target.Any() == false)
            {
                return Task.Delay(0);
            }

            return Task.Factory.StartNew(() =>
            {
                foreach (T item in target)
                {
                    origin.Add(item);
                }
            }, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }
    }
}