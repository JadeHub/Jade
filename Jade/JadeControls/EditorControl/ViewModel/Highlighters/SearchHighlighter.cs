using JadeCore.Search;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadeControls.EditorControl.ViewModel
{
    public class SearchHighlighter
    {
        private Highlighting.IHighlighter _highlighter;
        private CppCodeBrowser.IProjectItem _projectItem;
        private ISearchController _searchController;
        private HashSet<Highlighting.IHighlightedRange> _ranges;
        
        public SearchHighlighter(CppCodeBrowser.IProjectItem projectItem, Highlighting.IHighlighter highlighter)
        {
            _highlighter = highlighter;
            _projectItem = projectItem;
            _searchController = JadeCore.Services.Provider.SearchController;

            ((INotifyCollectionChanged)_searchController.Searches).CollectionChanged += Searchs_CollectionChanged;
            _ranges = new HashSet<Highlighting.IHighlightedRange>();

            if(_searchController.Current != null)
            {
                OnNewSearch(_searchController.Current);
            }
        }

        private void Searchs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                OnNewSearch((ISearch)e.NewItems[0]);
            }
        }

        private void OnNewSearch(ISearch search)
        {
            RemoveAllRanges();
            ((INotifyCollectionChanged)search.Results).CollectionChanged += Results_CollectionChanged;
            foreach(ISearchResult result in search.Results)
            {
                OnNewResult(result);
            }
        }

        private void Results_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                ISearchResult result = (ISearchResult)e.NewItems[0];
                OnNewResult(result);
            }
        }

        private void OnNewResult(ISearchResult result)
        {
            if (result.Location.Path == _projectItem.Path)
            {
                Highlighting.IHighlightedRange hr = _highlighter.AddRange(result.Location.Offset, result.Extent);
                hr.ForegroundColour = System.Windows.Media.Colors.Blue;
                _ranges.Add(hr);
            }
        }
        
        private void RemoveAllRanges()
        { 
            foreach(Highlighting.IHighlightedRange range in _ranges)
            {
                _highlighter.RemoveRange(range);
            }
            _ranges.Clear();
        }
    }
}
