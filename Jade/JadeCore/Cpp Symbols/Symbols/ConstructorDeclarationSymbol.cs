using System;
using System.Collections.Generic;
using LibClang;

namespace JadeCore.CppSymbols
{
    public class ConstructorDeclarationSymbol : SymbolCursorBase
    {
        private List<MethodArgumentSymbol> _args;

        public ConstructorDeclarationSymbol(Cursor cur)
            : base(cur)
        {
            _args = new List<MethodArgumentSymbol>();
            foreach (Cursor arg in cur.ArgumentCursors)
            {
                _args.Add(new MethodArgumentSymbol(arg));
            }
        }

        public IEnumerable<MethodArgumentSymbol> Arguments
        {
            get { return _args; }
        }
    }
}
