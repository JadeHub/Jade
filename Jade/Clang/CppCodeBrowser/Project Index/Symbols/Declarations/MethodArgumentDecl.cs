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
}