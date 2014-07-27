using System;
using System.Text;
using System.Collections.Generic;
using LibClang;

namespace JadeCore.CppSymbols2
{
    public class MethodDeclarationSymbol : SymbolCursorBase
    {
        private List<MethodArgumentSymbol> _args;

        public MethodDeclarationSymbol(Cursor cur)
            : base(cur)
        {
            _args = new List<MethodArgumentSymbol>();
            foreach(Cursor arg in cur.ArgumentCursors)
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
