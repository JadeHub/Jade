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
    /// <summary>
    /// Main View Model class. Singleton instance that lives the life of the application
    /// </summary>
    internal class JadeViewModel// : NotifyPropertyChangedImpl
    {
        public JadeViewModel()
        {
            _workspace = JadeData.FakeData.Workspace;
            _workspaceModel = new JadeControls.Workspace.ViewModel.Workspace(_workspace);

            _editorModel = new JadeControls.EditorControl.ViewModel.EditorControlViewModel();            
        }

        #region EditorControl

        private JadeControls.EditorControl.ViewModel.EditorControlViewModel _editorModel;

        public JadeControls.EditorControl.ViewModel.EditorControlViewModel EditorModel
        {
            get { return _editorModel; }
        }

        #endregion

        #region RequestClose [event]

        /// <summary>
        /// Raised when this workspace should be removed from the UI.
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

        private JadeData.Workspace.IWorkspace _workspace;
        private JadeControls.Workspace.ViewModel.Workspace _workspaceModel;       

        public JadeControls.Workspace.ViewModel.Workspace Workspace
        {
            get { return _workspaceModel; }
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
                    _closeCommand = new RelayCommand(param => this.OnCloseCommand(), param => this.CanCloseCommand);
                }
                return _closeCommand;
            }
        }

        private void OnCloseCommand()
        {
            OnRequestClose();
        }

        private bool CanCloseCommand { get { return true; } }

        #endregion

        #endregion

        #region public Methods

        public bool OnExit()
        {
            if (Workspace.Modified)
            {
                MessageBoxResult result = MessageBox.Show("Do you wish to save Workspace " + _workspace.Name + " before exiting?", "Confirm save",
                                                MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                if (result == MessageBoxResult.Cancel)
                {
                    return false;
                }
                else if (result == MessageBoxResult.Yes)
                {
                    Workspace.Save(Workspace.Path);
                }                
            }
            return true;
        }

        #endregion
    }
}
