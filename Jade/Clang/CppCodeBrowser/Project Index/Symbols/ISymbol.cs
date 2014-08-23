using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibClang;

namespace CppCodeBrowser.Symbols
{
  /*  public interface IFileSymbolMap
    {
        void AddMapping(int startOffset, int endOffset, ISymbol referenced);
    }*/

    public interface ISymbol
    {
        string Spelling { get; }
        ICodeLocation Location { get; }
        int SpellingLength { get; }
        Cursor Cursor { get; }
    }
}