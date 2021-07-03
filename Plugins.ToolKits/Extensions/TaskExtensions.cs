
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Plugins.ToolKits
{ 
    public static class TaskExtensions
    {
        public static void NoAwaiter([NotNull] this Task task)
        {
        }

        public static void NoAwaiter<TType>([NotNull] this Task<TType> task)
        {
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


        public static Task<bool> TryRemoveAsync<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            return Task.Factory.StartNew(() =>
            {
                return dictionary.TryRemove(key);
            }, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }

    }
}