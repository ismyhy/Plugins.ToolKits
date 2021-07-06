using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Plugins.ToolKits.MVVM
{
    public class CommandBinder 
    {
         
        #region  CommandExtensions

        public RelayCommand BindCommand(Action commandAction, Func<bool> canExecuteAction = null, Action<Exception> exceptionCallback = null, [CallerMemberName] string propertyName = null)
        {
            if (commandAction is null)
            {
                throw new ArgumentNullException(nameof(commandAction));
            }
            RelayCommand command = CommandMapper(() => new RelayCommand(commandAction, canExecuteAction, exceptionCallback), propertyName);

            return command;

        }

        public RelayCommand<TParameter> BindCommand<TParameter>(Action<TParameter> commandAction, Func<bool> canExecuteAction = null, Action<Exception> exceptionCallback = null, [CallerMemberName] string propertyName = null)
        {
            if (commandAction is null)
            {
                throw new ArgumentNullException(nameof(commandAction));
            }
            RelayCommand<TParameter> command = CommandMapper(() => new RelayCommand<TParameter>(commandAction, canExecuteAction, exceptionCallback), propertyName);

            return command;

        }

        public RelayCommand BindExclusiveCommand(Action<IExclusiveContext> commandAction, Func<bool> canExecuteAction = null, Action<Exception> exceptionCallback = null, [CallerMemberName] string propertyName = null)
        {
            if (commandAction is null)
            {
                throw new ArgumentNullException(nameof(commandAction));
            }
            RelayCommand command = CommandMapper(() => new RelayCommand(commandAction, canExecuteAction, exceptionCallback), propertyName);

            return command;
        }

        public RelayCommand<TParameter> BindExclusiveCommand<TParameter>(Action<IExclusiveContext<TParameter>> commandAction, Func<bool> canExecuteAction = null, Action<Exception> exceptionCallback = null, [CallerMemberName] string propertyName = null)
        {
            if (commandAction is null)
            {
                throw new ArgumentNullException(nameof(commandAction));
            }

            RelayCommand<TParameter> command = CommandMapper(() => new RelayCommand<TParameter>(commandAction, canExecuteAction, exceptionCallback), propertyName);

            return command;
        }


        public RelayCommand TryBindCommand(Action commandAction, Func<bool> canExecuteAction, Action<Exception> exceptionCallback, [CallerMemberName] string propertyName = null)
        {
            if (commandAction is null)
            {
                throw new ArgumentNullException(nameof(commandAction));
            }

            if (exceptionCallback is null)
            {
                throw new ArgumentNullException(nameof(exceptionCallback));
            }
            RelayCommand command = CommandMapper(() => new RelayCommand(commandAction, canExecuteAction, exceptionCallback), propertyName);

            return command;
        }

        public RelayCommand<TParameter> TryBindCommand<TParameter>(Action<TParameter> commandAction, Func<bool> canExecuteAction, Action<Exception> exceptionCallback, [CallerMemberName] string propertyName = null)
        {
            if (commandAction is null)
            {
                throw new ArgumentNullException(nameof(commandAction));
            }
            if (exceptionCallback is null)
            {
                throw new ArgumentNullException(nameof(exceptionCallback));
            }

            RelayCommand<TParameter> command = CommandMapper(() => new RelayCommand<TParameter>(commandAction, canExecuteAction, exceptionCallback), propertyName);

            return command;

        }

        public RelayCommand TryBindExclusiveCommand(Action<IExclusiveContext> commandAction, Func<bool> canExecuteAction, Action<Exception> exceptionCallback, [CallerMemberName] string propertyName = null)
        {
            if (commandAction is null)
            {
                throw new ArgumentNullException(nameof(commandAction));
            }
            if (exceptionCallback is null)
            {
                throw new ArgumentNullException(nameof(exceptionCallback));
            }


            RelayCommand command = CommandMapper(() => new RelayCommand(commandAction, canExecuteAction, exceptionCallback), propertyName);

            return command;
        }

        public RelayCommand<TParameter> TryBindExclusiveCommand<TParameter>(Action<IExclusiveContext<TParameter>> commandAction, Func<bool> canExecuteAction, Action<Exception> exceptionCallback, [CallerMemberName] string propertyName = null)
        {
            if (commandAction is null)
            {
                throw new ArgumentNullException(nameof(commandAction));
            }
            if (exceptionCallback is null)
            {
                throw new ArgumentNullException(nameof(exceptionCallback));
            }
            RelayCommand<TParameter> command = CommandMapper(() => new RelayCommand<TParameter>(commandAction, canExecuteAction, exceptionCallback), propertyName);

            return command;
        }



        public RelayCommand TryBindCommand(Action commandAction, Action<Exception> exceptionCallback, [CallerMemberName] string propertyName = null)
        {
            if (commandAction is null)
            {
                throw new ArgumentNullException(nameof(commandAction));
            }

            if (exceptionCallback is null)
            {
                throw new ArgumentNullException(nameof(exceptionCallback));
            }
            RelayCommand command = CommandMapper(() => new RelayCommand(commandAction, null, exceptionCallback), propertyName);

            return command;

        }

        public RelayCommand<TParameter> TryBindCommand<TParameter>(Action<TParameter> commandAction, Action<Exception> exceptionCallback, [CallerMemberName] string propertyName = null)
        {
            if (commandAction is null)
            {
                throw new ArgumentNullException(nameof(commandAction));
            }
            if (exceptionCallback is null)
            {
                throw new ArgumentNullException(nameof(exceptionCallback));
            }

            RelayCommand<TParameter> command = CommandMapper(() => new RelayCommand<TParameter>(commandAction, null, exceptionCallback), propertyName);

            return command;
        }

        public RelayCommand TryBindExclusiveCommand(Action<IExclusiveContext> commandAction, Action<Exception> exceptionCallback, [CallerMemberName] string propertyName = null)
        {
            if (commandAction is null)
            {
                throw new ArgumentNullException(nameof(commandAction));
            }
            if (exceptionCallback is null)
            {
                throw new ArgumentNullException(nameof(exceptionCallback));
            }

            RelayCommand command = CommandMapper(() => new RelayCommand(commandAction, null, exceptionCallback), propertyName);

            return command;

        }

        public RelayCommand<TParameter> TryBindExclusiveCommand<TParameter>(Action<IExclusiveContext<TParameter>> commandAction, Action<Exception> exceptionCallback, [CallerMemberName] string propertyName = null)
        {
            if (commandAction is null)
            {
                throw new ArgumentNullException(nameof(commandAction));
            }
            if (exceptionCallback is null)
            {
                throw new ArgumentNullException(nameof(exceptionCallback));
            }

            RelayCommand<TParameter> command = CommandMapper(() => new RelayCommand<TParameter>(commandAction, null, exceptionCallback), propertyName);

            return command;
        }


        public TCommand CommandMapper<TCommand>(Func<TCommand> commandFunc, [CallerMemberName] string propertyName = null) where TCommand : class, ICommand
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