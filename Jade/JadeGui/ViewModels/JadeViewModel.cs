using JadeUtils.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace JadeGui.ViewModels
{
    using JadeControls.EditorControl.ViewModel;
    using JadeControls.OutputControl.ViewModel;
    using JadeControls.Workspace.ViewModel;
    
    /// <summary>
    /// Main View Model class. Singleton instance that lives the life of the application
    /// </summary>
    internal class JadeViewModel : JadeControls.NotifyPropertyChanged, JadeCore.IJadeCommandHandler
    {
        #region Data

        private JadeCommandAdaptor _commands;

        private JadeCore.IWorkspaceController _workspaceController;
        private WorkspaceViewModel _currentWorkspace;

        private EditorControlViewModel _editorModel;
        private JadeCore.IEditorController _editorController;

        private JadeCore.Output.IOutputController _outputController;
        private OutputViewModel _outputModel;

        private Window _view;

        #endregion

        #region Contsructor

        public JadeViewModel(Window view)
        {
            _workspaceController = JadeCore.Services.Provider.WorkspaceController;
            //Todo - Workspace viewmodel to track WorkspaceController changes
            _workspaceController.WorkspaceChanged += delegate { OnWorkspaceChanged(); };

            _editorController = JadeCore.Services.Provider.EditorController;
            _editorModel = new JadeControls.EditorControl.ViewModel.EditorControlViewModel(_editorController);

            _outputController = JadeCore.Services.Provider.OutputController;
            _outputModel = new OutputViewModel(_outputController);

            _commands = new JadeCommandAdaptor(this);
            _view = view;
            UpdateWindowTitle();
        }

        private void OnWorkspaceChanged()
        {
            if(_workspaceController.CurrentWorkspace != null)
            {
                _currentWorkspace = new WorkspaceViewModel(_workspaceController.CurrentWorkspace);
            }
            else
            {
                _currentWorkspace = null;
            }
            OnPropertyChanged("Workspace"); 
            UpdateWindowTitle();
        }

        #endregion

        #region Public Properties

        public JadeCommandAdaptor Commands { get { return _commands; } }

        public string MainWindowTitle
        {
            get
            {
                if (_workspaceController.WorkspaceOpen)
                {
                    return _workspaceController.CurrentWorkspace.Name + " - Jade IDE";
                }
                return "Jade IDE";
            }            
        }

        public bool DisplayingLineNumbers
        {
            get 
            { 
                return true;
            }
        }

        
        #endregion

        #region EditorControl   

        public EditorControlViewModel Editor
        {
            get { return _editorModel; }
        }

        #endregion

        #region Workspace
        
        public JadeCore.IWorkspaceController WorkspaceController
        {
            get
            {
                return _workspaceController;
            }
        }
        
        #endregion

        #region Editor

        public OutputViewModel Output { get { return _outputModel; } }

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

        public WorkspaceViewModel Workspace
        {
            get { return _currentWorkspace; }
        }
        
        #endregion

        #region Commands

        public void OnOpenDocument(JadeUtils.IO.IFileHandle file)
        {
            _editorController.OpenDocument(file);
        }

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
                    WorkspaceController.NewWorkspace(name);
                }
                catch (Exception e)
                {
                    JadeCore.GuiUtils.DisplayErrorAlert("Error creating new workspace. " + e.ToString());
                }
            }
        }

        public void OnCloseWorkspace()
        {
            WorkspaceController.CloseWorkspace();
        }

        public bool CanCloseWorkspace()
        {
            return WorkspaceController.WorkspaceOpen;
        }

        public void OnPromptOpenWorkspace()
        {
            IFileHandle handle = JadeCore.GuiUtils.PromptOpenFile(".jws", "Jade Workspace files (.jws)|*.jws", true);
            if (handle == null)
            {
                return;
            }

            try
            {
                WorkspaceController.OpenWorkspace(handle);
            }
            catch (Exception e)
            {
                JadeCore.GuiUtils.DisplayErrorAlert("Error opening workspace. " + e.ToString());
            }
        }

        public bool CanPromptOpenWorkspace()
        {
            return true;
        }

        public void OnOpenWorkspace(string path)
        {
            try
            {
                IFileHandle handle = JadeCore.Services.Provider.FileService.MakeFileHandle(path);
                WorkspaceController.OpenWorkspace(handle);
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
            string path = _workspaceController.CurrentWorkspace.Path;
            if (path == null || path.Length == 0)
            {
                if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path) == false)
                    return;
            }
            WorkspaceController.SaveWorkspace(path);
        }

        public bool CanSaveWorkspace()
        {
            return WorkspaceController.RequiresSave;
        }

        public void OnSaveAsWorkspace()
        {
            string path;
            if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path))
            {
                WorkspaceController.SaveWorkspace(path);
            }
        }

        public bool CanSaveAsWorkspace()
        {
            return WorkspaceController.WorkspaceOpen;
        }

        public void OnViewLineNumbers()
        {
        }

        public bool CanViewLineNumbers()
        {
            return true;
        }

        public void OnCloseAllDocuments()
        {
            _editorController.CloseAllDocuments();            
        }

        public bool CanCloseAllDocuments()
        {
            return _editorController.HasOpenDocuments;
        }

        public void OnNewFile()
        {
        }

        public bool CanNewFile()
        {
            return true;
        }

        public void OnOpenFile()
        {
            IFileHandle handle = JadeCore.GuiUtils.PromptOpenFile(".cs", "C# Source files (.cs)|*.cs", true);
            if (handle == null)
            {
                return;
            }

            try
            {
                _editorController.OpenDocument(handle);
            }
            catch (Exception e)
            {
                JadeCore.GuiUtils.DisplayErrorAlert("Error opening file. " + e.ToString());
            }
        }

        public bool CanOpenFile()
        {
            return true;
        }

        public void OnSaveFile()
        {
            _editorController.SaveActiveDocument();
        }

        public bool CanSaveFile()
        {
            return _editorController.HasOpenDocuments && _editorController.ActiveDocument.Modified;
        }

        public void OnSaveAsFile()
        {
            string path;
            if (JadeCore.GuiUtils.PromptSaveFile(".c#", "C# Source files (.cs)|*.cs", "", out path))
            {
                //WorkspaceController.SaveWorkspaceAs(path);
            }
        }

        public bool CanSaveAsFile()
        {
            return _editorController.HasOpenDocuments;
        }

        public void OnSaveAllFiles()
        {
            foreach (var doc in _editorController.ModifiedDocuments)
            {
                doc.Save();
            }
        }

        private bool? PromptSaveFiles(IEnumerable<string> files)
        {
            System.Diagnostics.Debug.Assert(_editorController.ModifiedDocuments.Any());
            JadeControls.Dialogs.SaveFiles dlg = new JadeControls.Dialogs.SaveFiles();
            dlg.DataContext = files;
            return dlg.ShowDialog(_view);
        }    

        public bool CanSaveAllFiles()
        {
            return _editorController.ModifiedDocuments.Any();
        }

        public void OnCloseFile()
        {
            Debug.Assert(_editorController.ActiveDocument != null);

            if (_editorController.ActiveDocument.Modified)
            { 
                List<string> files = new List<string>();
                files.Add(_editorController.ActiveDocument.Path.Str);

                bool? result = PromptSaveFiles(files);

                if (result == null)
                    return;

                if (result == true)
                {
                    _editorController.SaveActiveDocument();
                }
            }
            _editorController.CloseActiveDocument();
        }

        public bool CanCloseFile()
        {
            return _editorController.HasOpenDocuments;
        }

        #endregion

        #region public Methods

        public bool RequestExit()
        {
            if (_editorController.ModifiedDocuments.Any())
            {
                bool? result = PromptSaveFiles(_editorController.ModifiedDocuments.Select(doc => doc.Path.Str));
                if (result == true)
                {
                    //User asked to save files
                    OnSaveAllFiles();
                }
                else if(result == null)
                {
                    //user cancelled
                    return false;
                }
                //if user pressed 'No', ie discard changes, we just continue
            }

            if (_workspaceController.RequiresSave)
            {
                return _workspaceController.SaveOrDiscardWorkspace();
            }
            return true;
        }
       
        #endregion

        #region private Methods

        private void UpdateWindowTitle()
        {
            OnPropertyChanged("MainWindowTitle");
        }

        #endregion
    }
}
