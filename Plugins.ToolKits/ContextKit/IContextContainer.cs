﻿
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Plugins.ToolKits
{
    public interface IContextContainer : IDisposable
    {
    
        void Clear();

        bool RemoveKey(string key);

        string[] AllKey { get; }

        TInstance Get<TInstance>();

        bool TryGet<TInstance>(out TInstance instance);

        void Set<TInstance>(TInstance instance);

        bool TryGet<TInstance>(string uniqueKey, out TInstance instance);

        TInstance Get<TInstance>(string uniqueKey);

        void Set<TInstance>(string uniqueKey, TInstance instance);

        TInstance TryGet<TInstance>(string uniqueKey, [NotNull] Func<TInstance> instanceCreateFunc);

        TInstance TryGet<TInstance>([NotNull] Func<TInstance> instanceCreateFunc);

        Task<TInstance> TryGetAsync<TInstance>(string uniqueKey, [NotNull] Func<TInstance> instanceCreateFunc, CancellationToken token = default);

        Task<TInstance> TryGetAsync<TInstance>([NotNull] Func<TInstance> instanceCreateFunc, CancellationToken token = default);
    }
}