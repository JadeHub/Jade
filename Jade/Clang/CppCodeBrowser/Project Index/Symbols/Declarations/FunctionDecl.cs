using LibClang;
using System.Diagnostics;

namespace CppCodeBrowser.Symbols
{
    public class FunctionDecl : FunctionDeclBase
    { 
        public IDeclaration _parent;

        public FunctionDecl(Cursor c, ISymbolTable table)
            : base(c, table)
        {
            Debug.Assert(c.Kind == CursorKind.FunctionDecl);
            if (c.SemanticParentCurosr.Kind == CursorKind.Namespace)
            {
                _parent = table.FindNamespaceDeclaration(c.SemanticParentCurosr.Usr);
                Debug.Assert(_parent != null);
            }
            else if (c.SemanticParentCurosr.Kind == CursorKind.TranslationUnit ||
                    c.SemanticParentCurosr.Kind == CursorKind.UnexposedDecl)
            {
                _parent = null;
            }
            else
            {
                Debug.Assert(false);
            }
        }

        public override EntityKind Kind { get { return EntityKind.Function; } }

        public IDeclaration Parent { get { return _parent; } }
    }
}
