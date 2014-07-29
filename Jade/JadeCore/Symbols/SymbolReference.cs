using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadeCore.Symbols
{

    public interface ISymbolReference : ISymbol
    {
        //ISymbolDeclaration ReferenedDefinition { get; }
    }
        
    public class SymbolReference : ISymbolReference
    {
        LibClang.Indexer.EntityReference _ref;

        public SymbolReference(LibClang.Indexer.EntityReference r)
        {
            _ref = r;

        }

        public LibClang.Cursor Cursor { get { return _ref.Cursor; } }
        public CppCodeBrowser.ICodeLocation Location { get { return new CppCodeBrowser.CodeLocation(_ref.Location); } }
    }
}
