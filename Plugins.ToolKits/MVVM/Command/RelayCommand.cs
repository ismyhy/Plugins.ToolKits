using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Plugins.ToolKits.MVVM
{
    public sealed class RelayCommand : CommandBase
    {
        private readonly Action ExecuteAction;
        private readonly Action<ExclusiveContext> ExecuteExclusiveAction;
        public RelayCommand(Action executeAction, Func<bool> canExecuteFunc = null, Action<Exception> catchCallback = null)
        {
            ExecuteAction = executeAction;
            CanExecuteFunc = canExecuteFunc;
            CatchCallback = catchCallback;
        }


        public RelayCommand(Action<IExclusiveContext> executeAction, Func<bool> canExecuteFunc = null, Action<Exception> catchCallback = null)
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
            ExclusiveContext context = new ExclusiveContext(() => isRunning = true, () => isRunning = false);

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
                ExecuteAction();
            }
            catch (Exception e)
            {
                if (CatchCallback is null)
                {
                    throw;
                }
                CatchCallback.Invoke(e);
            }
            finally
            {
                isRunning = false;
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