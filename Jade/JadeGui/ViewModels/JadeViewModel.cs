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
    using JadeControls.SymbolInspector;
    using JadeControls.CursorInspector;
    using JadeControls.ContextTool;
    using JadeUtils.IO;
    
    /// <summary>
    /// Main View Model class. Singleton instance that lives the life of the application
    /// </summary>
    internal class JadeViewModel : JadeControls.NotifyPropertyChanged , JadeCore.IJadeCommandHandler, IDisposable
    {
        #region Data

        //Command adaptor
        private JadeCommandAdaptor _commands;

        //Workspace
        private JadeCore.Workspace.IWorkspaceController _workspaceController;
        private WorkspaceViewModel _currentWorkspace;
        
        //Editor
        private EditorControlViewModel _editorViewModel;
        private JadeCore.IEditorController _editorController;

        //Output window
        private OutputViewModel _outputViewModel;

        //Search results window
        private JadeCore.Search.ISearchController _searchController;
        private SearchResultsPaneViewModel _seachResultsViewModel;
        
        //Symbol Inspector window
        private SymbolInspectorPaneViewModel _symbolInspectorViewModel;

        private CursorInspectorPaneViewModel _cursorInspectorViewModel;

        private ContextPaneViewModel _contextPaneViewModel;

        //Main window
        private DockingGui.MainWindow _view;

        //Set of tool windows
        private ObservableCollection<JadeControls.Docking.ToolPaneViewModel> _toolWindows;

        #endregion

        #region Contsructor

        public JadeViewModel(DockingGui.MainWindow view)
        {
            _workspaceController = JadeCore.Services.Provider.WorkspaceController;
            //Todo - Workspace viewmodel to track WorkspaceController changes
            _workspaceController.WorkspaceChanged += delegate { OnWorkspaceChanged(); };
            _currentWorkspace = new WorkspaceViewModel();
            
            _editorController = JadeCore.Services.Provider.EditorController;
            _editorController.ActiveDocumentChanged += OnEditorControllerActiveDocumentChanged;
            _editorViewModel = new JadeControls.EditorControl.ViewModel.EditorControlViewModel(_editorController, new JadeControls.EditorControl.ViewModel.DocumentViewModelFactory());

            _outputViewModel = new OutputViewModel(JadeCore.Services.Provider.OutputController);

            _searchController = JadeCore.Services.Provider.SearchController;
            _seachResultsViewModel = new SearchResultsPaneViewModel(_searchController);

            _symbolInspectorViewModel = new SymbolInspectorPaneViewModel(_editorController);

            _cursorInspectorViewModel = new CursorInspectorPaneViewModel();

            _contextPaneViewModel = new ContextPaneViewModel(_editorController);

            _commands = new JadeCommandAdaptor(this);
            _view = view;

            _toolWindows = new ObservableCollection<JadeControls.Docking.ToolPaneViewModel>();                        
            _toolWindows.Add(_seachResultsViewModel);
            _toolWindows.Add(_outputViewModel);
            _toolWindows.Add(_currentWorkspace);
            _toolWindows.Add(_symbolInspectorViewModel);
            _toolWindows.Add(_cursorInspectorViewModel);
            _toolWindows.Add(_contextPaneViewModel);
            OnViewWorkspaceWindow();

            UpdateWindowTitle();
        }

        public void Dispose()
        {
            _editorController.Dispose();
        }

        public void LoadMainWindowLayout()
        {
            DockingGui.LayoutReaderWriter.Read(_view.dockManager, "loutout.dat");
        }

        public void SaveMainWindowLayout()
        {
            DockingGui.LayoutReaderWriter.Write(_view.dockManager, "loutout.dat");
        }

        private void OnEditorControllerActiveDocumentChanged(JadeCore.IEditorDoc newValue, JadeCore.IEditorDoc oldValue)
        {
            DocumentViewModel doc = _editorViewModel.FindViewModel(newValue);
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
            OnViewWorkspaceWindow();            
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

        public void OnViewWorkspaceWindow()
        {
            _currentWorkspace.IsVisible = true;
            _currentWorkspace.IsSelected = true;
        }

        public bool CanViewWorkspaceWindow()
        {
            return _currentWorkspace != null;
        }

        public void OnViewSearchResultsWindow()
        {
            _seachResultsViewModel.IsVisible = true;
            _seachResultsViewModel.IsSelected = true;
        }

        public bool CanViewSearchResultsWindow()
        {
            return _seachResultsViewModel != null;
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
            System.Diagnostics.Debug.Assert(_editorController.HasModifiedDocuments);
            JadeControls.Dialogs.SaveFiles dlg = new JadeControls.Dialogs.SaveFiles();
            dlg.DataContext = files;
            return dlg.ShowDialog(_view);
        }    

        public bool CanSaveAllFiles()
        {
            return _editorController.HasModifiedDocuments;
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
            if (_editorController.HasModifiedDocuments)
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

        public void OnDisplaySymbolInspector(JadeCore.CppSymbols.ISymbolCursor symbol)
        {
            _symbolInspectorViewModel.SymbolCursor = symbol;
            _symbolInspectorViewModel.IsSelected = true;
            _symbolInspectorViewModel.IsVisible = true;
        }

        public void OnDisplayCursorInspector(LibClang.Cursor c)
        {
            _cursorInspectorViewModel.SetCursor(c);
            _cursorInspectorViewModel.IsSelected = true;
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
