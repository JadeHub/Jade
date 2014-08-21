using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CppCodeBrowser.Symbols;

namespace JadeControls.SymbolInspector
{
    public class MethodDeclarationViewModel : SymbolViewModelBase
    {
        private MethodDecl _declaration;

        public MethodDeclarationViewModel(MethodDecl decl)
            : base(decl.Cursor)
        {
            _declaration = decl;
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
            get { return _declaration.BuildParamText(); }
        }
    }
}
