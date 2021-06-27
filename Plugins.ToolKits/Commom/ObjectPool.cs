using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Plugins.ToolKits
{
    public interface IResettable
    {
        void Reset();
    }

    public static class ObjectPool
    {
        public static ObjectPool<Target> Share<Target>(int maxCount, Func<Target> createFunc) where Target : class, IResettable
        {
            if (createFunc == null)
            {
                throw new ArgumentNullException(nameof(createFunc));
            }

            return new ObjectPool<Target>(maxCount, createFunc);
        }

        public static ObjectPool<Target> Share<Target>(int maxCount) where Target : class, IResettable, new()
        {
            return new ObjectPool<Target>(maxCount, () => new Target());
        }

    }
    public class ObjectPool<Target> : IDisposable where Target : IResettable
    {
        private readonly int size;
        private readonly Func<Target> creator;
        private readonly SemaphoreSlim handleAvailable;
        private readonly ConcurrentQueue<Target> itemQueue;
        private bool disposed = false;
        private int totalAllocated = 0;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="size"></param>
        /// <param name="creator"></param>
        public ObjectPool(int size, Func<Target> creator)
        {
            this.size = size;
            this.creator = creator;
            handleAvailable = new SemaphoreSlim(0);
            itemQueue = new ConcurrentQueue<Target>();
        }

        /// <summary>
        /// Get item synchronously
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public Target Rent(CancellationToken token = default)
        {
            for (; ; )
            {
                if (disposed)
                {
                    throw new Exception("Getting handle in disposed device");
                }

                if (GetOrAdd(itemQueue, out Target item))
                {
                    return item;
                }

                handleAvailable.Wait(token);
            }
        }

        /// <summary>
        /// Get item asynchronously
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<Target> RentAsync(CancellationToken token = default)
        {
            for (; ; )
            {
                if (disposed)
                {
                    throw new Exception("Getting handle in disposed device");
                }

                if (GetOrAdd(itemQueue, out Target item))
                {
                    return item;
                }

                await handleAvailable.WaitAsync(token).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Try get item (fast path)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool TryRent(out Target item)
        {
            if (disposed)
            {
                item = default;
                return false;
            }
            return GetOrAdd(itemQueue, out item);
        }

        /// <summary>
        /// Return item to pool
        /// </summary>
        /// <param name="item"></param>
        public void Return(Target item, bool needReset = true)
        {
            if (needReset)
            {
                item.Reset();
            }
            itemQueue.Enqueue(item);
            if (handleAvailable.CurrentCount < itemQueue.Count)
            {
                handleAvailable.Release();
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            disposed = true;

            while (totalAllocated > 0)
            {
                while (itemQueue.TryDequeue(out Target item))
                {
                    if (item is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                    Interlocked.Decrement(ref totalAllocated);
                }
                if (totalAllocated > 0)
                {
                    handleAvailable.Wait();
                }
            }
        }

        /// <summary>
        /// Get item from queue, adding up to pool-size items if necessary
        /// </summary>
        private bool GetOrAdd(ConcurrentQueue<Target> itemQueue, out Target item)
        {
            if (itemQueue.TryDequeue(out item))
            {
                return true;
            }

            int _totalAllocated = totalAllocated;
            while (_totalAllocated < size)
            {
                if (Interlocked.CompareExchange(ref totalAllocated, _totalAllocated + 1, _totalAllocated) == _totalAllocated)
                {
                    item = creator();
                    return true;
                }
                if (itemQueue.TryDequeue(out item))
                {
                    return true;
                }

                _totalAllocated = totalAllocated;
            }
            return false;
        }
    }

}