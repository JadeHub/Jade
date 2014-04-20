using JadeCore.Search;
using System;
using System.Diagnostics;
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
        private ISearch _currentSearch;
        private Highlighting.IHighlightedRange _currentResultRange;
                
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
            if(_currentSearch != null)
            {
                ((INotifyCollectionChanged)_currentSearch.Results).CollectionChanged -= Results_CollectionChanged;
                _currentSearch.CurrentResultChanged -= SearchCurrentResultChanged;
            }

            _currentSearch = search;
            ((INotifyCollectionChanged)_currentSearch.Results).CollectionChanged += Results_CollectionChanged;
            _currentSearch.CurrentResultChanged += SearchCurrentResultChanged;
            foreach (ISearchResult result in _currentSearch.Results)
            {
                OnNewResult(result);
            }
            SearchCurrentResultChanged(this, EventArgs.Empty);
        }

        private void RemoveCurrentResultHighlight()
        {
            if (_currentResultRange != null)
            {
                _currentResultRange.BackgroundColour = System.Windows.Media.Colors.LemonChiffon;
                _highlighter.Redraw(_currentResultRange);
                _currentResultRange = null;
            }
        }

        private void AddCurrentResultHighlight()
        {
            if (_currentResultRange == null)
            {
                _currentResultRange = FindRange(_currentSearch.CurrentResult);
                if (_currentResultRange == null)
                    _currentResultRange = _highlighter.AddRange(_currentSearch.CurrentResult.Location.Offset, _currentSearch.CurrentResult.Extent);
                _currentResultRange.BackgroundColour = System.Windows.Media.Colors.Cyan;
                _highlighter.Redraw(_currentResultRange);
            }
        }

        private bool HighlighCurrentResult()
        {
            return _currentSearch.CurrentResult != null && _currentSearch.CurrentResult.Location.Path == _projectItem.Path;
        }

        private void SearchCurrentResultChanged(object sender, EventArgs e)
        {
            RemoveCurrentResultHighlight();
                        
            if (HighlighCurrentResult())
            {                
                AddCurrentResultHighlight();
            }
        }

        private void Results_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                ISearchResult result = (ISearchResult)e.NewItems[0];
                OnNewResult(result);
            }
            else if(e.Action == NotifyCollectionChangedAction.Reset)
            {
                RemoveAllRanges();
            }
        }

        private void OnNewResult(ISearchResult result)
        {
            if (result.Location.Path == _projectItem.Path)
            {
                Highlighting.IHighlightedRange hr = _currentResultRange = FindRange(result);
                if(hr == null)
                    hr = _highlighter.AddRange(result.Location.Offset, result.Extent);
                hr.BackgroundColour = System.Windows.Media.Colors.LemonChiffon;
                _ranges.Add(hr);
            }
        }
        
        private void RemoveAllRanges()
        {
            RemoveCurrentResultHighlight();
            foreach(Highlighting.IHighlightedRange range in _ranges)
            {
                _highlighter.RemoveRange(range);
                _highlighter.Redraw(range);                
            }
            _ranges.Clear();
        }

        public void Clear()
        {
            RemoveAllRanges();
        }

        private Highlighting.IHighlightedRange FindRange(ISearchResult result)
        {
            foreach(Highlighting.IHighlightedRange range in _ranges)
            {
                if (range.Offset == result.Location.Offset && range.Length == result.Extent)
                    return range;
            }
            return null;
        }
    }
}
