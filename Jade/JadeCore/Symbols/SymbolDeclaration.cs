using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadeCore.Symbols
{
    public interface ISymbolDeclaration : ISymbol
    {
        string Name { get; }
        string Usr { get; }
        LibClang.Indexer.EntityKind Kind { get; }

        bool IsSameDecl(LibClang.Indexer.DeclInfo decl);
    }

    public class SymbolDeclaration : ISymbolDeclaration
    {
        private LibClang.Indexer.DeclInfo _decl;

        public SymbolDeclaration(LibClang.Indexer.DeclInfo decl)
        {
            _decl = decl;
        }

        #region ISymbol

        public LibClang.Cursor Cursor { get { return _decl.EntityInfo.Cursor; } }
        public CppCodeBrowser.ICodeLocation Location { get { return new CppCodeBrowser.CodeLocation(_decl.Location); } }

        #endregion

        #region ISymbolDeclration

        public string Name { get { return _decl.EntityInfo.Name; } }
        public string Usr { get { return _decl.EntityInfo.Usr; } }
        public LibClang.Indexer.EntityKind Kind { get { return _decl.EntityInfo.Kind; } }

        public bool IsSameDecl(LibClang.Indexer.DeclInfo decl)
        {
            return decl.EntityInfo.Kind == Kind &&
                    decl.EntityInfo.Name == Name &&
                    decl.EntityInfo.Usr == Usr &&
                    decl.IsDefinition == _decl.IsDefinition &&
                    decl.IsRedefinition == _decl.IsRedefinition;
        }

        public void UpdateDecl(LibClang.Indexer.DeclInfo decl)
        {
            _decl = decl;
        }

        #endregion
    }
}
