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
            string name;

            if (JadeCore.GuiUtils.PromptUserInput("Workspace name", out name))
            {
                try
                {
                    _viewModel.WorkspaceManager.NewWorkspace(name, "");
                }
                catch (Exception e)
                {
                    JadeCore.GuiUtils.DisplayErrorAlert("Error creating new workspace. " + e.ToString());
                }
            }
        }

        private bool CanDoCommand 
        { 
            get { return true; } 
        }
    }
}
