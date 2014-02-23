using System;
using System.Collections.Generic;

namespace CppCodeBrowser
{
    public interface ICodeBrowser
    {
        /// <summary>
        /// Return, via callback, code locations which 'match'
        /// </summary>
        /// <param name="location"></param>
        /// Starting location of the browse action.
        /// <param name="OnResult"></param>
        /// Function called when a match is found. 
        /// Function should return false to stop searching.
        /// <returns></returns>
        void BrowseFrom(IEnumerable<LibClang.Cursor> fromCursors, Func<ICodeLocation, bool> OnResult);
    }    
}
