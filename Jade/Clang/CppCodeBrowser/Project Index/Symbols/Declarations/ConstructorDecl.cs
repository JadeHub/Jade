using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using LibClang;

namespace CppCodeBrowser.Symbols
{
    public class ConstructorDecl : FunctionDeclBase
    {
        private ClassDecl _class;

        public ConstructorDecl(Cursor c, ISymbolTable table)
            : base(c, table)
        {            
            Debug.Assert(c.Kind == CursorKind.Constructor);
            _class = table.FindClassDeclaration(c.SemanticParentCurosr.Usr);
            //could be classtemplate 
            //Debug.Assert(_class != null);
        }

        public override EntityKind Kind { get { return EntityKind.Constructor; } }

        public ClassDecl Class { get { return _class; } }
    }
}
