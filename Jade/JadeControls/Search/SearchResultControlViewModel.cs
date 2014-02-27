using JadeCore.Search;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;

namespace JadeControls.SearchResultsControl.ViewModel
{
    public class ResultItemViewModel
    {
        private JadeCore.Search.ISearchResult _result;

        public ResultItemViewModel(JadeCore.Search.ISearchResult result)
        {
            _result = result;
        }

        public string DisplayText
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(_result.Summary);
                sb.Append(" ");
                sb.Append(_result.Path);
                sb.Append(":");
                sb.Append(_result.FileOffset);
                return sb.ToString();
            }
        }
    }

    public class SearchResultsViewModel : JadeControls.Docking.ToolPaneViewModel
    {
        private JadeCore.Search.ISearchController _controller;
        private JadeCore.Collections.ObservableCollectionTransform<ISearchResult, ResultItemViewModel> _items;
        private StringBuilder _sb;

        public SearchResultsViewModel(JadeCore.Search.ISearchController controller)
        {
            Title = "Search Results";
            ContentId = "SearchResultsToolPane";

            _controller = controller;

            ((INotifyCollectionChanged)_controller.Searches).CollectionChanged += SearchResultsViewModel_CollectionChanged;
                        
            _sb = new StringBuilder();
        }

        private void SearchResults_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (Text.Length > 0)
                    _sb.Append(Environment.NewLine);
                _sb.Append(((ResultItemViewModel)e.NewItems[0]).DisplayText);
                
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                _sb = new StringBuilder();
            }
            else 
            {
                RebuildString();
            }
            OnPropertyChanged("Text");
        }

        private void SearchResultsViewModel_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                OnNewSearch((ISearch)e.NewItems[0]);
            }
        }

        private void OnNewSearch(ISearch search)
        {
            _items = new JadeCore.Collections.ObservableCollectionTransform<ISearchResult, ResultItemViewModel>(search.Results,
                delegate(ISearchResult result) { return new ResultItemViewModel(result); });

            _items.CollectionChanged += SearchResults_CollectionChanged;
            OnPropertyChanged("Text");
        }
        
        private void RebuildString()
        {
            _sb = new StringBuilder();
            
            foreach (ResultItemViewModel i in _items)
            {
                if (_sb.Length > 0)
                {
                    _sb.Append(Environment.NewLine);

                }
                _sb.Append(i.DisplayText);
            }
        }

        public ObservableCollection<ResultItemViewModel> Items
        {
            get { return _items; }
        }

        public string Text
        {
            get
            {
                return _sb.ToString();
            }
            set
            {

            }
        }
    }
}
