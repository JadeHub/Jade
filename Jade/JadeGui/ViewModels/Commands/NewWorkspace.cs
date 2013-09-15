using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class NewWorkspace
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

        public NewWorkspace(JadeCore.ViewModels.IJadeViewModel vm)
        {
            _viewModel = vm;
        }

        private void OnCommand()
        {
            JadeCore.IWorkspaceManager wrkmgr = _viewModel.WorkspaceManager;
            if (wrkmgr.WorkspaceOpen && wrkmgr.RequiresSave)
            {
                if (wrkmgr.SaveWorkspace() == false)
                    return;
            }

            string name;

            if (JadeCore.GuiUtils.PromptUserInput("Workspace name", out name))
            {
                try
                {
                    wrkmgr.NewWorkspace(name, "");
                }
                catch (Exception e)
                {
                    JadeCore.GuiUtils.DisplayErrorAlert("Error creating new workspace. " + e.ToString());
                }
            }
/*
            if (_viewModel.CloseWorkspace() == false)
            {
                return;
            }

            if (JadeCore.GuiUtils.PromptUserInput("Workspace name", out name))
            {
                JadeData.Workspace.IWorkspace workspace = new JadeData.Workspace.Workspace(name, "");
                _viewModel.Workspace = new JadeControls.Workspace.ViewModel.Workspace(workspace);
            }*/
        }

        private bool CanDoCommand 
        { 
            get 
            { 
                
                return true; 
            } 
        }
    }
}
