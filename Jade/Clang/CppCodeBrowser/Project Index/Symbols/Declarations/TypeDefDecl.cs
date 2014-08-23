using System;
using System.Diagnostics;
using System.Collections.Generic;
using LibClang;

namespace CppCodeBrowser.Symbols
{
    public class TypedefDecl : DeclarationBase
    {
        public TypedefDecl(Cursor c, ISymbolTable table)
            : base(c, table)
        {
            Debug.Assert(c.Kind == CursorKind.TypedefDecl);
        }

        public override EntityKind Kind { get { return EntityKind.Typedef; } }
    }
}
