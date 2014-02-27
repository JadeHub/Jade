using CppCodeBrowser;
using LibClang;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace JadeCore.Search
{
    public class FindAllReferencesSearch : ISearch
    {
        #region Data
                
        private IProjectIndex _projectIndex;
        private ICodeLocation _location;
        private bool _searching;
        private ObservableCollection<ISearchResult> _results;

        #endregion

        #region Constructor

        public FindAllReferencesSearch(IProjectIndex index, ICodeLocation location)
        {
            JadeUtils.ArgChecking.ThrowIfNull(index, "index");
            JadeUtils.ArgChecking.ThrowIfNull(location, "location");

            _projectIndex = index;
            _location = location;
            _searching = false;
            _results = new ObservableCollection<ISearchResult>();
            Results = new ReadOnlyObservableCollection<ISearchResult>(_results);
        }

        #endregion

        #region ISearch implementation

        public string Summary { get; private set; }
                
        public void Cancel() { }

        public void Rerun() {}

        public ReadOnlyObservableCollection<ISearchResult> Results { get; private set; }
        
        #endregion

        public void Start()
        {
            if (_searching)
                throw new Exception("Attempt to perform reentrant search in FindAllReferencesSearch.");

            HashSet<ICodeLocation> uniqueLocations = new HashSet<ICodeLocation>();
            _searching = true;
            foreach (LibClang.TranslationUnit tu in _projectIndex.TranslationUnits)
            {
                Cursor c = tu.GetCursorAt(_location.Path.Str, _location.Offset);
                if (c != null)
                {                    
                    tu.FindAllReferences(c,
                                        delegate(Cursor cursor, SourceRange range)
                                        {
                                            ICodeLocation loc = new CodeLocation(cursor.Location);
                                            if (uniqueLocations.Add(loc))
                                            {
                                                ISearchResult result = new CodeSearchResult(10, loc.Path, loc.Offset);
                                                _results.Add(result);
                                                Debug.WriteLine(result);
                                            }
                                            return true;
                                        });
                }
            }
            _searching = false;
        }
    }
}
