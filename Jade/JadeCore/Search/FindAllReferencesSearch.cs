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
            TranslationUnit tu = _projectIndex.FindTranslationUnit(_location.Path);
            if (tu == null)
                return null;
            Cursor c = tu.GetCursorAt(_location.Path.Str, _location.Offset);

            if (c != null && c.DefinitionCursor != null)
                c = c.DefinitionCursor;

            return new CodeLocation(c.Location);
        }

        protected override void DoSearch()
        {
            HashSet<LibClang.SourceLocation> uniqueResults = new HashSet<LibClang.SourceLocation>();

            ICodeLocation loc = FindSearchOrigin();
            

            //foreach(CppCodeBrowser.ISourceFile sf in _projectIndex.SourceFiles)
            foreach(var parseResult in _projectIndex.ParseResults)
            {
                TranslationUnit tu = parseResult.TranslationUnit;

                Cursor c = null;
                if(loc != null)
                    c = tu.GetCursorAt(loc.Path.Str, loc.Offset);
                else
                    c = tu.GetCursorAt(_location.Path.Str, _location.Offset);

                if (c != null)
                {
                    if (Summary == null)
                    {
                        Summary = "Find all references to: " + c.Spelling;
                    }
                    int before = uniqueResults.Count;
                    tu.FindAllReferences(c,
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

                    Debug.WriteLine(string.Format("sf {0} added {1}", parseResult.Path.FileName, after - before));
                }
                else
                {
                    Debug.WriteLine(string.Format("sf {0} added 0", parseResult.Path.FileName));
                }
            }
            Debug.WriteLine(string.Format("sf total {0} ", uniqueResults.Count));
        }
    }
}
