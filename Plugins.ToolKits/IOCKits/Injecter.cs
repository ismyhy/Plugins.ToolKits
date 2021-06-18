
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Plugins.ToolKits.IOCKits
{
    public static class Injecter
    {
        private static readonly Container Container = new Container();

        public static bool Register<T>() where T : class
        {
            return ((IContainer)Container).Register(typeof(T), typeof(T));
        }


        public static bool Register<TType, TTypeImpl>() where TType : class where TTypeImpl : class, TType
        {
            return ((IContainer)Container).Register(typeof(TType), typeof(TTypeImpl));
        }


        public static IContainerBuilder Register<TType>([NotNull] Func<TType> func) where TType : class
        {
            if (func is null)
            {
                throw new ArgumentNullException(nameof(func));
            }

            return ((IContainer)Container).Register(func);
        }

        public static IContainerBuilder RegisterInstance<TType>([NotNull] TType instance) where TType : class
        {
            if (instance is null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            return ((IContainer)Container).Register(instance);
        }

        public static TType Resolve<TType>(ResolveMode resolveMode = ResolveMode.Global) where TType : class
        {
            return (TType)Container.GetObject(typeof(TType), resolveMode);
        }

        public static async Task<TType> ResolveAsync<TType>(ResolveMode resolveMode = ResolveMode.Global)
            where TType : class
        {
            return await Task.Factory.StartNew(() => (TType)Container.GetObject(typeof(TType), resolveMode),
                CancellationToken.None,
                TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }


        public static object Resolve([NotNull] Type targetType, ResolveMode resolveMode = ResolveMode.Global)
        {
            if (targetType is null)
            {
                throw new ArgumentNullException(nameof(targetType));
            }
            return Container.GetObject(targetType, resolveMode);
        }

        public static async Task<object> ResolveAsync([NotNull] Type targetType, ResolveMode resolveMode = ResolveMode.Global)
        {
            if (targetType is null)
            {
                throw new ArgumentNullException(nameof(targetType));
            }
            return await Task.Factory.StartNew(() => Container.GetObject(targetType, resolveMode),
                CancellationToken.None,
                TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }
    }
}