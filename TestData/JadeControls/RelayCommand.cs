using System;
using System.Diagnostics;
using System.Windows.Input;

namespace JadeControls
{
    /// <summary>
    /// A command whose sole purpose is to 
    /// relay its functionality to other
    /// objects by invoking delegates. The
    /// default return value for the CanExecute
    /// method is 'true'.
    /// </summary>
    public class ObjectCommand<T> : ICommand
    {
        #region Fields

        private Action<T> _execute;
        private Predicate<T> _canExecute;
        private T _obj;

        #endregion // Fields

        #region Constructors

        public ObjectCommand(T obj)
        {
            _obj = obj;
            _execute = null;
            _canExecute = null;
        }

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public ObjectCommand(T obj, Action<T> execute, Predicate<T> canExecute)
        {
            _obj = obj;
            _execute = execute;
            _canExecute = canExecute;
        }

        #endregion // Constructors

        public void Attach(Action<T> execute, Predicate<T> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        #region ICommand Members

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
            {
                return _execute != null;
            }
            return _canExecute(_obj);
        }
         
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            if(_execute != null)
                _execute(_obj);
        }

        #endregion // ICommand Members
    }

    public class RelayCommand : ObjectCommand<object>
    {
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
            : base(null, execute, canExecute)
        {
        }

        public RelayCommand(Action<object> execute)
            : base(null, execute, null)
        {
        }
    }
    
}