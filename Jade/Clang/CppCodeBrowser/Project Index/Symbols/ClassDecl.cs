using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibClang;

namespace CppCodeBrowser.Symbols
{
    public class ClassDecl : DeclarationBase
    {
        public IDeclaration _parent;

        public ClassDecl(Cursor c, ISymbolTable table)
            : base(c, table)
        {
            Debug.Assert(c.Kind == CursorKind.ClassDecl);
            if(c.SemanticParentCurosr.Kind == CursorKind.Namespace)
            {
                _parent = table.FindNamespace(c.SemanticParentCurosr.Usr);
                Debug.Assert(_parent != null);
            }
        }
        
        public override string Name { get { return Cursor.Spelling; } }
        public override EntityKind Kind { get { return EntityKind.Class; } }

        public IDeclaration Parent { get { return _parent; } }
    }
}
