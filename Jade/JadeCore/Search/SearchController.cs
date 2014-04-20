using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadeCore.Search
{   
    public class SearchController : ISearchController
    {
        #region Data

        private ObservableCollection<ISearch> _searches;
        private ReadOnlyObservableCollection<ISearch> _readonlySearches;
        private ISearch _currentSearch;

        #endregion

        public SearchController()
        {
            _searches = new ObservableCollection<ISearch>();
            _readonlySearches = new ReadOnlyObservableCollection<ISearch>(_searches);
        }

        #region Properties

        public ReadOnlyObservableCollection<ISearch> Searches
        {
            get { return _readonlySearches; }
        }

        public ISearch Current 
        {
            get { return _currentSearch; }
            set
            {
                if(value != _currentSearch)
                {
                    _currentSearch = value;
                }
            }
        }

        #endregion

        #region Public Methods

        public void RegisterSearch(ISearch search)
        {
            _searches.Add(search);
            _currentSearch = search;
        }

        public void RemoveSearch(ISearch search)
        {
            _searches.Remove(search);
        }

        #endregion
    }
}
