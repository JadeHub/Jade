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

        private ICodeLocation FindSearchOrigin()
        {
            //starting location
            CppCodeBrowser.ISourceFile fileIndex = _projectIndex.FindSourceFile(_location.Path);
            if (fileIndex == null)
                return null;
            Cursor c = fileIndex.GetCursorAt(_location.Path, _location.Offset);

            if (c != null && c.Definition != null)
                c = c.Definition;

            return new CodeLocation(c.Location);
        }

        protected override void DoSearch()
        {
            HashSet<LibClang.SourceLocation> uniqueResults = new HashSet<LibClang.SourceLocation>();

            ICodeLocation loc = FindSearchOrigin();
            
            foreach(CppCodeBrowser.ISourceFile sf in _projectIndex.SourceFiles)
            {
                
                
                Cursor c = null;
                if(loc != null)
                    c = sf.TranslationUnit.GetCursorAt(loc.Path.Str, loc.Offset);
                else
                    c = sf.TranslationUnit.GetCursorAt(_location.Path.Str, _location.Offset);

                if (c != null)
                {
                    if (Summary == null)
                    {
                        Summary = "Find all references to: " + c.Spelling;
                    }
                    int before = uniqueResults.Count;
                    sf.TranslationUnit.FindAllReferences(c,
                                        delegate(Cursor cursor, SourceRange range)
                                        {
                                            if (uniqueResults.Add(cursor.Location))
                                            {
                                                ICodeLocation location = new CodeLocation(range.Start);
                                                ISearchResult result = new CodeSearchResult(10, location, range.Length);
                                                AddResult(result);
                                                Debug.WriteLine(result);
                                            }
                                            return true;
                                        });
                    int after = uniqueResults.Count;

                    Debug.WriteLine(string.Format("sf {0} added {1}", sf.Path.FileName, after - before));
                }
                else
                {
                    Debug.WriteLine(string.Format("sf {0} added 0", sf.Path.FileName));
                }
            }
            Debug.WriteLine(string.Format("sf total {0} ", uniqueResults.Count));
        }
    }
}
