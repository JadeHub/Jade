using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using JadeControls;
using System.Windows;

namespace JadeGui.ViewModels
{
    using JadeData;

    internal class WorkspaceViewModel : JadeControls.NotifyPropertyChanged, JadeCore.ViewModels.IWorkspaceViewModel
    {
        private JadeData.Workspace.IWorkspace _data;
        private JadeControls.Workspace.ViewModel.WorkspaceTree _tree;
        private bool _modified;

        public WorkspaceViewModel(JadeData.Workspace.IWorkspace data)
        {
            _data = data;
            _modified = false;
            _tree = new JadeControls.Workspace.ViewModel.WorkspaceTree(_data);
        }

        public JadeControls.Workspace.ViewModel.WorkspaceTree Tree
        {
            get
            {
                return _tree;
            }
        }

        public string Name 
        {
            get
            {
                return _data.Name;
            }
        }

        public string Directory
        {
            get
            {
                return _data.Directory;
            }
        }

        public string Path
        {
            get
            {
                return _data.Path;
            }
            set
            {
                _data.Path = value;
                OnPropertyChanged("Path");
                OnPropertyChanged("Directory");
            }
        }

        public bool Modified
        {
            get { return _modified || _tree.Modified; }
            set { _modified = value; }
        }
    }
   
    /// <summary>
    /// Main View Model class. Singleton instance that lives the life of the application
    /// </summary>
    internal class JadeViewModel : JadeControls.NotifyPropertyChanged, JadeCore.ViewModels.IJadeViewModel
    {
        #region Data

        private JadeCore.IWorkspaceManager _workspaceManager;
        private Commands.NewWorkspace _newWorkspaceCmd;
        private Commands.OpenWorkspace _openWorkspaceCmd;
        private Commands.SaveWorkspace _saveWorkspaceCmd;
        private Commands.SaveAsWorkspace _saveAsWorkspaceCmd;
        private Commands.CloseWorkspace _closeWorkspaceCmd;

        private ICommand _exitCommand;

        #endregion

        public JadeViewModel()
        {
            _workspaceManager = new WorkspaceManager();
            _workspaceManager.WorkspaceOpened += delegate { OnPropertyChanged("Workspace"); };

            _editorModel = new JadeControls.EditorControl.ViewModel.EditorControlViewModel();
            _newWorkspaceCmd = new Commands.NewWorkspace(this);
            _openWorkspaceCmd = new Commands.OpenWorkspace(this);
            _saveWorkspaceCmd = new Commands.SaveWorkspace(this);
            _saveAsWorkspaceCmd = new Commands.SaveAsWorkspace(this);
            _closeWorkspaceCmd = new Commands.CloseWorkspace(this);
        }

        #region EditorControl

        private JadeControls.EditorControl.ViewModel.EditorControlViewModel _editorModel;

        public JadeCore.ViewModels.IEditorViewModel Editor
        {
            get { return _editorModel; }
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

        #region Workspace Tree

        public JadeCore.ViewModels.IWorkspaceViewModel Workspace
        {
            get { return _workspaceManager.ViewModel; }
        }

        #endregion

        #region Commands

        public ICommand CloseWorkspaceCommand { get { return _closeWorkspaceCmd.Command; } }
        public ICommand NewWorkspaceCommand { get { return _newWorkspaceCmd.Command; } }
        public ICommand OpenWorkspaceCommand { get { return _openWorkspaceCmd.Command;}}
        public ICommand SaveWorkspaceCommand { get { return _saveWorkspaceCmd.Command; } }
        public ICommand SaveAsWorkspaceCommand { get { return _saveAsWorkspaceCmd.Command; } }

        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                {
                    _exitCommand = new RelayCommand(delegate { OnRequestClose(); });
                }
                return _exitCommand;
            }
        }

        #endregion

        #region public Methods

        public bool OnExit()
        {
            if (_workspaceManager.RequiresSave)
            {
                return _workspaceManager.SaveOrDiscardWorkspace();
            }
            return true;
        }
       
        #endregion

        public JadeCore.IWorkspaceManager WorkspaceManager
        {
            get
            {
                return _workspaceManager;
            }
        }
    }
}
