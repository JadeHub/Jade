﻿using System;
using System.Collections.Generic;
using LibClang;

namespace JadeCore.CppSymbols
{
    public class DataMemberDeclarationSymbol : SymbolCursorBase
    {
        public DataMemberDeclarationSymbol(Cursor cur)
            : base(cur)
        {
        }
    }
}