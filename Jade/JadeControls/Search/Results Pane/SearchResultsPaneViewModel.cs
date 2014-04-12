using JadeCore.Search;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using System.Windows.Input;


namespace JadeControls.SearchResultsControl.ViewModel
{
    public class SearchResultsPaneViewModel : JadeControls.Docking.ToolPaneViewModel
    {
        private JadeCore.Search.ISearchController _controller;
        private ObservableCollection<SearchViewModel> _searches;
        private SearchViewModel _currentSearch;
        private bool _seachCurrentDocVisible;

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
            _seachCurrentDocVisible = false;

            JadeCore.Services.Provider.MainWindow.CommandBindings.Add(new CommandBinding(JadeCore.Commands.SearchCurrentFile,
                                        delegate(object target, ExecutedRoutedEventArgs args)
                                        {
                                            SearchCurrentDocument();
                                            args.Handled = true;
                                        },
                                        delegate(object target, CanExecuteRoutedEventArgs args)
                                        {
                                            args.CanExecute = JadeCore.Services.Provider.EditorController.ActiveDocument != null; ;
                                            args.Handled = true;

                                        }));
        }

        private void SearchCurrentDocument()
        {
            _seachCurrentDocVisible = true;
            OnPropertyChanged("CurrentSearchVisibility");
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

        public System.Windows.Visibility CurrentSearchVisibility
        {
            get { return _seachCurrentDocVisible ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed; }
            set { }
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

        public void OnDoubleClick()
        {
            if (CurrentSearch != null)
                CurrentSearch.OnDoubleClick();
        }        
    }
}
