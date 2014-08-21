using System;
using System.Linq;
using System.Text;
using System.Diagnostics;
using CppCodeBrowser.Symbols;

namespace JadeControls.SymbolInspector
{
    public class ConstructorViewModel : SymbolViewModelBase
    {
        ConstructorDecl _declaration;

        public ConstructorViewModel(ConstructorDecl decl)
            : base(decl.Cursor)
        {
            _declaration = decl;
        }

        private string BuildParamText()
        {
            return _declaration.BuildParamText();
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
