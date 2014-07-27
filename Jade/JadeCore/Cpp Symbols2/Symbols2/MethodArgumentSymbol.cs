using System;
using System.Text;
using System.Collections.Generic;
using LibClang;

namespace JadeCore.CppSymbols2
{
    public class MethodArgumentSymbol : SymbolCursorBase
    {
        public MethodArgumentSymbol(Cursor cur)
            : base(cur)
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

        public Cursor C { get { return Cursor; } }
    }
}