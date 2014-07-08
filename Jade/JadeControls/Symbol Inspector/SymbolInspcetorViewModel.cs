using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JadeCore;

namespace JadeControls.SymbolInspector
{
    public class SymbolInspectorPaneViewModel : JadeControls.Docking.ToolPaneViewModel
    {
        private SymbolViewModelBase _symbol;

        public SymbolInspectorPaneViewModel(IEditorController editorController)
        {
            Title = "Symbol Inspector";
            ContentId = "SymbolInspectorPaneViewModel";
        }

        public JadeCore.CppSymbols.ISymbolCursor SymbolCursor
        {
            set
            {
                if (_symbol == null || _symbol.SymbolCursor != value && _symbol is JadeCore.CppSymbols.ClassDeclarationSymbol)
                {
                    _symbol = new ClassDeclarationViewModel(value as JadeCore.CppSymbols.ClassDeclarationSymbol);
                    OnPropertyChanged("Symbol");
                }
            }
        }

        public SymbolViewModelBase Symbol
        {
            get { return _symbol; }
        }
    }
}
