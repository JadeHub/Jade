using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JadeCore;
using JadeControls.CppCodeViewModel;

namespace JadeControls.SymbolInspector
{
    public class SymbolInspectorPaneViewModel : JadeControls.Docking.ToolPaneViewModel
    {
        public SymbolInspectorPaneViewModel(IEditorController editorController)
        {
            Title = "Symbol Inspector";
            ContentId = "SymbolInspectorPaneViewModel";
        }

        public JadeCore.CppSymbols.ISymbolCursor SymbolCursor
        {
            get;
            set;
        }
    }
}
