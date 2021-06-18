using System;

namespace Plugins.ToolKits.MVVM
{
    public sealed class RelayCommand<TParameter> : CommandBase
    {
        private readonly Action<TParameter> ExecuteAction;
        private readonly Action<IExclusiveContext<TParameter>> ExecuteExclusiveAction;
        public RelayCommand(Action<TParameter> executeAction, Func<bool> canExecuteFunc = null, Action<Exception> catchCallback = null)
        {
            ExecuteAction = executeAction;
            CanExecuteFunc = canExecuteFunc;
            CatchCallback = catchCallback;
        }


        public RelayCommand(Action<IExclusiveContext<TParameter>> executeAction, Func<bool> canExecuteFunc = null, Action<Exception> catchCallback = null)
        {
            ExecuteExclusiveAction = executeAction;
            CanExecuteFunc = canExecuteFunc;
            CatchCallback = catchCallback;
        }


        public override void Execute(object parameter)
        {
            ExecuteActionRun(parameter);

            ExecuteExclusiveActionRun(parameter);

        }
        private void ExecuteExclusiveActionRun(object parameter)
        {
            if (ExecuteExclusiveAction is null)
            {
                return;
            }
            if (isRunning)
            {
                return;
            }

            if (parameter is not TParameter t)
            {
                return;
            }
            ExclusiveContext<TParameter> context = new ExclusiveContext<TParameter>(() => isRunning = true, () => isRunning = false)
            {
                Parameter = t
            };
            try
            {
                ExecuteExclusiveAction(context);
            }
            catch (Exception e)
            {
                if (CatchCallback is null)
                {
                    throw;
                }
                CatchCallback?.Invoke(e);
            }
        }

        private void ExecuteActionRun(object parameter)
        {
            if (ExecuteAction is null)
            {
                return;
            }
            try
            {
                if (isRunning)
                {
                    return;
                }
                isRunning = true;

                if (parameter is TParameter target)
                {
                    ExecuteAction(target);
                }
            }
            catch (Exception e)
            {
                if (CatchCallback is null)
                {
                    throw;
                }
                CatchCallback?.Invoke(e);
            }
            finally
            {
                isRunning = false;
            }
        }
    }
}