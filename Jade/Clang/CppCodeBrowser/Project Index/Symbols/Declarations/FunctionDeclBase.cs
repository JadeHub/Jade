using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibClang;

namespace CppCodeBrowser.Symbols
{
    public class MethodArgumentSymbol : DeclarationBase
    {
        public MethodArgumentSymbol(Cursor declaration, ISymbolTable table)
            : base(declaration, table)
        {
        }

        public override string Name { get { return ToString(); } }
        public override EntityKind Kind { get { return EntityKind.FunctionArg; } }

        public bool IsConst
        {
            get
            {
                if (IsLValueReference)
                    return Cursor.Type.PointeeType.IsConstQualified;
                return Cursor.Type.IsConstQualified;
            }
        }

        public bool IsLValueReference
        {
            get { return Cursor.Type.Kind == TypeKind.LValueReference; }
        }

        public bool IsRValueReference
        {
            get { return Cursor.Type.Kind == TypeKind.RValueReference; }
        }

        public bool IsPointer
        {
            get { return Cursor.Type.Kind == TypeKind.Pointer; }
        }

        public LibClang.Type Pointee
        {
            get { return Cursor.Type.PointeeType; }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (IsLValueReference)
            {
                sb.Append(Pointee.Spelling);
                sb.Append("& ");
                sb.Append(Cursor.Spelling);
            }
            else if (IsPointer)
            {
                sb.Append(Pointee.Spelling);
                sb.Append("* ");
                sb.Append(Cursor.Spelling);
            }
            else
            {
                sb.Append(Cursor.Type.Spelling);
                sb.Append(" ");
                sb.Append(Cursor.Spelling);
            }
            return sb.ToString();
        }
    }

    /// <summary>
    /// Add things common to Functions, Methods, Constructors and Destructors
    /// 
    /// </summary>
    public abstract class FunctionDeclBase : DeclarationBase
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

        public void UpdateDefinition(Cursor c)
        {
            Debug.Assert(CursorKinds.IsFunctionEtc(c.Kind));
            Debug.Assert(c.IsDefinition);
            if (_definition == null)
                _definition = c;
        }

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