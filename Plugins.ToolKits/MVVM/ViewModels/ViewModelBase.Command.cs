using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Plugins.ToolKits.MVVM
{
    public abstract partial class ViewModelBase
    {
        #region  CommandExtensions

        public RelayCommand ExecuteCommand(Action commandAction, Func<bool> canExecuteAction = null, Action<Exception> exceptionCallback = null, [CallerMemberName] string propertyName = null)
        {
            if (commandAction is null)
            {
                throw new ArgumentNullException(nameof(commandAction));
            }
            RelayCommand command = CommandBinder(() => new RelayCommand(commandAction, canExecuteAction, exceptionCallback), propertyName);

            return command;

        }

        public RelayCommand<TParameter> ExecuteCommand<TParameter>(Action<TParameter> commandAction, Func<bool> canExecuteAction = null, Action<Exception> exceptionCallback = null, [CallerMemberName] string propertyName = null)
        {
            if (commandAction is null)
            {
                throw new ArgumentNullException(nameof(commandAction));
            }
            RelayCommand<TParameter> command = CommandBinder(() => new RelayCommand<TParameter>(commandAction, canExecuteAction, exceptionCallback), propertyName);

            return command;

        }

        public RelayCommand ExecuteExclusiveCommand(Action<IExclusiveContext> commandAction, Func<bool> canExecuteAction = null, Action<Exception> exceptionCallback = null, [CallerMemberName] string propertyName = null)
        {
            if (commandAction is null)
            {
                throw new ArgumentNullException(nameof(commandAction));
            }
            RelayCommand command = CommandBinder(() => new RelayCommand(commandAction, canExecuteAction, exceptionCallback), propertyName);

            return command;
        }

        public RelayCommand<TParameter> ExecuteExclusiveCommand<TParameter>(Action<IExclusiveContext<TParameter>> commandAction, Func<bool> canExecuteAction = null, Action<Exception> exceptionCallback = null, [CallerMemberName] string propertyName = null)
        {
            if (commandAction is null)
            {
                throw new ArgumentNullException(nameof(commandAction));
            }

            RelayCommand<TParameter> command = CommandBinder(() => new RelayCommand<TParameter>(commandAction, canExecuteAction, exceptionCallback), propertyName);

            return command;
        }


        public RelayCommand TryExecuteCommand(Action commandAction, Func<bool> canExecuteAction, Action<Exception> exceptionCallback, [CallerMemberName] string propertyName = null)
        {
            if (commandAction is null)
            {
                throw new ArgumentNullException(nameof(commandAction));
            }

            if (exceptionCallback is null)
            {
                throw new ArgumentNullException(nameof(exceptionCallback));
            }
            RelayCommand command = CommandBinder(() => new RelayCommand(commandAction, canExecuteAction, exceptionCallback), propertyName);

            return command;
        }

        public RelayCommand<TParameter> TryExecuteCommand<TParameter>(Action<TParameter> commandAction, Func<bool> canExecuteAction, Action<Exception> exceptionCallback, [CallerMemberName] string propertyName = null)
        {
            if (commandAction is null)
            {
                throw new ArgumentNullException(nameof(commandAction));
            }
            if (exceptionCallback is null)
            {
                throw new ArgumentNullException(nameof(exceptionCallback));
            }

            RelayCommand<TParameter> command = CommandBinder(() => new RelayCommand<TParameter>(commandAction, canExecuteAction, exceptionCallback), propertyName);

            return command;

        }

        public RelayCommand TryExecuteExclusiveCommand(Action<IExclusiveContext> commandAction, Func<bool> canExecuteAction, Action<Exception> exceptionCallback, [CallerMemberName] string propertyName = null)
        {
            if (commandAction is null)
            {
                throw new ArgumentNullException(nameof(commandAction));
            }
            if (exceptionCallback is null)
            {
                throw new ArgumentNullException(nameof(exceptionCallback));
            }


            RelayCommand command = CommandBinder(() => new RelayCommand(commandAction, canExecuteAction, exceptionCallback), propertyName);

            return command;
        }

        public RelayCommand<TParameter> TryExecuteExclusiveCommand<TParameter>(Action<IExclusiveContext<TParameter>> commandAction, Func<bool> canExecuteAction, Action<Exception> exceptionCallback, [CallerMemberName] string propertyName = null)
        {
            if (commandAction is null)
            {
                throw new ArgumentNullException(nameof(commandAction));
            }
            if (exceptionCallback is null)
            {
                throw new ArgumentNullException(nameof(exceptionCallback));
            }
            RelayCommand<TParameter> command = CommandBinder(() => new RelayCommand<TParameter>(commandAction, canExecuteAction, exceptionCallback), propertyName);

            return command;
        }



        public RelayCommand TryExecuteCommand(Action commandAction, Action<Exception> exceptionCallback, [CallerMemberName] string propertyName = null)
        {
            if (commandAction is null)
            {
                throw new ArgumentNullException(nameof(commandAction));
            }

            if (exceptionCallback is null)
            {
                throw new ArgumentNullException(nameof(exceptionCallback));
            }
            RelayCommand command = CommandBinder(() => new RelayCommand(commandAction, null, exceptionCallback), propertyName);

            return command;

        }

        public RelayCommand<TParameter> TryExecuteCommand<TParameter>(Action<TParameter> commandAction, Action<Exception> exceptionCallback, [CallerMemberName] string propertyName = null)
        {
            if (commandAction is null)
            {
                throw new ArgumentNullException(nameof(commandAction));
            }
            if (exceptionCallback is null)
            {
                throw new ArgumentNullException(nameof(exceptionCallback));
            }

            RelayCommand<TParameter> command = CommandBinder(() => new RelayCommand<TParameter>(commandAction, null, exceptionCallback), propertyName);

            return command;
        }

        public RelayCommand TryExecuteExclusiveCommand(Action<IExclusiveContext> commandAction, Action<Exception> exceptionCallback, [CallerMemberName] string propertyName = null)
        {
            if (commandAction is null)
            {
                throw new ArgumentNullException(nameof(commandAction));
            }
            if (exceptionCallback is null)
            {
                throw new ArgumentNullException(nameof(exceptionCallback));
            }

            RelayCommand command = CommandBinder(() => new RelayCommand(commandAction, null, exceptionCallback), propertyName);

            return command;

        }

        public RelayCommand<TParameter> TryExecuteExclusiveCommand<TParameter>(Action<IExclusiveContext<TParameter>> commandAction, Action<Exception> exceptionCallback, [CallerMemberName] string propertyName = null)
        {
            if (commandAction is null)
            {
                throw new ArgumentNullException(nameof(commandAction));
            }
            if (exceptionCallback is null)
            {
                throw new ArgumentNullException(nameof(exceptionCallback));
            }

            RelayCommand<TParameter> command = CommandBinder(() => new RelayCommand<TParameter>(commandAction, null, exceptionCallback), propertyName);

            return command;
        }


        public TCommand CommandBinder<TCommand>(Func<TCommand> commandFunc, [CallerMemberName] string propertyName = null) where TCommand : class, ICommand
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            if (commandFunc is null)
            {
                throw new ArgumentNullException(nameof(commandFunc));
            }

            TCommand command = ViewModelCommands.GetOrAdd(propertyName, i => commandFunc()) as TCommand;


            return command;
        }


        #endregion


        private readonly ConcurrentDictionary<string, ICommand> ViewModelCommands = new ConcurrentDictionary<string, ICommand>();


    }
}
