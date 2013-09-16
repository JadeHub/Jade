using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class SaveWorkspace
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

        public SaveWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path = _viewModel.Workspace.Path;

            if(path == null || path.Length == 0)
            {
                if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path) == false)
                    return;                
            }
            _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.RequiresSave; } }
    }

    internal class SaveAsWorkspace
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

        public SaveAsWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            string path;

            if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path))
            {
                _viewModel.WorkspaceManager.SaveWorkspaceAs(path);
            }
        }

        private bool CanDoCommand { get { return _viewModel.WorkspaceManager.WorkspaceOpen; } }
    }
}
