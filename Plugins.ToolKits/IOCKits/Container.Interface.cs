
using System;

namespace Plugins.ToolKits.IOCKits
{
    public interface IContainerBuilder
    {
        IContainerBuilder As<TType>() where TType : class;

        IContainerBuilder As([NotNull] Type type);

        IContainerBuilder AsSelf();
    }


    public interface IContainer : IDisposable
    {
        bool Register<TInterface, TImplementation>() where TImplementation : TInterface;

        bool Register([NotNull] Type @interfaceType, [NotNull] Type implementationType);

        IContainerBuilder Register<TType>() where TType : class;

        IContainerBuilder Register<TType>([NotNull] Func<TType> factory) where TType : class;

        IContainerBuilder Register<TType>([NotNull] TType instanceSingleTon) where TType : class;

    }


    internal interface IContainerScope : IDisposable
    {
        object GetService(Type ype);

        object GetObject(Type type, ResolveMode resolveMode = ResolveMode.Global);
    }

    internal interface ILifetime : IContainerScope
    {
    }
}