using System.Diagnostics;
using JadeCore.CppSymbols;

namespace JadeControls.SymbolInspector
{
    public class DataMemberViewModel : SymbolViewModelBase
    {
        public DataMemberViewModel(DataMemberDeclarationSymbol symbol)
            : base(symbol)
        {

        }

        private JadeCore.CppSymbols.DataMemberDeclarationSymbol CtorSymbol
        {
            get
            {
                Debug.Assert(SymbolCursor is DataMemberDeclarationSymbol);
                return SymbolCursor as DataMemberDeclarationSymbol;
            }
        }

        public override string DisplayText
        {
            get { return SourceText; }
        }
    }
}
