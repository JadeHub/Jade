using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadeCore.CppSymbols
{
    public interface ISymbolCursor
    {
        LibClang.Cursor Cursor { get; }
    }

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
    }

    public class ClassSymbol : SymbolCursorBase
    {
        public ClassSymbol(LibClang.Cursor cur)
            :base(cur)
        { }
    }
}
