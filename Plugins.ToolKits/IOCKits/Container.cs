
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using StrategyExpression = System.Func<Plugins.ToolKits.IOCKits.ILifetime, object>;

namespace Plugins.ToolKits.IOCKits
{
    /// <summary>
    ///     Inversion of control container handles dependency injection for registered types
    /// </summary>
    public class Container : IContainerScope, IContainer
    {
        private readonly ContainerLifetime _lifetime;

        private readonly Dictionary<Type, XStrategy> _registeredTypes = new Dictionary<Type, XStrategy>();


        public Container()
        {
            _lifetime = new ContainerLifetime(_registeredTypes);
        }


        public object GetService(Type type)
        {
            return _lifetime.GetObject(type);
        }

        public void Dispose()
        {
            _lifetime.Dispose();
        }

        public object GetObject([NotNull] Type type, ResolveMode resolveMode = ResolveMode.Global)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (!_registeredTypes.ContainsKey(type))
            {
                throw new ArgumentException("Type not registered");
            }

            return _lifetime.GetObject(type, resolveMode);
        }


        private bool RegisterType(Type itemType, StrategyExpression factory, bool onlyOne = false)
        {
            if (_registeredTypes.TryGetValue(itemType, out _))
            {
                return false;
            }

            _registeredTypes[itemType] = new XStrategy
            {
                Strategy = factory,
                OnlyOne = onlyOne
            };

            return true;
        }


        #region Container

        internal static StrategyExpression FactoryFromType(Type itemType)
        {
            ConstructorInfo[] constructors = itemType.GetConstructors();
            if (constructors.Length == 0)
            {
                constructors = itemType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
            }

            ConstructorInfo constructor = constructors.First();

            ParameterExpression arg = Expression.Parameter(typeof(ILifetime));
            return (StrategyExpression)Expression.Lambda(
                Expression.New(constructor, constructor.GetParameters().Select(
                    param =>
                    {
                        StrategyExpression resolve = new StrategyExpression(lifetime => lifetime.GetService(param.ParameterType));
                        return Expression.Convert(
                            Expression.Call(Expression.Constant(resolve.Target), resolve.Method, arg),
                            param.ParameterType);
                    })),
                arg).Compile();
        }

        #endregion


        #region Register

        public bool Register<TInterface, TImplementation>() where TImplementation : TInterface
        {
            return Register(typeof(TInterface), typeof(TImplementation));
        }

        public bool Register([NotNull] Type @interface, [NotNull] Type implementation)
        {
            if (@interface is null)
            {
                throw new ArgumentNullException(nameof(@interface));
            }

            if (implementation is null)
            {
                throw new ArgumentNullException(nameof(implementation));
            }

            return RegisterType(@interface, FactoryFromType(implementation));
        }


        IContainerBuilder IContainer.Register<TType>() where TType : class
        {
            ContainerBuilder builder = new ContainerBuilder(typeof(TType), _registeredTypes)
            {
                IsSingleTon = false,
                StrategyExpression = FactoryFromType(typeof(TType)),
            };

            return builder;
        }

        IContainerBuilder IContainer.Register<TType>([NotNull] Func<TType> factory) where TType : class
        {
            if (factory is null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            ContainerBuilder builder = new ContainerBuilder(factory.Method.ReturnType, _registeredTypes)
            {
                IsSingleTon = false,
                StrategyExpression = i => factory.Invoke(),
            };

            return builder;
        }

        IContainerBuilder IContainer.Register<TType>([NotNull] TType instanceSingleTon) where TType : class
        {
            if (instanceSingleTon is null)
            {
                throw new ArgumentNullException(nameof(instanceSingleTon));
            }

            ContainerBuilder builder = new ContainerBuilder(instanceSingleTon.GetType(), _registeredTypes)
            {
                IsSingleTon = true,
                StrategyExpression = i => instanceSingleTon,
            };
            return builder;
        }



        #endregion
    }
}