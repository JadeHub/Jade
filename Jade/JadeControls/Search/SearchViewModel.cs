using JadeCore.Search;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JadeUtils.IO;

namespace JadeControls.SearchResultsControl.ViewModel
{
    public class SearchViewModel : NotifyPropertyChanged
    {
        private ISearch _search;

        private HashSet<Tuple<int, int>> _uniqueLocations;
        private ObservableCollection<SearchResultItemViewModel> _items;

        private SearchResultItemViewModel _selectedItem;

        public SearchViewModel(ISearch search)
        {
            _search = search;
            _uniqueLocations = new HashSet<Tuple<int, int>>();
            ((INotifyCollectionChanged)_search.Results).CollectionChanged += OnResultsCollectionChanged;
            _search.CurrentResultChanged += Search_CurrentResultChanged;
            _items = new ObservableCollection<SearchResultItemViewModel>();
            OnPropertyChanged("Items");

        }

        void Search_CurrentResultChanged(object sender, EventArgs e)
        {
            
        }

        private void OnResultsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    for (int i = 0; i < e.NewItems.Count; i++)
                    {                        
                        OnNewResult(e.NewItems[i] as ISearchResult);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    // Remove the item from our collection
                    for (int i = 0; i < e.OldItems.Count; i++)
                    {
                        //this.RemoveAt(e.OldStartingIndex + i);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    _items.Clear();
                    break;
                default:
                    throw new NotSupportedException(e.Action.ToString());
            }
        }

        private void OnNewResult(ISearchResult result)
        {
            SearchResultItemViewModel itemVm = new SearchResultItemViewModel(result);
            Tuple<IFileHandle, int> line = new Tuple<IFileHandle, int>(itemVm.File, itemVm.LineNum);
            if (_uniqueLocations.Add(new Tuple<int, int>(itemVm.Result.Location.Offset, itemVm.Result.Extent)))
                _items.Add(itemVm);
        }

        public ObservableCollection<SearchResultItemViewModel> Items
        {
            get { return _items; }
        }

        public ISearch Search
        {
            get { return _search; }
        }

        public SearchResultItemViewModel SelectedItem
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

        private void OnItemSelected(bool setFocus)
        {
            JadeUtils.ArgChecking.ThrowIfNull(_selectedItem, "SelectedItem");

            _search.CurrentResult = _selectedItem.Result;
            JadeCore.IJadeCommandHandler cmdHandler = JadeCore.Services.Provider.CommandHandler;
            cmdHandler.OnDisplayCodeLocation(new JadeCore.DisplayCodeLocationCommandParams(_selectedItem.Result.Location, setFocus, true));
        }

        public void OnDoubleClick()
        {
            if (_selectedItem != null)
            {
                OnItemSelected(true);
            }
        }

        public string Summary
        {
            get { return _search.Summary; }
        }

        public override string ToString()
        {
            return Summary;
        }
    }
}
