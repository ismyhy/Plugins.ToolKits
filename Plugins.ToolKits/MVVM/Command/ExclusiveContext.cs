using System;

namespace Plugins.ToolKits.MVVM
{

    public interface IEndExclusiveContext : IDisposable
    {
        void EndExclusive();
    }


    public interface IExclusiveContext : IEndExclusiveContext
    {
        IEndExclusiveContext BeginExclusive();
    }
    public interface IExclusiveContext<TParameter> : IExclusiveContext
    {
        TParameter Parameter { get; }
    }


    internal class ExclusiveContext<TContext> : ExclusiveContext, IExclusiveContext<TContext>
    {
        internal ExclusiveContext(Action beginAction, Action endAction) : base(beginAction, endAction)
        {
        }

        public TContext Parameter { get; internal set; }

    }


    internal class ExclusiveContext : IExclusiveContext, IEndExclusiveContext
    {
        private readonly Action EndExecuteCallback;
        private readonly Action BeginExecuteCallback;
        internal ExclusiveContext(Action beginAction, Action endAction)
        {
            BeginExecuteCallback = beginAction;
            EndExecuteCallback = endAction;
        }

        public IEndExclusiveContext BeginExclusive()
        {
            BeginExecuteCallback.Invoke();
            return this;
        }


        public void EndExclusive()
        {
            EndExecuteCallback.Invoke();
        }


        public void Dispose()
        {
            EndExclusive();
        }

    }
}