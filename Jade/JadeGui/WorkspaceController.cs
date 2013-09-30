using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading.Tasks;
using JadeUtils.IO;
using JadeCore;

namespace JadeGui
{
    public class WorkspaceController : JadeCore.IWorkspaceController
    {
        #region Data

        private JadeData.Workspace.IWorkspace _workspace;
        private bool _modified;
        private JadeCore.RecentFileList _recentFiles;

        #endregion

        #region Events

        public event EventHandler WorkspaceChanged;

        private void OnWorkspaceChanged()
        {
            EventHandler handler = WorkspaceChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion

        #region Constructor

        public WorkspaceController()
        {
            _modified = false;
            _recentFiles = new JadeCore.RecentFileList();
            LoadSettings(JadeCore.Services.Provider.Settings);
        }

        #endregion
        
        #region Public Properties

        public JadeData.Workspace.IWorkspace CurrentWorkspace 
        {
            get { return _workspace; }
        }

        public RecentFileList RecentFiles 
        { 
            get { return _recentFiles;}
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

        #endregion
        
        #region Public Methods

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
                
        public bool SaveWorkspace(string path)
        {
            if (WorkspaceOpen == false)
            {
                throw new Exception("Attempt to save null workspace.");
            }
            return WriteWorkspace(path);           
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
                _recentFiles.Add(file.Path.Str);
                OnWorkspaceChanged();
            }
            catch(Exception)
            {
                _workspace = null;
                throw;
            }
        }

        #endregion

        #region Private Methods

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

        #endregion

        #region Settings

        private void LoadSettings(JadeCore.Properties.Settings settings)
        {
            if (settings.RecentFiles != null)
            {
                _recentFiles.Load(settings.RecentFiles);
            }
        }

        public void SaveSettings()
        {
            JadeCore.Properties.Settings settings = JadeCore.Services.Provider.Settings;
            if (settings.RecentFiles == null)
                settings.RecentFiles = new System.Collections.Specialized.StringCollection();
            _recentFiles.Save(settings.RecentFiles);
        }

        #endregion

    }
}
