using System;
using System.Collections.Generic;

namespace Plugins.ToolKits.MVVM
{
    public sealed class RelayCommand<TParameter> : CommandBase
    {
        private readonly ICollection<Action<TParameter>> executeActions = new List<Action<TParameter>>();

        private readonly Action<IExclusiveContext<TParameter>> executeExclusiveAction;
        public RelayCommand(Action<TParameter> executeAction, Func<bool> canExecuteFunc = null, Action<Exception> catchCallback = null)
        {
            executeActions.Add(executeAction);
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

        public RelayCommand<TParameter> AddExecuteAction(Action<TParameter> executeAction)
        {
            if (executeAction == null)
            {
                throw new ArgumentNullException(nameof(executeAction));
            }
            executeActions.Add(executeAction);
            return this;
        }



        private void ExecuteActionRun(object parameter)
        {
            if (executeActions is null || executeActions.Count==0)
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
                    executeActions.ForEach(i=>i(target));
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