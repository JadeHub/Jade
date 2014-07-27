using System;
using System.Collections.Generic;
using LibClang;

namespace JadeCore.CppSymbols2
{
    public class VariableDeclSymbol : SymbolCursorBase
    {
        public VariableDeclSymbol(Cursor cur)
            : base(cur)
        {
            
        }
    }
}
