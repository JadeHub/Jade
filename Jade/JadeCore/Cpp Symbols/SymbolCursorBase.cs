using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadeCore.CppSymbols
{
    public abstract class SymbolCursorBase : ISymbolCursor
    {
        private LibClang.Cursor _cursor;

        public SymbolCursorBase(LibClang.Cursor c)
        {
            _cursor = c;
        }

        public LibClang.Cursor Cursor
        {
            get { return _cursor; }
        }

        public virtual string Spelling
        {
            get { return Cursor.Spelling; }
        }
        
        protected IEnumerable<T> GetType<T>(LibClang.CursorKind kind)
        {
            return from c in _cursor.Children
                   where c.Kind == kind
                   select
                       (T)JadeCore.Services.Provider.SymbolCursorFactory.Create(c);
                       ;
        }
    }
}
