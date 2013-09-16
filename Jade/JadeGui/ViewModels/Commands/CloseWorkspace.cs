using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class CloseWorkspace
    {
        private JadeCore.ViewModels.IJadeViewModel _viewModel;
        private RelayCommand _cmd;

        public ICommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = new RelayCommand(param => this.OnCommand(), param => this.CanDoCommand);
                }
                return _cmd;
            }
        }

        public CloseWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            _viewModel.WorkspaceManager.CloseWorkspace();            
        }

        private bool CanDoCommand
        {
            get
            {
                return _viewModel.WorkspaceManager.WorkspaceOpen;
            }
        }
    }
}
