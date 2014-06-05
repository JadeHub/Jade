using System;
using LibClang;

namespace JadeCore.CppSymbols
{
    public interface ISymbolCursor
    {
        Cursor Cursor { get; }

        string Spelling { get; }
    }
}
