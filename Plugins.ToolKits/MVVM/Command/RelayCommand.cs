using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Plugins.ToolKits.MVVM
{
    public sealed class RelayCommand : CommandBase
    {
        private readonly ICollection<Action> executeActions = new List<Action>();
        private readonly Action<IExclusiveContext> executeExclusiveAction;
        public RelayCommand(Action executeAction, Func<bool> canExecuteFunc = null, Action<Exception> catchCallback = null)
        {
            executeActions.Add(executeAction);
            base.canExecuteFunc = canExecuteFunc;
            base.catchCallback = catchCallback;
        }


        public RelayCommand(Action<IExclusiveContext> executeAction, Func<bool> canExecuteFunc = null, Action<Exception> catchCallback = null)
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
            ExclusiveContext context = new ExclusiveContext(() => IsCommandExecuting = true, () => IsCommandExecuting = false);

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

        public RelayCommand AddExecuteAction(Action executeAction)
        {
            if(executeAction == null)
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
                executeActions.ForEach(i=>i());
            }
            catch (Exception e)
            {
                if (catchCallback is null)
                {
                    throw;
                }
                catchCallback.Invoke(e);
            }
            finally
            {
                IsCommandExecuting = false;
            }
        }

        #region     new 

        public static RelayCommand Execute(object commandOwner, Action commandAction, Func<bool> canExecuteAction = null, Action<Exception> exceptionCallback = null, [CallerMemberName] string propertyName = null)
        {
            RelayCommand command = CommandBinder(commandOwner, propertyName, () => new RelayCommand(commandAction, canExecuteAction, exceptionCallback));
            return command;
        }
        public static RelayCommand<T> Execute<T>(object commandOwner, Action<T> commandAction, Func<bool> canExecuteAction = null, Action<Exception> exceptionCallback = null, [CallerMemberName] string propertyName = null)
        {
            RelayCommand<T> command = CommandBinder(commandOwner, propertyName, () => new RelayCommand<T>(commandAction, canExecuteAction, exceptionCallback));
            return command;
        }
        public static RelayCommand ExclusiveExecute(object commandOwner, Action<IExclusiveContext> commandAction, Func<bool> canExecuteAction = null, Action<Exception> exceptionCallback = null, [CallerMemberName] string propertyName = null)
        {
            RelayCommand command = CommandBinder(commandOwner, propertyName, () => new RelayCommand(commandAction, canExecuteAction, exceptionCallback));
            return command;
        }
        public static RelayCommand<T> ExclusiveExecute<T>(object commandOwner, Action<IExclusiveContext<T>> commandAction, Func<bool> canExecuteAction = null, Action<Exception> exceptionCallback = null, [CallerMemberName] string propertyName = null)
        {
            RelayCommand<T> command = CommandBinder(commandOwner, propertyName, () => new RelayCommand<T>(commandAction, canExecuteAction, exceptionCallback));
            return command;
        }

        public static RelayCommand TryExecute(object commandOwner, Action commandAction, Action<Exception> exceptionCallback, Func<bool> canExecuteAction = null, [CallerMemberName] string propertyName = null)
        {
            RelayCommand command = CommandBinder(commandOwner, propertyName, () => new RelayCommand(commandAction, canExecuteAction, exceptionCallback));
            return command;
        }
        public static RelayCommand<T> TryExecute<T>(object commandOwner, Action<T> commandAction, Action<Exception> exceptionCallback, Func<bool> canExecuteAction = null, [CallerMemberName] string propertyName = null)
        {
            RelayCommand<T> command = CommandBinder(commandOwner, propertyName, () => new RelayCommand<T>(commandAction, canExecuteAction, exceptionCallback));
            return command;
        }
        public static RelayCommand TryExclusiveExecute(object commandOwner, Action<IExclusiveContext> commandAction, Action<Exception> exceptionCallback, [CallerMemberName] string propertyName = null)
        {
            RelayCommand command = CommandBinder(commandOwner, propertyName, () => new RelayCommand(commandAction, null, exceptionCallback));
            return command;
        }
        public static RelayCommand<T> TryExclusiveExecute<T>(object commandOwner, Action<IExclusiveContext<T>> commandAction, Action<Exception> exceptionCallback, [CallerMemberName] string propertyName = null)
        {
            RelayCommand<T> command = CommandBinder(commandOwner, propertyName, () => new RelayCommand<T>(commandAction, null, exceptionCallback));

            return command;
        }


        private static TCommand CommandBinder<TCommand>(object commandOwner, string propertyName, Func<TCommand> commandFunc) where TCommand : ICommand
        {
            if (commandOwner is null)
            {
                throw new ArgumentNullException(nameof(commandOwner));
            }

            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            if (commandFunc is null)
            {
                throw new ArgumentNullException(nameof(commandFunc));
            }
            ConcurrentDictionary<string, ICommand> commandDict = keyValuePairs.GetOrAdd(commandOwner, i => new ConcurrentDictionary<string, ICommand>());

            ICommand command = commandDict.GetOrAdd(propertyName, i => commandFunc());

            return (TCommand)command;
        }

        private static readonly ConcurrentDictionary<object, ConcurrentDictionary<string, ICommand>> keyValuePairs = new ConcurrentDictionary<object, ConcurrentDictionary<string, ICommand>>();

        #endregion






        public static RelayCommand Execute(Action commandAction, Func<bool> canExecuteAction = null, Action<Exception> exceptionCallback = null)
        {
            return new RelayCommand(commandAction, canExecuteAction, exceptionCallback);
        }
        public static RelayCommand<T> Execute<T>(Action<T> commandAction, Func<bool> canExecuteAction = null, Action<Exception> exceptionCallback = null)
        {
            return new RelayCommand<T>(commandAction, canExecuteAction, exceptionCallback);
        }
        public static RelayCommand ExclusiveExecute(Action<IExclusiveContext> commandAction, Func<bool> canExecuteAction = null, Action<Exception> exceptionCallback = null)
        {
            return new RelayCommand(commandAction, canExecuteAction, exceptionCallback);
        }
        public static RelayCommand<T> ExclusiveExecute<T>(Action<IExclusiveContext<T>> commandAction, Func<bool> canExecuteAction = null, Action<Exception> exceptionCallback = null)
        {
            return new RelayCommand<T>(commandAction, canExecuteAction, exceptionCallback);
        }

        public static RelayCommand TryExecute(Action commandAction, Action<Exception> exceptionCallback, Func<bool> canExecuteAction = null)
        {
            return new RelayCommand(commandAction, canExecuteAction, exceptionCallback);
        }
        public static RelayCommand<T> TryExecute<T>(Action<T> commandAction, Action<Exception> exceptionCallback, Func<bool> canExecuteAction = null)
        {
            return new RelayCommand<T>(commandAction, canExecuteAction, exceptionCallback);
        }
        public static RelayCommand TryExclusiveExecute(Action<IExclusiveContext> commandAction, Action<Exception> exceptionCallback)
        {
            return new RelayCommand(commandAction, null, exceptionCallback);
        }
        public static RelayCommand<T> TryExclusiveExecute<T>(Action<IExclusiveContext<T>> commandAction, Action<Exception> exceptionCallback)
        {
            return new RelayCommand<T>(commandAction, null, exceptionCallback);
        }

    }
}