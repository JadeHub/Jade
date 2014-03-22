using JadeCore.Search;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;

namespace JadeControls.SearchResultsControl.ViewModel
{
    public class SearchResultsPaneViewModel : JadeControls.Docking.ToolPaneViewModel
    {
        private JadeCore.Search.ISearchController _controller;
        private ObservableCollection<SearchViewModel> _searches;
        private SearchViewModel _currentSearch;

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

        public void OnDoubleClick()
        {
            if (CurrentSearch != null)
                CurrentSearch.OnDoubleClick();
        }        
    }
}
