using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading.Tasks;

namespace JadeGui
{
    internal class WorkspaceManager : JadeCore.IWorkspaceManager
    {
        private JadeData.Workspace.IWorkspace _workspace;
        private JadeCore.ViewModels.IWorkspaceViewModel _viewModel;

        public event EventHandler WorkspaceOpened;

        private void OnWorkspaceOpened()
        {
            EventHandler handler = WorkspaceOpened;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        public JadeCore.ViewModels.IWorkspaceViewModel ViewModel 
        {
            get { return _viewModel; }
        }

        public bool WorkspaceOpen 
        {
            get { return _workspace != null; } 
        }

        public bool RequiresSave 
        {
            get
            {
                if (WorkspaceOpen && _viewModel.Modified)
                    return true;
                return false;
            }
        }

        public void CloseWorkspace()
        {
            if (RequiresSave)
            {
                throw new Exception("Attempt to close modified Workspace.");
            }
            _workspace = null;
            _viewModel = null;
        }

        /// <summary>
        /// Save the current workspace. Will use the current path or prompt the user if unknown.
        /// </summary>
        /// <returns>true if saved</returns>
        public bool SaveWorkspace()
        {
            if (WorkspaceOpen == false)
            {
                throw new Exception("Attempt to save null workspace.");
            }

            if (_viewModel.Modified)
            {
                MessageBoxResult result = JadeCore.GuiUtils.PromptYesNoCancelQuestion("Workspace has been modified, do you wish to save?");
                if(result == MessageBoxResult.No)
                {
                    //abandon changes
                    _viewModel.Modified = false;
                    return true;                    
                }
                if(result == MessageBoxResult.Cancel)
                {
                    return false;
                }

                string path = _workspace.Path;
                if (path == null || path.Length == 0)
                {
                    if(JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path) == false)
                    {
                        return false;
                    }                    
                }
                return SaveWorkspace(path);
            }
            return true;
        }

        /// <summary>
        /// Save the current workspace to the specified path.
        /// </summary>
        /// <returns>true if saved</returns>
        public bool SaveWorkspaceAs(string path)
        {
            if (WorkspaceOpen == false)
            {
                throw new Exception("Attempt to save null workspace.");
            }
            return SaveWorkspace(path);           
        }

        private bool SaveWorkspace(string path)
        {
            try
            {
                JadeData.Persistence.Workspace.Writer.Write(_workspace, path);
                _viewModel.Modified = false;
                _viewModel.Path = path;
                return true;
            }
            catch (Exception e)
            {
                JadeCore.GuiUtils.DisplayErrorAlert("Error saving workspace. " + e.ToString());
            }
            return false;
        }

        public void NewWorkspace(string name, string path)
        {
            if (RequiresSave && SaveWorkspace() == false)
            {
                return;
            }

            try
            {
                CloseWorkspace();
                _workspace = new JadeData.Workspace.Workspace(name, path);
                _viewModel = new JadeControls.Workspace.ViewModel.Workspace(_workspace);
                _viewModel.Modified = true;
                OnWorkspaceOpened();
            }
            catch(Exception)
            {
                _workspace = null;
                _viewModel = null;
                throw;
            }
        }

        public void OpenWorkspace(string path)
        {
            if (RequiresSave && SaveWorkspace() == false)
            {
                return;
            }
            
            try
            {
                CloseWorkspace();
                _workspace = JadeData.Persistence.Workspace.Reader.Read(path);
                _viewModel = new JadeControls.Workspace.ViewModel.Workspace(_workspace);
                OnWorkspaceOpened();
            }
            catch(Exception)
            {
                _workspace = null;
                _viewModel = null;
                throw;
            }
        }
    }
}
