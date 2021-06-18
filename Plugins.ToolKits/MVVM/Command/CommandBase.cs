using System;
using System.Windows.Input;

namespace Plugins.ToolKits.MVVM
{
    public abstract class CommandBase : ICommand
    {
        protected internal Func<bool> CanExecuteFunc;
        protected internal Action<Exception> CatchCallback;

        protected bool isRunning;
        public abstract void Execute(object parameter);

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return CanExecuteFunc?.Invoke() ?? true;
        }

        //public void NotifyCanExecuteChanged()
        //{
        //    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        //}
    }
}