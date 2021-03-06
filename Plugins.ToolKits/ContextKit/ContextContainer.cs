﻿
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Plugins.ToolKits
{


    [DebuggerDisplay("Count:{(KeyObjects.Count+TypeObjects.Count)}")]
    public class ContextContainer : DynamicObject, IContextContainer
    {

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            KeyObjects[binder.Name] = value;
            return true;
        }
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            KeyObjects.TryGetValue(binder.Name, out result);
            return true;
        }

        public override string ToString()
        {
            return $"Key :{KeyObjects.Count}  Type :{TypeObjects.Count}";
        }


        private readonly ConcurrentDictionary<string, object> KeyObjects;
        private readonly ConcurrentDictionary<Type, object> TypeObjects;

        public ContextContainer()
        { 
            KeyObjects = new ConcurrentDictionary<string, object>();
            TypeObjects = new ConcurrentDictionary<Type, object>(); 
        }


        public virtual TInstance Get<TInstance>()
        {
            Type type = typeof(TInstance);
            if (TypeObjects.TryGetValue(type, out object insValue) && insValue is TInstance instance)
            {
                return instance;
            }
            return default;
        }

        public virtual bool TryGet<TInstance>(out TInstance instance)
        {
            Type type = typeof(TInstance);
            if (TypeObjects.TryGetValue(type, out object insValue))
            {
                instance = insValue is null ? default : (TInstance)insValue;
                return true;
            }

            instance = default;
            return false;
        }

        public virtual void Set<TInstance>([NotNull] TInstance instance)
        {
            if (instance is null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            Type type = typeof(TInstance);

            TypeObjects[type] = instance;
        }

        public virtual bool TryGet<TInstance>([NotNull] string uniqueKey, out TInstance instance)
        {
            if (uniqueKey is null)
            {
                throw new ArgumentNullException(nameof(uniqueKey));
            }

            if (KeyObjects.TryGetValue(uniqueKey, out object insValue))
            {
                instance = insValue is null ? default : (TInstance)insValue;
                return true;
            }

            instance = default;
            return false;
        }

        public virtual TInstance Get<TInstance>([NotNull] string uniqueKey)
        {
            if (uniqueKey is null)
            {
                throw new ArgumentNullException(nameof(uniqueKey));
            }

            if (KeyObjects.TryGetValue(uniqueKey, out object insValue) && insValue is TInstance instance)
            {
                return instance;
            }
            return default;
        }

        public virtual void Set<TInstance>([NotNull] string uniqueKey, [NotNull] TInstance instance)
        {
            if (uniqueKey is null)
            {
                throw new ArgumentNullException(nameof(uniqueKey));
            }

            KeyObjects[uniqueKey] = instance ?? throw new ArgumentNullException(nameof(instance));
        }

        public TInstance TryGet<TInstance>(string uniqueKey, Func<TInstance> instanceCreateFunc)
        {
            if (uniqueKey is null)
            {
                throw new ArgumentNullException(nameof(uniqueKey));
            }

            if (instanceCreateFunc is null)
            {
                throw new ArgumentNullException(nameof(instanceCreateFunc));
            }

             var instValue = (TInstance)KeyObjects.GetOrAdd(uniqueKey,i=> instanceCreateFunc.Invoke());
 
            return instValue;
        }

        public TInstance TryGet<TInstance>(Func<TInstance> instanceCreateFunc)
        {
            if (instanceCreateFunc is null)
            {
                throw new ArgumentNullException(nameof(instanceCreateFunc));
            }

            Type uniqueKey = typeof(TInstance);

            var instValue = (TInstance)TypeObjects.GetOrAdd(uniqueKey, i => instanceCreateFunc.Invoke());

            return instValue;
             
        }

        public Task<TInstance> TryGetAsync<TInstance>(string uniqueKey, Func<TInstance> instanceCreateFunc, CancellationToken token = default)
        {
            if (uniqueKey is null)
            {
                throw new ArgumentNullException(nameof(uniqueKey));
            }

            if (instanceCreateFunc is null)
            {
                throw new ArgumentNullException(nameof(instanceCreateFunc));
            }
            CancellationToken token2 = token == default ? CancellationToken.None : token;
            return Task.Factory.StartNew(() =>
            {
                var instValue = (TInstance)KeyObjects.GetOrAdd(uniqueKey, i => instanceCreateFunc.Invoke());

                return instValue;
            }, token2, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }

        public Task<TInstance> TryGetAsync<TInstance>(Func<TInstance> instanceCreateFunc, CancellationToken token = default)
        {
            if (instanceCreateFunc is null)
            {
                throw new ArgumentNullException(nameof(instanceCreateFunc));
            }
            CancellationToken token2 = token == default ? CancellationToken.None : token;
            return Task.Factory.StartNew(() =>
            {
                Type uniqueKey = typeof(TInstance); 
                var instValue = (TInstance)TypeObjects.GetOrAdd(uniqueKey, i => instanceCreateFunc.Invoke()); 
                return instValue;
            }, token2, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }

        public virtual void Dispose()
        {
            TypeObjects?.Clear();
            KeyObjects?.Clear();
        }

        internal ICollection<object> ToCollection()
        {
            var list = new List<object>();
            list.AddRange(TypeObjects.Values);
            list.AddRange(KeyObjects.Values);
            return list;
        }

        public void Clear()
        {
            TypeObjects?.Clear();
            KeyObjects?.Clear();
        }

        public string[] AllKey => KeyObjects?.Select(x => x.Key).ToArray();


        public bool RemoveKey(string key)
        {
            return KeyObjects.TryRemove(key,out var _);
        }

        public void CopyTo(ContextContainer other)
        {
            foreach (KeyValuePair<string, object> item in KeyObjects)
            {
                other.KeyObjects[item.Key] = item.Value;
            }

            foreach (KeyValuePair<Type, object> item in TypeObjects)
            {
                other.TypeObjects[item.Key] = item.Value;
            }
        }


        #region MyRegion

        private static IContextContainer _defaultInstance;

        private static readonly object syncRootLock = new object();

        public static IContextContainer Default
        {
            get
            {
                if (_defaultInstance == null)
                {
                    lock (syncRootLock)
                    {
                        if (_defaultInstance == null)
                        {
                            lock (syncRootLock)
                            {
                                if (_defaultInstance == null)
                                {
                                    _defaultInstance = new ContextContainer();
                                }
                            }
                        }
                    }
                }

                return _defaultInstance;
            }
        }

        #endregion
    }
}