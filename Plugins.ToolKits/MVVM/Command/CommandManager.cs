using System;

namespace Plugins.ToolKits.MVVM
{
    public class CommandManager
    {










        public static RelayCommand BindCommand(Action commandAction, Func<bool> canExecuteAction = null, Action<Exception> exceptionCallback = null)
        {
            return new RelayCommand(commandAction, canExecuteAction, exceptionCallback);
        }
        public static RelayCommand<T> BindCommand<T>(Action<T> commandAction, Func<bool> canExecuteAction = null, Action<Exception> exceptionCallback = null)
        {
            return new RelayCommand<T>(commandAction, canExecuteAction, exceptionCallback);
        }
        public static RelayCommand BindExclusiveCommand(Action<IExclusiveContext> commandAction, Func<bool> canExecuteAction = null, Action<Exception> exceptionCallback = null)
        {
            return new RelayCommand(commandAction, canExecuteAction, exceptionCallback);
        }
        public static RelayCommand<T> BindExclusiveCommand<T>(Action<IExclusiveContext<T>> commandAction, Func<bool> canExecuteAction = null, Action<Exception> exceptionCallback = null)
        {
            return new RelayCommand<T>(commandAction, canExecuteAction, exceptionCallback);
        }

        public static RelayCommand BindTryCommand(Action commandAction, Action<Exception> exceptionCallback, Func<bool> canExecuteAction = null)
        {
            return new RelayCommand(commandAction, canExecuteAction, exceptionCallback);
        }
        public static RelayCommand<T> BindTryCommand<T>(Action<T> commandAction, Action<Exception> exceptionCallback, Func<bool> canExecuteAction = null)
        {
            return new RelayCommand<T>(commandAction, canExecuteAction, exceptionCallback);
        }
        public static RelayCommand BindTryExclusiveCommand(Action<IExclusiveContext> commandAction, Action<Exception> exceptionCallback)
        {
            return new RelayCommand(commandAction, null, exceptionCallback);
        }
        public static RelayCommand<T> BindTryExclusiveCommand<T>(Action<IExclusiveContext<T>> commandAction, Action<Exception> exceptionCallback)
        {
            return new RelayCommand<T>(commandAction, null, exceptionCallback);
        }
    }
}