using System;
using System.Linq;
using System.Text;
using System.Diagnostics;
using JadeCore.CppSymbols2;

namespace JadeControls.SymbolInspector
{
    public class ConstructorViewModel : SymbolViewModelBase
    {
        public ConstructorViewModel(ConstructorDeclarationSymbol symbol)
            : base(symbol)
        {

        }

        private JadeCore.CppSymbols2.ConstructorDeclarationSymbol CtorSymbol
        {
            get
            {
                Debug.Assert(SymbolCursor is ConstructorDeclarationSymbol);
                return SymbolCursor as ConstructorDeclarationSymbol;
            }
        }

        private string BuildParamText()
        {
            StringBuilder sb = new StringBuilder();
            //LibClang.Cursor c = SymbolCursor.Cursor;

            sb.Append("(");

            foreach (JadeCore.CppSymbols2.MethodArgumentSymbol arg in CtorSymbol.Arguments)
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
                return "";
                //return SourceText; 
            }
        }

        public string ParamText
        {
            get { return BuildParamText(); }
        }
    }
}
