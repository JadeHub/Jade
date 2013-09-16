using System;
using System.Windows.Input;

namespace JadeGui.ViewModels.Commands
{
    using JadeControls;

    internal class OpenWorkspace
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

        public OpenWorkspace(JadeCore.ViewModels.IJadeViewModel vm)        
        {
            _viewModel = vm;     
        }

        private void OnCommand()
        {
            string path;
            if (JadeCore.GuiUtils.PromptOpenFile(".jws", "Jade Workspace files (.jws)|*.jws", true, out path) == false)
            {
                return;
            }

            try
            {
                _viewModel.WorkspaceManager.OpenWorkspace(path);
            }
            catch (Exception e)
            {
                JadeCore.GuiUtils.DisplayErrorAlert("Error opening workspace. " + e.ToString());
            }            
        }

        private bool CanDoCommand { get { return true; } }
    }
}
