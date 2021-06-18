using System;

namespace Plugins.ToolKits.MVVM
{
    public sealed class RelayCommand<TParameter> : CommandBase
    {
        private readonly Action<TParameter> executeAction;
        private readonly Action<IExclusiveContext<TParameter>> executeExclusiveAction;
        public RelayCommand(Action<TParameter> executeAction, Func<bool> canExecuteFunc = null, Action<Exception> catchCallback = null)
        {
            this.executeAction = executeAction;
            base.canExecuteFunc = canExecuteFunc;
            base.catchCallback = catchCallback;
        }


        public RelayCommand(Action<IExclusiveContext<TParameter>> executeAction, Func<bool> canExecuteFunc = null, Action<Exception> catchCallback = null)

        {
            executeExclusiveAction = executeAction;
            base.canExecuteFunc = canExecuteFunc;
            base.catchCallback = catchCallback;

        }


        public override void Execute(object parameter)
        {
            ExecuteActionRun(parameter);

            ExecuteExclusiveActionRun(parameter);

        }


        private void ExecuteExclusiveActionRun(object parameter)
        {
            if (executeExclusiveAction is null)
            {
                return;
            }
            if (IsCommandExecuting)
            {
                return;
            }

            if (parameter is not TParameter t)
            {
                return;
            }

            ExclusiveContext<TParameter> context = new ExclusiveContext<TParameter>(() => IsCommandExecuting = true, () => IsCommandExecuting = false)
            {
                Parameter = t
            };

            try
            {
                executeExclusiveAction(context);
            }
            catch (Exception e)
            {
                if (catchCallback is null)
                {
                    throw;
                }
                catchCallback?.Invoke(e);
            }
        }

        private void ExecuteActionRun(object parameter)
        {
            if (executeAction is null)
            {
                return;
            }
            try
            {
                if (IsCommandExecuting)
                {
                    return;
                }
                IsCommandExecuting = true;

                if (parameter is TParameter target)
                {
                    executeAction(target);
                }
            }
            catch (Exception e)
            {
                if (catchCallback is null)
                {
                    throw;
                }
                catchCallback?.Invoke(e);
            }
            finally
            {
                IsCommandExecuting = false;
            }
        }
    }
}