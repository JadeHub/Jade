using JadeCore.Search;
using System;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using System.Windows.Input;
using JadeCore;


namespace JadeControls.SearchResultsControl.ViewModel
{
    public class CurrentFileTextSearchViewModel : NotifyPropertyChanged
    {
        private TextDocumentSearch _currentFileTextSearch;
        private string _currenttFileTextSearchStr;
        private JadeCore.IEditorController _editorController;

        private JadeCore.Search.ISearchController _controller;

        public CurrentFileTextSearchViewModel(JadeCore.Search.ISearchController controller)
        {
            _editorController = JadeCore.Services.Provider.EditorController;
            _controller = controller;            
        }

        public string SearchString
        {
            get
            {
                return _currenttFileTextSearchStr;
            }
            set
            {
                if (_currenttFileTextSearchStr != value)
                {
                    _currenttFileTextSearchStr = value;
                    SearchCurrentDocument();
                }
            }
        }

        private void SearchCurrentDocument()
        {
            JadeCore.IEditorDoc doc = _editorController.ActiveDocument;
            if (doc == null) return;

            if (_currentFileTextSearch == null || _currentFileTextSearch.Document != doc.TextDocument)
            {
                _currentFileTextSearch = new JadeCore.Search.TextDocumentSearch(doc.TextDocument);
                _controller.RegisterSearch(_currentFileTextSearch);
                _currentFileTextSearch.Start();
            }
            _currentFileTextSearch.Pattern = _currenttFileTextSearchStr;
        }

        public bool CanPerformSearch
        {
            get { return JadeCore.Services.Provider.EditorController.ActiveDocument != null; }
        }
    }

    public class SearchResultsPaneViewModel : JadeControls.Docking.ToolPaneViewModel
    {
        private JadeCore.Search.ISearchController _controller;
        private ObservableCollection<SearchViewModel> _searches;
        private SearchViewModel _currentSearch;
        private CurrentFileTextSearchViewModel _currentFileTextSearch;

        public event EventHandler StartNewCurrentFileSearch;

        private void RaiseStartNewCurrentFileSearch()
        {
            EventHandler h = StartNewCurrentFileSearch;
            if(h != null)
                h(this, EventArgs.Empty);
        }

        public SearchResultsPaneViewModel(JadeCore.Search.ISearchController controller)
        {
            Title = "Search Results";
            ContentId = "SearchResultsToolPane";
            _controller = controller;
            _searches = new ObservableCollection<SearchViewModel>();
            ((INotifyCollectionChanged)_controller.Searches).CollectionChanged +=
                delegate(object sender, NotifyCollectionChangedEventArgs e)
                {
                    if (e.Action == NotifyCollectionChangedAction.Add)
                    {
                        OnNewSearch((ISearch)e.NewItems[0]);
                    }
                };

            _currentFileTextSearch = new CurrentFileTextSearchViewModel(_controller);

            JadeCore.Services.Provider.EditorController.ActiveDocumentChanged += delegate { OnPropertyChanged("CanPerformSearchInCurrentFile"); };

            JadeCore.Services.Provider.MainWindow.CommandBindings.Add(new CommandBinding(JadeCore.Commands.SearchCurrentFile,
                                        delegate(object target, ExecutedRoutedEventArgs args)
                                        {
                                            RaiseStartNewCurrentFileSearch();
                                            args.Handled = true;
                                        },
                                        delegate(object target, CanExecuteRoutedEventArgs args)
                                        {
                                            args.CanExecute = CanPerformSearchInCurrentFile;
                                            args.Handled = true;

                                        }));
        }

        public bool CanPerformSearchInCurrentFile
        {
            //update
            get { return _currentFileTextSearch.CanPerformSearch; }
        }

        private void OnNewSearch(ISearch search)
        {
            SearchViewModel searchViewModel = new SearchViewModel(search);
            
            _searches.Add(searchViewModel);
            CurrentSearch = searchViewModel;            
        }
        
        public ObservableCollection<SearchViewModel> Searches
        {
            get { return _searches; }
            
        }

        public SearchViewModel CurrentSearch
        {
            get { return _currentSearch; }
            set 
            {
                if(value != _currentSearch)
                {
                    _currentSearch = value;
                    _controller.Current = _currentSearch.Search;
                    OnPropertyChanged("CurrentSearch");
                }
            }
        }

        public string SearchString
        {
            get 
            {
                return _currentFileTextSearch.SearchString;
            }
            set
            {
                _currentFileTextSearch.SearchString = value;
            }
        }

        public void OnDoubleClick()
        {
            if (CurrentSearch != null)
                CurrentSearch.OnDoubleClick();
        }        
    }
}
