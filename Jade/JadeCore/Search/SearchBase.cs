using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace JadeCore.Search
{
    public abstract class SearchBase : ISearch
    {
        private bool _searching;
        private ObservableCollection<ISearchResult> _results;
        private ISearchResult _currentResult;

        internal SearchBase()
        {
            _searching = false;
            _results = new ObservableCollection<ISearchResult>();
            Results = new ReadOnlyObservableCollection<ISearchResult>(_results);
        }

        #region Events

        public event EventHandler CurrentResultChanged;
        public event EventHandler FilterChanged;

        protected void RaiseFilterChangedEvent()
        {
            EventHandler h = FilterChanged;
            if (h != null)
                h(this, EventArgs.Empty);
        }

        private void RaiseCurrentResultChangedEvent()
        {
            EventHandler h = CurrentResultChanged;
            if (h != null)
                h(this, EventArgs.Empty);
        }

        #endregion

        #region Properties

        public ReadOnlyObservableCollection<ISearchResult> Results { get; private set; }

        public ISearchResult CurrentResult
        {
            get
            {
                return _currentResult;
            }

            set
            {
                if (value != _currentResult)
                {
                    _currentResult = value;
                    RaiseCurrentResultChangedEvent();
                }
            }
        }

        #endregion

        #region ISearch implementation

        public string Summary { get; protected set; }

        public void Cancel() { }

        public void Rerun() 
        {
            ClearResults();
            _searching = true;
            DoSearch();
            _searching = false;
        }

        public void Start()
        {
            if (_searching)
                throw new Exception("Attempt to perform reentrant search in FindAllReferencesSearch.");

            _searching = true;
            DoSearch();
            _searching = false;
        }

        public void MoveToNextResult()
        {
            if(CurrentResult == null)
            {
                if(_results.Count > 0)
                {
                    CurrentResult = _results.First();
                }
            }
            else
            {
                int idx = _results.IndexOf(CurrentResult);
                if(idx >= 0)
                    CurrentResult = _results[idx == (_results.Count-1) ? 0 : idx+1];
            }
        }

        public void MoveToPreviousResult()
        {
            if (CurrentResult == null)
            {
                if (_results.Count > 0)
                {
                    CurrentResult = _results.Last();
                }
            }
            else
            {
                int idx = _results.IndexOf(CurrentResult);
                if (idx >= 0)
                    CurrentResult = _results[idx == 0 ? _results.Count-1 : idx-1];
            }
        }

        #endregion

        protected void ClearResults()
        {
            _results.Clear();
        }

        protected void AddResult(ISearchResult result)
        {
            _results.Add(result);
        }

        protected abstract void DoSearch();
    }
}
