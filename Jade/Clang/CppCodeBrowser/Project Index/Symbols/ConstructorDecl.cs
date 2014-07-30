using System;
using System.Diagnostics;
using System.Collections.Generic;
using LibClang;

namespace CppCodeBrowser.Symbols
{
    public class ConstructorDecl : DeclarationBase
    {
        private ClassDecl _class;

        public ConstructorDecl(Cursor c, ISymbolTable table)
            : base(c, table)
        {
            Debug.Assert(c.Kind == CursorKind.Constructor);
            _class = table.FindClass(c.SemanticParentCurosr.Usr);
            //could be classtemplate 
            //Debug.Assert(_class != null);
        }

        public override string Name { get { return Cursor.Spelling; } }
        public override EntityKind Kind { get { return EntityKind.Constructor; } }

        public ClassDecl Class { get { return _class; } }
    }
}
