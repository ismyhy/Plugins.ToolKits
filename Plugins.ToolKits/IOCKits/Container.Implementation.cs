
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

using StrategyExpression = System.Func<Plugins.ToolKits.IOCKits.ILifetime, object>;

namespace Plugins.ToolKits.IOCKits
{
    internal abstract class ObjectCache : IDisposable
    {
        private readonly ConcurrentDictionary<Type, object>
            _instanceLifetime = new ConcurrentDictionary<Type, object>();

        public void Dispose()
        {
            foreach (object obj in _instanceLifetime.Values)
            {
                (obj as IDisposable)?.Dispose();
            }
        }

        protected object GetObject(Type type, XStrategy factory, ILifetime lifetime,
            ResolveMode resolveMode = ResolveMode.Global)
        {
            if (factory.OnlyOne)
            {
                return factory.Instance ??= factory.Strategy(lifetime);
            }


            if (resolveMode == ResolveMode.Global)
            {
                return _instanceLifetime.GetOrAdd(type, _ => factory.Strategy(lifetime));
            }

            return factory.Strategy(lifetime);
        }
    }

    internal class ContainerLifetime : ObjectCache, ILifetime
    {
        public ContainerLifetime(IDictionary<Type, XStrategy> creator)
        {
            Creator = creator;
        }

        public IDictionary<Type, XStrategy> Creator { get; }


        public object GetService(Type type)
        {
            return GetObject(type, Creator[type], this);
        }

        public object GetObject(Type type, ResolveMode resolveMode = ResolveMode.Global)
        {
            return GetObject(type, Creator[type], this, resolveMode);
        }
    }

    internal class XStrategy
    {
        public object Instance;
        public bool OnlyOne;
        public StrategyExpression Strategy;

        public override string ToString()
        {
            return $"{GetHashCode()}";
        }
    }


    internal class ContainerBuilder : IContainerBuilder
    {
        private readonly Dictionary<Type, XStrategy> _xStrategies;
        private readonly Type _baseType;
        private XStrategy _baseStrategy;

        public ContainerBuilder(Type type, Dictionary<Type, XStrategy> xStrategies)
        {
            _baseType = type;
            _xStrategies = xStrategies;
        }

        public bool IsSingleTon { private get; set; }
        public StrategyExpression StrategyExpression { get; set; }

        public IContainerBuilder As<TType>() where TType : class
        {
            return As(typeof(TType));
        }

        public IContainerBuilder As([NotNull] Type type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (_baseStrategy != null && IsSingleTon)
            {
                _xStrategies[type] = _baseStrategy;
                return this;
            }

            _xStrategies[type] = new XStrategy
            {
                OnlyOne = IsSingleTon,
                Strategy = StrategyExpression
            };


            if (IsSingleTon)
            {
                _baseStrategy = _xStrategies[type];
            }


            return this;
        }

        public IContainerBuilder AsSelf()
        {
            return As(_baseType);
        }
    }
}