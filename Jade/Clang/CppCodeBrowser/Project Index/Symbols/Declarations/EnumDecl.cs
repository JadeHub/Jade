using System;
using System.Diagnostics;
using System.Collections.Generic;
using LibClang;

namespace CppCodeBrowser.Symbols
{
    public class EnumDecl : DeclarationBase
    {
        public IDeclaration _parent;

        public EnumDecl(Cursor c, ISymbolTable table)
            : base(c, table)
        {
            Debug.Assert(c.Kind == CursorKind.EnumDecl);
            if (c.SemanticParentCurosr.Kind == CursorKind.Namespace)
            {
                _parent = table.FindNamespaceDeclaration(c.SemanticParentCurosr.Usr);
                Debug.Assert(_parent != null);
            }
            else if (CursorKinds.IsClassStructEtc(c.SemanticParentCurosr.Kind))
            {
                _parent = table.FindClassDeclaration(c.SemanticParentCurosr.Usr);
                Debug.Assert(_parent != null);
            }
            else
            {
                CursorKind k = c.SemanticParentCurosr.Kind;
            }
        }

        public override string Name { get { return Cursor.Spelling; } }
        public override EntityKind Kind { get { return EntityKind.Enum; } }

        public IDeclaration Parent { get { return _parent; } }
    }
}
