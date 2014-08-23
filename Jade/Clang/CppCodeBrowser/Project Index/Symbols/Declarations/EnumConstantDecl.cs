using System;
using System.Diagnostics;
using System.Collections.Generic;
using LibClang;

namespace CppCodeBrowser.Symbols
{
    public class EnumConstantDecl : DeclarationBase
    {
        public EnumDecl _parent;

        public EnumConstantDecl(Cursor c, ISymbolTable table)
            : base(c, table)
        {
            Debug.Assert(c.Kind == CursorKind.EnumConstantDecl);
            Debug.Assert(c.SemanticParentCurosr != null);
            Debug.Assert(c.SemanticParentCurosr.Kind == CursorKind.EnumDecl);
            _parent = table.FindEnumDeclaration(c.SemanticParentCurosr.Usr);
            Debug.Assert(_parent != null);
        }

        public override EntityKind Kind { get { return EntityKind.EnumConstant; } }

        public EnumDecl Parent { get { return _parent; } }
    }
}
