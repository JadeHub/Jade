using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibClang;

namespace CppCodeBrowser.Symbols
{
    /// <summary>
    /// Add things common to Functions, Methods, Constructors and Destructors
    /// 
    /// </summary>
    public abstract class FunctionDeclBase : DeclarationBase
    {
        private Cursor _definition;

        //args
        //return type

        public FunctionDeclBase(Cursor declaration, ISymbolTable table)
            : base(declaration, table)
        {
        }

        public void UpdateDefinition(Cursor c)
        {
            Debug.Assert(CursorKinds.IsFunctionEtc(c.Kind));
            Debug.Assert(c.IsDefinition);
            if (_definition == null)
                _definition = c;
        }
    }

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