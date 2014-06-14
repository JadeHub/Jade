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

        public override string DisplayText
        {
            get { return SourceText; }
        }
    }
}
