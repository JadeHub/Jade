using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadeControls.SymbolInspector
{
    public class MethodDeclarationViewModel : SymbolViewModelBase
    {
        public MethodDeclarationViewModel(JadeCore.CppSymbols2.MethodDeclarationSymbol symbol)
            : base(symbol)
        {
            
        }

        private JadeCore.CppSymbols2.MethodDeclarationSymbol MethodDecl
        {
            get { return SymbolCursor as JadeCore.CppSymbols2.MethodDeclarationSymbol; }
        }

        private string BuildParamText()
        {
            StringBuilder sb = new StringBuilder();
            LibClang.Cursor c = SymbolCursor.Cursor;

            sb.Append("(");

            foreach(JadeCore.CppSymbols2.MethodArgumentSymbol arg in MethodDecl.Arguments)
            {                
                sb.Append(arg.ToString());
                if (arg != MethodDecl.Arguments.Last())
                    sb.Append(", ");
            }
            sb.Append(")");

            if (c.IsConstMethod)
                sb.Append(" const");

            sb.Append(" : ");
            sb.Append(c.Type.ResultType.Spelling);
            return sb.ToString();
        }

        public override string DisplayText
        {
            get 
            {
                return "";//();
            }
        }

        public string ParamText
        {
            get { return BuildParamText(); }
        }
    }
}
