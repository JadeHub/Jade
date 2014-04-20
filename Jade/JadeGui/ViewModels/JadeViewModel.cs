using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace JadeGui.ViewModels
{
    using JadeControls.EditorControl.ViewModel;
    using JadeControls.OutputControl.ViewModel;
    using JadeControls.SearchResultsControl.ViewModel;
    using JadeControls.Workspace.ViewModel;
    using JadeUtils.IO;
    
    /// <summary>
    /// Main View Model class. Singleton instance that lives the life of the application
    /// </summary>
    internal class JadeViewModel : JadeControls.NotifyPropertyChanged , JadeCore.IJadeCommandHandler
    {
        #region Data

        private JadeCommandAdaptor _commands;

        private JadeCore.Workspace.IWorkspaceController _workspaceController;
        private WorkspaceViewModel _currentWorkspace;
        
        private EditorControlViewModel _editorViewModel;
        private JadeCore.IEditorController _editorController;

        private JadeCore.Output.IOutputController _outputController;
        private OutputViewModel _outputViewModel;

        private JadeCore.Search.ISearchController _searchController;
        private SearchResultsPaneViewModel _seachResultsViewModel;

        private Window _view;

        private ObservableCollection<JadeControls.Docking.ToolPaneViewModel> _toolWindows;

        #endregion

        #region Contsructor

        public JadeViewModel(Window view)
        {
            _workspaceController = JadeCore.Services.Provider.WorkspaceController;
            //Todo - Workspace viewmodel to track WorkspaceController changes
            _workspaceController.WorkspaceChanged += delegate { OnWorkspaceChanged(); };
            _currentWorkspace = new WorkspaceViewModel();
            
            _editorController = JadeCore.Services.Provider.EditorController;
            _editorController.DocumentSelected += OnEditorControllerDocumentSelected;
            _editorViewModel = new JadeControls.EditorControl.ViewModel.EditorControlViewModel(_editorController);

            _outputController = JadeCore.Services.Provider.OutputController;
            _outputViewModel = new OutputViewModel(_outputController);

            _searchController = JadeCore.Services.Provider.SearchController;
            _seachResultsViewModel = new SearchResultsPaneViewModel(_searchController);

            _commands = new JadeCommandAdaptor(this);
            _view = view;

            _toolWindows = new ObservableCollection<JadeControls.Docking.ToolPaneViewModel>();                        
            _toolWindows.Add(_seachResultsViewModel);
            _toolWindows.Add(_outputViewModel);
            _toolWindows.Add(_currentWorkspace);
            _currentWorkspace.IsVisible = false;

            UpdateWindowTitle();
        }

        void OnEditorControllerDocumentSelected(JadeCore.EditorDocChangeEventArgs args)
        {
            DocumentViewModel doc = _editorViewModel.GetViewModel(args.Document);
            if (doc != null)
            {
                ActiveDockContent = doc;
            }
        }

        private void OnWorkspaceChanged()
        {
            _currentWorkspace.Data = _workspaceController.CurrentWorkspace;
                        
            OnPropertyChanged("Workspace");
            OnPropertyChanged("ToolWindows");
            
            UpdateWindowTitle();
        }

        #endregion

        #region Public Properties

        #region Docking Windows

        public ObservableCollection<JadeControls.Docking.ToolPaneViewModel> ToolWindows
        {
            get
            {
                
                return _toolWindows;
            }
        }

        private object _activeDockContent;

        public object ActiveDockContent
        {
            get
            {
                return _activeDockContent;
            }

            set
            {
                if (_activeDockContent != value)
                {
                    _activeDockContent = value;

                    if (_activeDockContent is DocumentViewModel)
                    {
                        DocumentViewModel doc = _activeDockContent as DocumentViewModel;
                        if(doc != Editor.SelectedDocument)
                            Editor.SelectedDocument = _activeDockContent as DocumentViewModel;
                    }
                    OnPropertyChanged("ActiveDockContent");
                }
            }
        }

        #endregion
          

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
            get { return _editorViewModel; }
        }

        #endregion

        #region Editor

        public OutputViewModel Output { get { return _outputViewModel; } }

        public SearchResultsPaneViewModel SearchResults { get { return _seachResultsViewModel; } }

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
        /*
        public WorkspaceViewModel Workspace
        {
            get { return _currentWorkspace; }
        }*/
        /*
        private JadeCore.Workspace.IWorkspaceController WorkspaceController
        {
            get
            {
                return _workspaceController;
            }
        }*/
        
        #endregion

        #region Commands

        public void OnOpenDocument(JadeCore.OpenFileCommandParams param)
        {
            IInputElement focus = FocusManager.GetFocusedElement(_view);
            
            OnOpenFile(param.File);

            //todo: if selected is now as requested...

            if (_editorViewModel.SelectedDocument != null)
                _editorViewModel.SelectedDocument.DisplayLocation(param.DisplayParams.Location.Offset, param.DisplayParams.SetFocus, param.DisplayParams.Scroll);

            if (!param.DisplayParams.SetFocus)
                FocusManager.SetFocusedElement(_view, focus);
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
                    _workspaceController.NewWorkspace(name);
                }
                catch (Exception e)
                {
                    JadeCore.GuiUtils.DisplayErrorAlert("Error creating new workspace. " + e.ToString());
                }
            }
        }

        public void OnCloseWorkspace()
        {
            _workspaceController.CloseWorkspace();
            //_editorController.Reset();
        }

        public bool CanCloseWorkspace()
        {
            return _workspaceController.WorkspaceOpen;
        }

        public void OnPromptOpenWorkspace()
        {
            string filter = "Solution files types|*.sln;*.jws";

            IFileHandle handle = JadeCore.GuiUtils.PromptOpenFile("", filter, true);
            if (handle == null)
            {
                return;
            }

            try
            {
                _workspaceController.OpenWorkspace(handle.Path);
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
                _workspaceController.OpenWorkspace(FilePath.Make(path));

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
            _workspaceController.SaveWorkspace(path);
        }

        public bool CanSaveWorkspace()
        {
            return _workspaceController.RequiresSave;
        }

        public void OnSaveAsWorkspace()
        {
            string path;
            if (JadeCore.GuiUtils.PromptSaveFile(".jws", "Jade Workspace files (.jws)|*.jws", "", out path))
            {
                _workspaceController.SaveWorkspace(path);
            }
        }

        public bool CanSaveAsWorkspace()
        {
            return _workspaceController.WorkspaceOpen;
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

        public void OnOpenFile(IFileHandle handle)
        {
            if(handle == null)
                handle = JadeCore.GuiUtils.PromptOpenFile(".cs", "C# Source files (.cs)|*.cs", true);

            if (handle == null)
                return;

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
                _workspaceController.SaveWorkspace(path);
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
                files.Add(_editorController.ActiveDocument.File.ToString());

                bool? result = PromptSaveFiles(files);

                if (result == null)
                    return;

                if (result == true)
                {
                    _editorController.SaveActiveDocument();
                }
                else
                {
                    _editorController.DiscardChangesToActiveDocument();
                }
            }
            _editorController.CloseActiveDocument();
        }

        public bool CanCloseFile()
        {
            return _editorController.HasOpenDocuments;
        }

        public void OnDisplayCodeLocation(JadeCore.DisplayCodeLocationCommandParams param)
        {
            IFileHandle f = JadeCore.Services.Provider.FileService.MakeFileHandle(param.Location.Path);
            OnOpenDocument(new JadeCore.OpenFileCommandParams(f, param));
        }
         
        public void OnSearchInFiles()
        {
        }

        public bool CanSearchInFiles()
        {
            return true;
        }

        #endregion

        #region public Methods

        public bool RequestExit()
        {
            if (_editorController.ModifiedDocuments.Any())
            {
                bool? result = PromptSaveFiles(_editorController.ModifiedDocuments.Select(doc => doc.File.ToString()));
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

        public void OnDisplayNextSearchResult()
        {
            if (_searchController.Current != null)
            {
                JadeCore.Search.ISearch search = _searchController.Current;
                search.MoveToNextResult();
                if(search.CurrentResult != null)
                    OnDisplayCodeLocation(new JadeCore.DisplayCodeLocationCommandParams(search.CurrentResult.Location, true, true));
            }
        }

        public bool CanDisplayNextSearchResult()
        {
            return _searchController.Current != null && _searchController.Current.Results.Count > 1;
        }

        public void OnDisplayPrevSearchResult()
        {
            if (_searchController.Current != null)
            {
                JadeCore.Search.ISearch search = _searchController.Current;
                search.MoveToPreviousResult();
                if (search.CurrentResult != null)
                    OnDisplayCodeLocation(new JadeCore.DisplayCodeLocationCommandParams(search.CurrentResult.Location, true, true));
            }
        }
        public bool CanDisplayPrevSearchResult()
        {
            return _searchController.Current != null && _searchController.Current.Results.Count > 1;
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
