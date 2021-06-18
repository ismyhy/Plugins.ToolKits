﻿using System;
using System.Collections.Concurrent;
using System.Threading;

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

    internal static class ObjectPoolKeys
    {
        public const string ConcurrentDictionary = "ConcurrentDictionary";
        public const string ConcurrentQueue = "ConcurrentQueue";
        public const string Semaphore = "Semaphore";
        public const string AutoResetEvent = "AutoResetEvent";
        public const string CreateFunc = "CreateFunc";
    }


    public class ObjectPool<TType> where TType : class, IResettable
    {
        private readonly ContextContainer container = new ContextContainer();

        internal ObjectPool(int maxCount, Func<TType> createFunc)
        {
            container.Set(ObjectPoolKeys.CreateFunc, createFunc);
            container.Set(ObjectPoolKeys.ConcurrentDictionary, new ConcurrentDictionary<int, TType>());
            container.Set(ObjectPoolKeys.ConcurrentQueue, new ConcurrentQueue<TType>());
            container.Set(ObjectPoolKeys.Semaphore, new Semaphore(1, 1));
            container.Set(ObjectPoolKeys.AutoResetEvent, new AutoResetEvent(false));

            MaxCount = maxCount;
        }
        public int MaxCount { get; private set; }

        public TType Rent(int millisecondsTimeout = -1)
        {

            Func<TType> createFunc = container.Get<Func<TType>>(ObjectPoolKeys.CreateFunc);
            ConcurrentDictionary<int, TType> dict = container.Get<ConcurrentDictionary<int, TType>>(ObjectPoolKeys.ConcurrentDictionary);
            ConcurrentQueue<TType> queue = container.Get<ConcurrentQueue<TType>>(ObjectPoolKeys.ConcurrentQueue);
            Semaphore Semaphore = container.Get<Semaphore>(ObjectPoolKeys.Semaphore);
            AutoResetEvent autoReset = container.Get<AutoResetEvent>(ObjectPoolKeys.AutoResetEvent);

            try
            {
                if (!Semaphore.WaitOne(millisecondsTimeout))
                {
                    throw new TimeoutException();
                }

                do
                {
                    if (queue.TryDequeue(out TType value))
                    {
                        return value;
                    }

                    int totalCount = queue.Count + dict.Count;
                    if (totalCount < MaxCount)
                    {
                        value = createFunc();
                        dict[value.GetHashCode()] = value;
                        return value;
                    }
                    if (!autoReset.WaitOne(millisecondsTimeout))
                    {
                        throw new TimeoutException();
                    }
                } while (true);
            }
            finally
            {
                Semaphore.Release(1);
            }
        }


        public void Return(TType target, bool needReset = true)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }
            if (needReset)
            {
                target.Reset();
            }
            ConcurrentDictionary<int, TType> dict = container.Get<ConcurrentDictionary<int, TType>>(ObjectPoolKeys.ConcurrentDictionary);
            ConcurrentQueue<TType> queue = container.Get<ConcurrentQueue<TType>>(ObjectPoolKeys.ConcurrentQueue);
            AutoResetEvent autoReset = container.Get<AutoResetEvent>(ObjectPoolKeys.AutoResetEvent);
            queue.Enqueue(target);
            dict.TryRemove(target.GetHashCode(), out TType _);
            autoReset.Set();
        }

    }
}
