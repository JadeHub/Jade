using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibClang;

namespace CppCodeBrowser.Symbols
{
    public class MethodDecl : FunctionDeclBase
    {
        private ClassDecl _class;

        //is const
        
        public MethodDecl(Cursor declaration, ISymbolTable table)
            : base(declaration, table)
        {
            Debug.Assert(CursorKinds.IsClassStructEtc(declaration.SemanticParentCurosr.Kind));
            _class = table.FindClass(declaration.SemanticParentCurosr.Usr);
            Debug.Assert(_class != null);
            //_class.AddMethodDecl(this);
        }

        public ClassDecl Class { get { return _class; } }

        public override string Name { get { return Cursor.Spelling; } }
        public override EntityKind Kind { get { return EntityKind.Method; } }

        public bool IsConst { get { return Cursor.IsConstMethod; } }
    }
}