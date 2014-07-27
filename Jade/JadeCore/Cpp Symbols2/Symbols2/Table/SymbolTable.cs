using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibClang;

namespace JadeCore.CppSymbols2
{
    public class TranslationUnitTable : ISymbolTable
    {
        public void Update(LibClang.TranslationUnit tu)
        {
            Reset(tu);
        }

        private void Reset(LibClang.TranslationUnit tu)
        {
            Cursor c = tu.Cursor;

        }
    }
}
