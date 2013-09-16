using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading.Tasks;

namespace JadeGui
{
    public interface IWorkspaceManager
    {
        event EventHandler WorkspaceOpened;

        /// <summary>
        /// Returns true if there is an open workspace
        /// </summary>
        bool WorkspaceOpen { get; }

        /// <summary>
        /// Returns true if the open workspace has been modified
        /// </summary>
        bool RequiresSave { get; }

        /// <summary>
        /// Close the open workspace. throws if RequiresSave is true
        /// </summary>
        bool CloseWorkspace();

        /// <summary>
        /// Create a new workspace. Throws if WorkspaceOpen is true
        /// </summary>
        /// <param name="name"></param>
        /// <param name="path"></param>
        void NewWorkspace(string name, string path);

        /// <summary>
        /// Open a workspace file. Throws if WorkspaceOpen is true
        /// </summary>
        /// <param name="path">full path to .jws file</param>
        void OpenWorkspace(JadeCore.IO.IFileHandle file);

        /// <summary>
        /// Save the current workspace. Will use the current path or prompt the user if unknown.
        /// </summary>
        /// <returns>true if saved</returns>
        bool SaveWorkspace();

        /// <summary>
        /// Prompt the use user to either Save, discard or cancel.
        /// </summary>
        /// <returns>true if saved or discarded</returns>
        bool SaveOrDiscardWorkspace();

        /// <summary>
        /// Save the current workspace to the specified path.
        /// </summary>
        /// <returns>true if saved</returns>
        bool SaveWorkspaceAs(string path);

        JadeCore.ViewModels.IWorkspaceViewModel ViewModel { get; }
    }

    internal class WorkspaceManager : IWorkspaceManager
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

        public bool CloseWorkspace()
        {
            if (WorkspaceOpen == false)
                return true;

            if (RequiresSave && SaveOrDiscardWorkspace() == false)
            {
                return false;
            }
            _workspace = null;
            _viewModel = null;
            OnWorkspaceOpened();
            return true;
        }

        /// <summary>
        /// Prompt the use user to either Save, discard or cancel.
        /// </summary>
        /// <returns>true if saved or discarded</returns>
        public bool SaveOrDiscardWorkspace()
        {
            if (WorkspaceOpen == false)
            {
                throw new Exception("Attempt to save null workspace.");
            }

            if (_viewModel.Modified)
            {
                MessageBoxResult result = JadeCore.GuiUtils.PromptYesNoCancelQuestion("Workspace has been modified, do you wish to save?");
                if (result == MessageBoxResult.No)
                {
                    //abandon changes
                    _viewModel.Modified = false;
                    return true;
                }
                if (result == MessageBoxResult.Cancel)
                {
                    return false;
                }

                string path = _workspace.Path;
                if (path == null || path.Length == 0)
                {
                    if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path) == false)
                    {
                        return false;
                    }
                }
                return WriteWorkspace(path);
            }
            return true;
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
                string path = _workspace.Path;
                if (path == null || path.Length == 0)
                {
                    if(JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path) == false)
                    {
                        return false;
                    }                    
                }
                return WriteWorkspace(path);
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
            return WriteWorkspace(path);           
        }

        private bool WriteWorkspace(string path)
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
            if (CloseWorkspace() == false)
            {
                return;
            }

            try
            {
                _workspace = new JadeData.Workspace.Workspace(name, path);
                _viewModel = new ViewModels.WorkspaceViewModel(_workspace);
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

        public void OpenWorkspace(JadeCore.IO.IFileHandle file)
        {
            if (CloseWorkspace() == false)
            {
                return;
            }

            try
            {
                _workspace = JadeData.Persistence.Workspace.Reader.Read(file.Path);
                _viewModel = new ViewModels.WorkspaceViewModel(_workspace);
                OnWorkspaceOpened();
            }
            catch(Exception e)
            {
                _workspace = null;
                _viewModel = null;
                throw;
            }
        }
    }
}
