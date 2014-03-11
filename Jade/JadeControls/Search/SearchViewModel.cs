using JadeCore.Search;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadeControls.SearchResultsControl.ViewModel
{
    public class SearchViewModel : NotifyPropertyChanged
    {
        private ISearch _search;
        private JadeCore.Collections.ObservableCollectionTransform<ISearchResult, SearchResultItemViewModel> _items;
        private SearchResultItemViewModel _selectedItem;

        public SearchViewModel(ISearch search)
        {
            _search = search;
            _items = new JadeCore.Collections.ObservableCollectionTransform<ISearchResult, SearchResultItemViewModel>(_search.Results,
                delegate(ISearchResult result) { return new SearchResultItemViewModel(result); });

            OnPropertyChanged("Items");
        }

        public ObservableCollection<SearchResultItemViewModel> Items
        {
            get { return _items; }
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

            JadeCore.IJadeCommandHandler cmdHandler = JadeCore.Services.Provider.CommandHandler;
            cmdHandler.OnDisplayCodeLocation(new JadeCore.DisplayCodeLocationParams(_selectedItem.Result.Location, setFocus));
        }

        public void OnDoubleClick()
        {
            if (_selectedItem != null)
            {
                OnItemSelected(true);
            }
        }
    }
}
