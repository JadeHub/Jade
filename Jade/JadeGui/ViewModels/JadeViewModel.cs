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

   
    /// <summary>
    /// Main View Model class. Singleton instance that lives the life of the application
    /// </summary>
    internal class JadeViewModel : JadeControls.NotifyPropertyChanged, JadeCore.ViewModels.IJadeViewModel
    {
        public JadeViewModel()
        {
            _editorModel = new JadeControls.EditorControl.ViewModel.EditorControlViewModel();
            _openWorkspaceCmd = new Commands.OpenWorkspace(this);
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

        private JadeCore.ViewModels.IWorkspaceViewModel _workspaceModel;
    
        public JadeCore.ViewModels.IWorkspaceViewModel Workspace
        {
            get { return _workspaceModel; }
            set { _workspaceModel = value; OnPropertyChanged("Workspace"); }
        }

        #endregion

        #region Commands

        #region Close Command

        private RelayCommand _closeCommand;

        public ICommand CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                {
                    _closeCommand = new RelayCommand(param => OnRequestClose(), param => this.CanCloseCommand);
                }
                return _closeCommand;
            }
        }

        private bool CanCloseCommand { get { return true; } }

        #endregion

        #region Open Workspace Command

        private Commands.OpenWorkspace _openWorkspaceCmd;

        public ICommand OpenWorkspaceCommand { get { return _openWorkspaceCmd.Command;}}

        #endregion

        #endregion

        #region public Methods

        public bool OnExit()
        {
            if (Workspace != null)
                return Workspace.SaveOnExit();
            return true;
        }

        #endregion
    }
}
