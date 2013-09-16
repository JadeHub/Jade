using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using JadeControls;
using System.Windows;
using JadeCore.IO;

namespace JadeGui.ViewModels
{
    using JadeData;
    
    /// <summary>
    /// Main View Model class. Singleton instance that lives the life of the application
    /// </summary>
    internal class JadeViewModel : JadeControls.NotifyPropertyChanged, JadeCore.ViewModels.IJadeViewModel
    {
        #region Data

        private JadeCommandAdaptor _commands;
        private IWorkspaceManager _workspaceManager;
       
        #endregion

        public JadeViewModel()
        {
            _workspaceManager = new WorkspaceManager();
            _workspaceManager.WorkspaceOpened += delegate { OnPropertyChanged("Workspace"); };
            _editorModel = new JadeControls.EditorControl.ViewModel.EditorControlViewModel();
            _commands = new JadeCommandAdaptor(this);
        }

        public JadeCommandAdaptor Commands { get { return _commands; } }

        #region EditorControl

        private JadeControls.EditorControl.ViewModel.EditorControlViewModel _editorModel;

        public JadeCore.ViewModels.IEditorViewModel Editor
        {
            get { return _editorModel; }
        }

        #endregion

        #region Workspace

        public IWorkspaceManager WorkspaceManager
        {
            get
            {
                return _workspaceManager;
            }
        }

        #endregion

        #region RequestClose [event]

        /// <summary>
        /// Raised to request ui to close
        /// </summary>
        public event EventHandler RequestClose;

        void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion // RequestClose [event]

        #region Workspace
                
        public JadeCore.ViewModels.IWorkspaceViewModel Workspace
        {
            get { return _workspaceManager.ViewModel; }
        }

        #endregion

        #region Commands

        #region Exit

        public void OnExit()
        {
            OnRequestClose();
        }

        #endregion
        
        public void OnNewWorkspace()
        {
            string name;
            if (JadeCore.GuiUtils.PromptUserInput("Workspace name", out name))
            {
                try
                {
                    WorkspaceManager.NewWorkspace(name, "");
                }
                catch (Exception e)
                {
                    JadeCore.GuiUtils.DisplayErrorAlert("Error creating new workspace. " + e.ToString());
                }
            }
        }

        public void OnCloseWorkspace()
        {
            WorkspaceManager.CloseWorkspace();
        }

        public bool CanCloseWorkspace()
        {
            return WorkspaceManager.WorkspaceOpen;
        }

        public void OnOpenWorkspace()
        {
            IFileHandle handle = JadeCore.GuiUtils.PromptOpenFile(".jws", "Jade Workspace files (.jws)|*.jws", true);
            if (handle == null)
            {
                return;
            }

            try
            {
                WorkspaceManager.OpenWorkspace(handle);
            }
            catch (Exception e)
            {
                JadeCore.GuiUtils.DisplayErrorAlert("Error opening workspace. " + e.ToString());
            }
        }

        public bool CanOpenWorkspace()
        {
            return true;
        }

        public void OnSaveWorkspace()
        {
            string path = Workspace.Path;
            if (path == null || path.Length == 0)
            {
                if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path) == false)
                    return;
            }
            WorkspaceManager.SaveWorkspaceAs(path);
        }

        public bool CanSaveWorkspace()
        {
            return WorkspaceManager.RequiresSave;
        }

        public void OnSaveAsWorkspace()
        {
            string path;
            if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path))
            {
                WorkspaceManager.SaveWorkspaceAs(path);
            }
        }

        public bool CanSaveAsWorkspace()
        {
            return WorkspaceManager.WorkspaceOpen;
        }

        #endregion

        #region public Methods

        public bool RequestExit()
        {
            if (_workspaceManager.RequiresSave)
            {
                return _workspaceManager.SaveOrDiscardWorkspace();
            }
            return true;
        }
       
        #endregion

        
    }
}
