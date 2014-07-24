using System;
using System.Collections.Generic;
using System.Linq;
using LibClang;

namespace JadeCore.CppSymbols
{
    public class NamespaceSymbol : SymbolCursorBase
    {
        private List<ClassDeclarationSymbol> _classes;

        public NamespaceSymbol(Cursor cur)
            : base(cur)
        {
        }

        public IEnumerable<ClassDeclarationSymbol> Classes
        {
            get
            {
                if(_classes == null)
                {
                    _classes = new List<ClassDeclarationSymbol>(GetType<ClassDeclarationSymbol>(LibClang.CursorKind.ClassDecl));
                  //  _classes.AddRange(GetType<ClassDeclarationSymbol>(LibClang.CursorKind));
                }
                return _classes;
            }
        }
    }
}
