using System;
using LibClang;

namespace JadeCore.CppSymbols
{
    public delegate void SymbolEvent();

    public interface ISymbolCursor
    {
        //event SymbolEvent Updated;
        //event SymbolEvent Removed;
        
        Cursor Cursor { get; }

        string Spelling { get; }
        string SourceText { get; }
    }
}
