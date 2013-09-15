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
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            //dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".jws"; // Default file extension
            dlg.Filter = "Jade Workspace files (.jws)|*.jws"; // Filter files by extension
            dlg.CheckFileExists = true;
            dlg.CheckPathExists = true;

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;

                try
                {
                    JadeData.Workspace.IWorkspace workspace = JadeData.Persistence.Workspace.Reader.Read(filename);
                    _viewModel.Workspace = new JadeControls.Workspace.ViewModel.Workspace(workspace);
                }
                catch (System.Exception e)
                {

                }
            }
        }

        private bool CanDoCommand { get { return true; } }
    }
}
