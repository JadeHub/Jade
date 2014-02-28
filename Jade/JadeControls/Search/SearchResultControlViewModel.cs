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
                sb.Append(_result.ToString());
                return sb.ToString();
            }
        }

        public JadeCore.Search.ISearchResult Result
        {
            get { return _result; }
        }

        public override string ToString()
        {
            return DisplayText;
        }
    }

    public class SearchResultsViewModel : JadeControls.Docking.ToolPaneViewModel
    {
        private JadeCore.Search.ISearchController _controller;
        private JadeCore.Collections.ObservableCollectionTransform<ISearchResult, ResultItemViewModel> _items;
        private ResultItemViewModel _selectedItem;

        public SearchResultsViewModel(JadeCore.Search.ISearchController controller)
        {
            Title = "Search Results";
            ContentId = "SearchResultsToolPane";
            _controller = controller;
            ((INotifyCollectionChanged)_controller.Searches).CollectionChanged += SearchResultsViewModel_CollectionChanged;
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

            OnPropertyChanged("Items");
        }
        
        public ObservableCollection<ResultItemViewModel> Items
        {
            get { return _items; }
        }

        public ResultItemViewModel SelectedItem
        {
            get
            {
                return _selectedItem;
            }

            set
            {
                if (value != _selectedItem)
                {
                    _selectedItem = value;

                    if (_selectedItem != null)
                    {
                        OnItemSelected(false);
                    }
                    OnPropertyChanged("SelectedItem");
                }
            }
        }

        public void OnDoubleClick()
        {
            if(_selectedItem != null)
            {
                OnItemSelected(true);
            }
        }

        private void OnItemSelected(bool setFocus)
        {
            JadeUtils.ArgChecking.ThrowIfNull(_selectedItem, "SelectedItem");
            
            JadeCore.IJadeCommandHandler cmdHandler = JadeCore.Services.Provider.CommandHandler;
            cmdHandler.OnDisplayCodeLocation(new JadeCore.DisplayCodeLocationParams(_selectedItem.Result.Location, setFocus));
        }
    }
}
