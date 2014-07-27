using System;
using System.Collections.Generic;
using LibClang;

namespace JadeCore.CppSymbols2
{
    public class FunctionDeclSymbol : SymbolCursorBase
    {
        public FunctionDeclSymbol(Cursor cur)
            : base(cur)
        {

        }
    }
}