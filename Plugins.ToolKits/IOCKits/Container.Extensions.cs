using Plugins.ToolKits.Attributes;

using System;

namespace Plugins.ToolKits.IOCKits
{
    /// <summary>
    ///     Extension methods for Container
    /// </summary>
    public static class ContainerExtensions
    {
        /// <summary>
        ///     Registers an implementation type for the specified interface
        /// </summary>
        /// <typeparam name="T">Interface to register</typeparam>
        /// <param name="container">This container instance</param>
        /// <param name="type">Implementing type</param>
        /// <returns>IRegisteredType object</returns>
        public static bool Register<T>(this Container container, Type type)
        {
            return ((IContainer)container).Register(typeof(T), type);
        }

        /// <summary>
        ///     Registers an implementation type for the specified interface
        /// </summary>
        /// <typeparam name="TInterface">Interface to register</typeparam>
        /// <typeparam name="TImplementation">Implementing type</typeparam>
        /// <param name="container">This container instance</param>
        /// <returns>IRegisteredType object</returns>
        public static bool Register<TInterface, TImplementation>(this Container container)
            where TImplementation : TInterface
        {
            return ((IContainer)container).Register(typeof(TInterface), typeof(TImplementation));
        }


        /// <summary>
        ///     Registers an implementation type for the specified interface
        /// </summary>
        /// <param name="container">This container instance</param>
        /// <param name="interfaceType"></param>
        /// <param name="implementationType"></param>
        /// <returns>IRegisteredType object</returns>
        public static bool Register(this Container container, Type interfaceType, Type implementationType)
        {
            return ((IContainer)container).Register(interfaceType, implementationType);
        }

        /// <summary>
        ///     Registers a factory function which will be called to resolve the specified interface
        /// </summary>
        /// <typeparam name="T">Interface to register</typeparam>
        /// <param name="container">This container instance</param>
        /// <param name="factory">Factory method</param>
        /// <returns>IRegisteredType object</returns>
        public static IContainerBuilder Register<T>(this Container container, Func<T> factory) where T : class
        {
            return ((IContainer)container).Register(factory);
        }

        /// <summary>
        ///     Registers a factory function which will be called to resolve the specified interface
        /// </summary>
        /// <param name="container">This container instance</param>
        /// <param name="instanceSingleTon">instance</param>
        /// <returns>IRegisteredType object</returns>
        public static IContainerBuilder Register<TType>(this Container container, [NotNull] TType instanceSingleTon) where TType : class
        {
            return ((IContainer)container).Register(instanceSingleTon);
        }

        /// <summary>
        ///     Registers a type
        /// </summary>
        /// <param name="container">This container instance</param>
        /// <typeparam name="T">Type to register</typeparam>
        /// <returns>IRegisteredType object</returns>
        public static bool Register<T>(this Container container)
        {
            return ((IContainer)container).Register(typeof(T), typeof(T));
        }

        /// <summary>
        ///     Returns an implementation of the specified interface
        /// </summary>
        /// <typeparam name="T">Interface type</typeparam>
        /// <param name="container">This container instance</param>
        /// <param name="resolveMode"></param>
        /// <returns>Object implementing the interface</returns>
        public static T Resolve<T>(this Container container, ResolveMode resolveMode = ResolveMode.Global)
        {
            return (T)container.GetObject(typeof(T), resolveMode);
        }

        /// <summary>
        ///     Returns an implementation of the specified interface
        /// </summary>
        /// <param name="container">This container instance</param>
        /// <param name="resolveMode"></param>
        /// <returns>Object implementing the interface</returns>
        public static object Resolve(this Container container, Type targetType,
            ResolveMode resolveMode = ResolveMode.Global)
        {
            return container.GetObject(targetType, resolveMode);
        }
    }
}