using CppCodeBrowser;
using LibClang;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using JadeUtils.IO;

namespace JadeCore.Search
{    
    public class FindAllReferencesSearch : SearchBase
    {
        #region Data
                
        private IProjectIndex _projectIndex;
        private ICodeLocation _location;
        
        #endregion

        #region Constructor

        public FindAllReferencesSearch(IProjectIndex index, ICodeLocation location)
        {
            JadeUtils.ArgChecking.ThrowIfNull(index, "index");
            JadeUtils.ArgChecking.ThrowIfNull(location, "location");

            _projectIndex = index;
            _location = location;
        }

        #endregion

        #region ISearch implementation
                
        #endregion

        protected override void DoSearch()
        {
            HashSet<LibClang.SourceLocation> uniqueLocations = new HashSet<LibClang.SourceLocation>();
            
            foreach (LibClang.TranslationUnit tu in _projectIndex.TranslationUnits)
            {
                Cursor c = tu.GetCursorAt(_location.Path.Str, _location.Offset);
                if (c != null)
                {
                    if (Summary == null)
                    {
                        Summary = "Find all references to: " + c.Spelling;
                    }
                    tu.FindAllReferences(c,
                                        delegate(Cursor cursor, SourceRange range)
                                        {
                                            if (uniqueLocations.Add(cursor.Location))
                                            {
                                                ICodeLocation location = new CodeLocation(range.Start);
                                                ISearchResult result = new CodeSearchResult(10, location, range.Length);
                                                AddResult(result);
                                                Debug.WriteLine(result);
                                            }
                                            return true;
                                        });

                }
            }
        }
    }
}
