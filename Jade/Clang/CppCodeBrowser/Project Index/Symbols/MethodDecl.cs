using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibClang;

namespace CppCodeBrowser.Symbols
{
    public class MethodDecl : DeclarationBase
    {
        private Cursor _definition;
        private ClassDecl _class;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="definition">Implementation</param>
        /// <param name="declaration">Method declaration in header, may be the same as declaration</param>
        public MethodDecl(Cursor declaration, ISymbolTable table)
            : base(declaration, table)
        {
            Debug.Assert(declaration.SemanticParentCurosr.Kind == CursorKind.ClassDecl);
            _class = table.FindClass(declaration.SemanticParentCurosr.Usr);
            Debug.Assert(_class != null);
            //_class.AddMethodDecl(this);
        }

        public ClassDecl Class { get { return _class; } }

        public override string Name { get { return Cursor.Spelling; } }
        public override EntityKind Kind { get { return EntityKind.Method; } }

        public void SetDefinition(Cursor c)
        {
            Debug.Assert(c.Kind == CursorKind.CXXMethod);
            Debug.Assert(c.IsDefinition);
            if (_definition == null)
                _definition = c;
        }
    }
}