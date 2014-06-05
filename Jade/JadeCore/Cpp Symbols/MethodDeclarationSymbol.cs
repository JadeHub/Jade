using System;
using LibClang;

namespace JadeCore.CppSymbols
{
    public class MethodDeclarationSymbol : SymbolCursorBase
    {
        public MethodDeclarationSymbol(Cursor cur)
            : base(cur)
        { }

        public string MethodDeclaration
        {
            get 
            {
                string result = "";

                foreach(LibClang.Token tok in Cursor.Extent.Tokens.Tokens)
                {
                    result += tok.Spelling;
                }
                return result; 
            }
        }

        public override string Spelling
        {
            get { return MethodDeclaration; }
        }
    }

    public class ConstructorDeclarationSymbol : SymbolCursorBase
    {
        public ConstructorDeclarationSymbol(Cursor cur)
            : base(cur)
        { }

        public string MethodDeclaration
        {
            get
            {
                string result = "";

                foreach (LibClang.Token tok in Cursor.Extent.Tokens.Tokens)
                {
                    result += tok.Spelling;
                }
                return result;
            }
        }

        public override string Spelling
        {
            get { return MethodDeclaration; }
        }
    }
}
