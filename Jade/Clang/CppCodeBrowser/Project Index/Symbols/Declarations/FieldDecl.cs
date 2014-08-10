using System;
using System.Diagnostics;
using System.Collections.Generic;
using LibClang;

namespace CppCodeBrowser.Symbols
{
    public class FieldDecl : DeclarationBase
    {
        private ClassDecl _class;

        public FieldDecl(Cursor c, ISymbolTable table)
            : base(c, table)
        {
            Debug.Assert(c.Kind == CursorKind.FieldDecl);
            _class = table.FindClassDeclaration(c.SemanticParentCurosr.Usr);
            Debug.Assert(_class != null);
        }

        public override string Name { get { return Cursor.Spelling; } }
        public override EntityKind Kind { get { return EntityKind.Field; } }

        public ClassDecl Class { get { return _class; } }
    }
}
