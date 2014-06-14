using System;
using System.Linq;
using System.Text;
using System.Diagnostics;
using JadeCore.CppSymbols;

namespace JadeControls.SymbolInspector
{
    public class ConstructorViewModel : SymbolViewModelBase
    {
        public ConstructorViewModel(ConstructorDeclarationSymbol symbol)
            : base(symbol)
        {

        }

        private JadeCore.CppSymbols.ConstructorDeclarationSymbol CtorSymbol
        {
            get
            {
                Debug.Assert(SymbolCursor is ConstructorDeclarationSymbol);
                return SymbolCursor as ConstructorDeclarationSymbol;
            }
        }

        private string BuildDisplayText()
        {
            StringBuilder sb = new StringBuilder();
            LibClang.Cursor c = SymbolCursor.Cursor;

            //name is in form "FuncName([params, ...])"
            sb.Append(SymbolCursor.Spelling);
            sb.Append("(");

            foreach (JadeCore.CppSymbols.MethodArgumentSymbol arg in CtorSymbol.Arguments)
            {
                sb.Append(arg.ToString());
                if (arg != CtorSymbol.Arguments.Last())
                    sb.Append(", ");
            }
            sb.Append(")");
                        
            return sb.ToString();
        }

        public override string DisplayText
        {
            get
            {
                return BuildDisplayText();
                //return SourceText; 
            }
        }
    }
}
