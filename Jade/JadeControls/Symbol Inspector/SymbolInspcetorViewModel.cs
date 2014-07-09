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
        private IEditorController _editorController;
        private bool _trackingCursor;
        private SymbolViewModelBase _symbol;
        private static NullSymbolViewModel _nullSymbolViewModel = new NullSymbolViewModel();

        public SymbolInspectorPaneViewModel(IEditorController editorController)
        {
            Title = "Symbol Inspector";
            ContentId = "SymbolInspectorPaneViewModel";
            _editorController = editorController;
            IsTrackingCursor = false;
            _symbol = _nullSymbolViewModel;
        }

        public JadeCore.CppSymbols.ISymbolCursor SymbolCursor
        {
            set
            {
                if (_symbol == null || _symbol.SymbolCursor != value && value is JadeCore.CppSymbols.ClassDeclarationSymbol)
                {
                    _symbol = new ClassDeclarationViewModel(value as JadeCore.CppSymbols.ClassDeclarationSymbol);
                    OnPropertyChanged("Symbol");
                }
                else
                {
                    _symbol = _nullSymbolViewModel;
                }
            }
        }
        

        public bool IsTrackingCursor
        {
            get { return _trackingCursor; }
            set 
            {
                if (_trackingCursor != value)
                {
                    _trackingCursor = value;
                    OnPropertyChanged("TrackingCursor");
                }
            }
        }

        public SymbolViewModelBase Symbol
        {
            get { return _symbol; }
        }
    }
}
