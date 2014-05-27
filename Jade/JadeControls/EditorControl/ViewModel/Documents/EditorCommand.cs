using System;
using System.Diagnostics;
using System.Windows.Input;
using JadeUtils.IO;

namespace JadeControls.EditorControl.ViewModel
{
    public abstract class EditorCommand<ArgT> : ICommand
    {
        public event EventHandler CanExecuteChanged;

        protected void RaiseCanExecuteChangedEvent()
        {
            EventHandler handler = CanExecuteChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        public bool CanExecute(object parameter)
        {
            if (parameter == null)
                return false;
            Debug.Assert(parameter is ArgT);
            return CanExecute((ArgT)parameter);
        }

        public void Execute(object parameter)
        {
            Execute((ArgT)parameter);
        }

        protected abstract bool CanExecute(ArgT arg);
        protected abstract void Execute(ArgT arg);
    }
}
