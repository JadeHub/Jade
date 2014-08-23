using System;
using System.Diagnostics;
using System.Collections.Generic;
using LibClang;

namespace CppCodeBrowser.Symbols
{
    public class DestructorDecl : FunctionDeclBase
    {
        private ClassDecl _class;

        public DestructorDecl(Cursor c, ISymbolTable table)
            : base(c, table)
        {
            Debug.Assert(c.Kind == CursorKind.Destructor);
            _class = table.FindClassDeclaration(c.SemanticParentCurosr.Usr);
        }

        public override EntityKind Kind { get { return EntityKind.Destructor; } }

        public ClassDecl Class { get { return _class; } }
    }
}
