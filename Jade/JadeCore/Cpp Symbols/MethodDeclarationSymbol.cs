using System;
using System.Text;
using System.Collections.Generic;
using LibClang;

namespace JadeCore.CppSymbols
{
    public class MethodArgumentSymbol : SymbolCursorBase
    {
        public MethodArgumentSymbol(Cursor cur)
            :base(cur)
        {
        }

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
            if(IsLValueReference)
            {
                sb.Append(Pointee.Spelling);
                sb.Append("& ");
                sb.Append(Cursor.Spelling);
            }
            else if(IsPointer)
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

        public Cursor C { get { return Cursor; } }
    }

    public class MethodDeclarationSymbol : SymbolCursorBase
    {
        private List<MethodArgumentSymbol> _args;

        public MethodDeclarationSymbol(Cursor cur)
            : base(cur)
        {
            _args = new List<MethodArgumentSymbol>();
            foreach(Cursor arg in cur.ArgumentCursors)
            {
                _args.Add(new MethodArgumentSymbol(arg));
            }
        }

        public IEnumerable<MethodArgumentSymbol> Arguments
        {
            get { return _args; }
        }
    }

    public class ConstructorDeclarationSymbol : SymbolCursorBase
    {
        private List<MethodArgumentSymbol> _args;

        public ConstructorDeclarationSymbol(Cursor cur)
            : base(cur)
        {
            _args = new List<MethodArgumentSymbol>();
            foreach (Cursor arg in cur.ArgumentCursors)
            {
                _args.Add(new MethodArgumentSymbol(arg));
            }
        }

        public IEnumerable<MethodArgumentSymbol> Arguments
        {
            get { return _args; }
        }
    }
}
