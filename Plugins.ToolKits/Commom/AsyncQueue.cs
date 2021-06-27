﻿using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Plugins.ToolKits
{



    public sealed class AsyncQueue<T>
    {
        private readonly SemaphoreSlim semaphore;
        private readonly ConcurrentQueue<T> queue;

        /// <summary>
        /// Queue count
        /// </summary>
        public int Count => queue.Count;

        /// <summary>
        /// Constructor
        /// </summary>
        public AsyncQueue()
        {
            semaphore = new SemaphoreSlim(0);
            queue = new ConcurrentQueue<T>();
        }

        /// <summary>
        /// Enqueue item
        /// </summary>
        /// <param name="item"></param>
        public void Enqueue(T item)
        {
            queue.Enqueue(item);
            semaphore.Release();
        }

        /// <summary>
        /// Async dequeue
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<T> DequeueAsync(CancellationToken cancellationToken = default)
        {
            for (; ; )
            {
                await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

                if (queue.TryDequeue(out T item))
                {
                    return item;
                }
            }
        }

        /// <summary>
        /// Wait for queue to have at least one entry
        /// </summary>
        /// <returns></returns>
        public void WaitForEntry() => semaphore.Wait();

        /// <summary>
        /// Wait for queue to have at least one entry
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task WaitForEntryAsync(CancellationToken token = default)
        {
            await semaphore.WaitAsync(token).ConfigureAwait(false);
        }

        /// <summary>
        /// Try dequeue (if item exists)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool TryDequeue(out T item)
        {
            return queue.TryDequeue(out item);
        }
    }

}