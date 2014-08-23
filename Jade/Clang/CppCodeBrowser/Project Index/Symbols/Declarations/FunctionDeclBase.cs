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
    public abstract class FunctionDeclBase : DeclarationBase, IHasDefinition
    {
        private Cursor _definition;
        private List<MethodArgumentSymbol> _args;
                
        public IEnumerable<MethodArgumentSymbol> Arguments { get { return _args; } }
        
        public LibClang.Type ReturnType
        {
            get
            {
                return Cursor.Type.ResultType;
            }
        }

        public FunctionDeclBase(Cursor declaration, ISymbolTable table)
            : base(declaration, table)
        {
            _args = new List<MethodArgumentSymbol>();
            foreach (Cursor arg in declaration.ArgumentCursors)
            {
                _args.Add(new MethodArgumentSymbol(arg, table));
            }
        }

        #region IHasDefinition

        public void UpdateDefinition(Cursor c)
        {
            Debug.Assert(CursorKinds.IsFunctionEtc(c.Kind));
            Debug.Assert(c.IsDefinition);
            if (_definition == null)
                _definition = c;
        }

        public Cursor DefinitionCursor
        {
            get { return _definition; }
        }

        public bool HasDefinitionCursor { get { return _definition != null; } }

        #endregion

        public string BuildParamText()
        {
            StringBuilder sb = new StringBuilder();
            
            sb.Append("(");

            foreach(MethodArgumentSymbol arg in Arguments)
            {                
                sb.Append(arg.ToString());
                if (arg != Arguments.Last())
                    sb.Append(", ");
            }
            sb.Append(")");

            if (Cursor.IsConstMethod)
                sb.Append(" const");

            return sb.ToString();
        }
    }
}