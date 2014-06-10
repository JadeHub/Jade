using System;
using System.Diagnostics;
using System.Windows.Input;
using JadeUtils.IO;

namespace JadeControls.EditorControl.ViewModel.Commands
{
    public abstract class EditorCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private DocumentViewModel _docVm;

        public EditorCommand(DocumentViewModel vm)
        {
            _docVm = vm;   
        }

        protected void RaiseCanExecuteChangedEvent()
        {
            EventHandler handler = CanExecuteChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        public bool CanExecute(object parameter)
        {
           return CanExecute();
        }

        public void Execute(object parameter)
        {
            Execute();
        }

        protected abstract bool CanExecute();
        protected abstract void Execute();

        protected DocumentViewModel ViewModel
        {
            get { return _docVm; }
        }
    }
}
