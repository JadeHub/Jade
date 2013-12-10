using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Diagnostics;

namespace JadeControls.EditorControl.ViewModel
{
    using JadeCore;

    /// <summary>
    /// View model for the entire Editor Control. Manages the collection of tabs and associated DocumentViewModels
    /// </summary>
    public class EditorControlViewModel : JadeControls.NotifyPropertyChanged
    {
        #region Data

        private ObservableCollection<EditorTabItem> _tabItems;

        private EditorTabItem _selectedDocumentTab;
        private JadeCore.IEditorController _controller;
        private EditorControlCommandAdaptor _commands;
        private DocumentViewModel _selectedDocument;

        #endregion

        #region Constructor

        public EditorControlViewModel(JadeCore.IEditorController controller)
        {
            //Bind to the Model
            _controller = controller;
            _controller.DocumentOpened += OnControllerDocumentOpened;
            _controller.DocumentSelected += OnControllerDocumentSelected;

            //Setup Command Adaptor
            _commands = new EditorControlCommandAdaptor(this);
            _tabItems = new ObservableCollection<EditorTabItem>();            
        }

        #endregion

        #region Model event handlers

        private void OnDocumentClosing(EditorTabItem tabItem)
        {
            if (_tabItems.Contains(tabItem))
            {
                _tabItems.Remove(tabItem);
            }
        }

        private void OnControllerDocumentSelected(JadeCore.EditorDocChangeEventArgs args)
        {
            EditorTabItem tabItem = GetTabItem(args.Document);
            if (tabItem != null)
            {
                SelectedDocumentTab = tabItem;
            }
        }

        private void OnControllerDocumentOpened(JadeCore.EditorDocChangeEventArgs args)
        {
            EditorTabItem view = new EditorTabItem();

            ISourceBrowserStrategy browserStrategy = new SourceBrowserStrategy(GetProjectSourceIndex());

            DocumentViewModel d = new SourceDocumentViewModel(args.Document, view.CodeEditor, browserStrategy);
            view.DataContext = d;
            _tabItems.Add(view);
            args.Document.OnClosing += delegate { OnDocumentClosing(view); };
            SelectedDocumentTab = view;
            OnPropertyChanged("TabItems");
            //hack to preload the document so we can immediatly set the cursor location
            view.CodeEditor.Document = d.TextDocument;
        }

        #endregion

        #region Public Properties

        public EditorControlCommandAdaptor Commands { get { return _commands; } }                
        public ObservableCollection<EditorTabItem> TabItems { get { return _tabItems; } }

        public EditorTabItem SelectedDocumentTab
        {
            get { return _selectedDocumentTab; }
            set
            {
                if (_selectedDocumentTab != null)
                {
                    DocumentViewModel vm = _selectedDocumentTab.DataContext as DocumentViewModel;
                    vm.Selected = false;
                }
                _selectedDocumentTab = value;
                if (_selectedDocumentTab != null)
                {
                    DocumentViewModel vm = _selectedDocumentTab.DataContext as DocumentViewModel;
                    vm.Selected = true;
                    SelectedDocument = vm;
                    _controller.ActiveDocument = vm.Document;

                }
                else
                {
                    SelectedDocument = null;
                    _controller.ActiveDocument = null;
                }
                
                OnPropertyChanged("SelectedDocumentTab");
            }
        }

        public DocumentViewModel SelectedDocument
        {
            get { return _selectedDocument; }
            private set
            {
                if (_selectedDocument != value)
                {
                    _selectedDocument = value;
                    OnPropertyChanged("SelectedDocument");
                }
            }
        }

        #endregion

        #region Command Handlers

        public void OnClose(object param)
        {
            if(param != null && param is DocumentViewModel)
            {
                DocumentViewModel doc = param as DocumentViewModel;
            }
        }
                
        public bool CanDoClose()
        {
            return true;
        }

        #endregion

        #region Private Methods

        private EditorTabItem GetTabItem(JadeCore.IEditorDoc doc)
        {
            foreach (EditorTabItem item in _tabItems)
            {
                if (GetDocument(item) == doc)
                {
                    return item;
                }
            }
            return null;
        }

        private JadeCore.IEditorDoc GetDocument(EditorTabItem tabItem)
        {
            DocumentViewModel vm = tabItem.DataContext as DocumentViewModel;
            return vm.Document;
        }

        private CppView.IProjectSourceIndex GetProjectSourceIndex()
        {
            if (Services.Provider.WorkspaceController.CurrentWorkspace != null &&
                Services.Provider.WorkspaceController.CurrentWorkspace.ActiveProject != null)
            {
                return Services.Provider.WorkspaceController.CurrentWorkspace.ActiveProject.SourceIndex;
            }
            return null;
        }

        #endregion
    }
}
