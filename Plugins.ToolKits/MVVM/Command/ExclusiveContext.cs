using System;

namespace Plugins.ToolKits.MVVM
{
    public interface IExclusiveContext : IDisposable
    {
        IExclusiveContext BeginExclusive();

        void EndExclusive();
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


    internal class ExclusiveContext : IExclusiveContext
    {
        private readonly Action EndExecuteCallback;
        private readonly Action BeginExecuteCallback;
        private bool ExclisiveLockRunning;
        internal ExclusiveContext(Action beginAction, Action endAction)
        {
            BeginExecuteCallback = beginAction;
            EndExecuteCallback = endAction;
        }

        public IExclusiveContext BeginExclusive()
        {
            ExclisiveLockRunning = true;

            BeginExecuteCallback.Invoke();
            return this;
        }


        public void EndExclusive()
        {
            if (ExclisiveLockRunning == false)
            {
                throw new Exception("exclusive lock execution has not yet started");
            }
            EndExecuteCallback.Invoke();
        }


        public void Dispose()
        {
            EndExecuteCallback.Invoke();
        }

    }
}