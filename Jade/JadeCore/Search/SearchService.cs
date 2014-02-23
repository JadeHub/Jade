using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadeCore.Search
{
    public interface ISearchController
    {
        void RegisterSearch(ISearch search);
        void RemoveSearch(ISearch search);

        ReadOnlyObservableCollection<ISearch> Searches { get; }
    }

    public class SearchController : ISearchController
    {
        #region Data

        private ObservableCollection<ISearch> _searches;
        private ReadOnlyObservableCollection<ISearch> _readonlySearches;

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

        #endregion

        #region Public Methods

        public void RegisterSearch(ISearch search)
        {
            _searches.Add(search);
        }

        public void RemoveSearch(ISearch search)
        {
            _searches.Remove(search);
        }

        #endregion
    }
}
