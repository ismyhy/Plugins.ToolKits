using System;
using System.Windows.Input;

namespace Plugins.ToolKits.MVVM
{
    public abstract class CommandBase : ICommand
    {
        internal Func<bool> canExecuteFunc;
        internal Action<Exception> catchCallback;

        protected bool IsCommandExecuting;
        public abstract void Execute(object parameter);

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return canExecuteFunc?.Invoke() ?? true;
        }

        //public void NotifyCanExecuteChanged()
        //{
        //    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        //}
    }
}