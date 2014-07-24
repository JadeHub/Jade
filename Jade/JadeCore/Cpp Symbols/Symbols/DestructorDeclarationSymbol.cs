using System;
using System.Collections.Generic;
using LibClang;

namespace JadeCore.CppSymbols
{
    public class DestructorDeclarationSymbol : SymbolCursorBase
    {
        public DestructorDeclarationSymbol(Cursor cur)
            : base(cur)
        {
            
        }
    }
}
