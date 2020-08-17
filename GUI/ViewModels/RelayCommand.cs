using System;
using System.Windows.Input;

namespace GUI.ViewModels
{
    public class RelayCommand<T> : ICommand
    {
        readonly Action<T> _execute = null;
        readonly Func<bool> _canExecute = null;

        public RelayCommand(Action<T> execute) : this(execute, null) {}

        public RelayCommand(Action<T> execute, Func<bool> canExecute)
        {
            if (execute is null)
            {
                throw new ArgumentNullException("execute");
            }
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute is null || _canExecute();
        }

        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
