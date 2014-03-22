using System;
using System.Collections.ObjectModel;

namespace JadeCore.Search
{
    public enum SearchState
    {
        Searching,
        Complete,
        Canceled
    }

    public interface ISearch
    {
        event EventHandler CurrentResultChanged;

        /// <summary>
        /// Summary text to be used in display. eg 'Find "abcd" in test.cpp'
        /// </summary>
        string Summary { get; }
                
        /// <summary>
        /// Cancel the search operation.
        /// </summary>
        void Cancel();

        /// <summary>
        /// Start the search operation.
        /// </summary>
        void Start();

        /// <summary>
        /// Search operation result set.
        /// </summary>        
        ReadOnlyObservableCollection<ISearchResult> Results { get; }

        /// <summary>
        /// Currrent result, if any.
        /// </summary>
        ISearchResult CurrentResult { get; set; }

        void MoveToNextResult();
        void MoveToPreviousResult();
    }
}
