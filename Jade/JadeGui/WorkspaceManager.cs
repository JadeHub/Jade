using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading.Tasks;
using JadeUtils.IO;

namespace JadeGui
{
    public class WorkspaceController : JadeCore.IWorkspaceController
    {
        private JadeData.Workspace.IWorkspace _workspace;
        private bool _modified;

        public event EventHandler WorkspaceChanged;

        public WorkspaceController()
        {
            _modified = false;
        }

        private void OnWorkspaceChanged()
        {
            EventHandler handler = WorkspaceChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
               
        public JadeData.Workspace.IWorkspace CurrentWorkspace 
        {
            get { return _workspace; }
        }

        public bool CurrentWorkspaceModified 
        {
            get { return _modified; }
            set { _modified = value; }
        }

        public bool WorkspaceOpen 
        {
            get { return _workspace != null; } 
        }

        public bool RequiresSave 
        {
            get
            {
                if (WorkspaceOpen && CurrentWorkspaceModified)
                    return true;
                return false;
            }
        }

        /// <summary>
        /// Close the current workspace, if open.
        /// </summary>
        /// <returns></returns>
        public bool CloseWorkspace()
        {
            if (WorkspaceOpen == false)
                return true;

            if (RequiresSave && SaveOrDiscardWorkspace() == false)
            {
                return false;
            }
            _workspace = null;
            CurrentWorkspaceModified = false;
            OnWorkspaceChanged();
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

            if (CurrentWorkspaceModified)
            {
                MessageBoxResult result = JadeCore.GuiUtils.PromptYesNoCancelQuestion("Workspace has been modified, do you wish to save?");
                if (result == MessageBoxResult.No)
                {
                    //abandon changes
                    CurrentWorkspaceModified = false;
                    OnWorkspaceChanged();
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

            if (CurrentWorkspaceModified)
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
                CurrentWorkspaceModified = false;
                _workspace.Path = path;
                OnWorkspaceChanged();
                return true;
            }
            catch (Exception e)
            {
                JadeCore.GuiUtils.DisplayErrorAlert("Error saving workspace. " + e.ToString());
            }
            return false;
        }

        public void NewWorkspace(string name)
        {
            if (CloseWorkspace() == false)
            {
                return;
            }

            try
            {
                IFileHandle file = JadeCore.Services.Provider.FileService.MakeFileHandle(FilePath.MakeTemporaryFilePath());

                _workspace = new JadeData.Workspace.Workspace(name, file);
                CurrentWorkspaceModified = true;
                OnWorkspaceChanged();
            }
            catch(Exception)
            {
                _workspace = null;
                throw;
            }
        }

        public void OpenWorkspace(IFileHandle file)
        {
            if (CloseWorkspace() == false)
            {
                return;
            }

            try
            {
                _workspace = JadeData.Persistence.Workspace.Reader.Read(file, JadeCore.Services.Provider.FileService);
                OnWorkspaceChanged();
            }
            catch(Exception e)
            {
                _workspace = null;
                throw;
            }
        }
    }
}
