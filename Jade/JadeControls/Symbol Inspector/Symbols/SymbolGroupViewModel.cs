using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadeControls.SymbolInspector
{
    public class SymbolGroupItemViewModel : NotifyPropertyChanged
    {
        private JadeCore.CppSymbols.SymbolCursorBase _symbol;
        
        public SymbolGroupItemViewModel(JadeCore.CppSymbols.SymbolCursorBase symbol)
        {
            _symbol = symbol;
        }

        public string SourceText
        {
            get { return _symbol.SourceText; }
        }

        public string Spelling
        {
            get { return _symbol.Spelling; }
        }
    }

    public class SymbolGroupViewModel : NotifyPropertyChanged
    {
        private string _name;
        private ObservableCollection<SymbolGroupItemViewModel> _symbols;

        public SymbolGroupViewModel(string name)
        {
            _name = name;
            _symbols = new ObservableCollection<SymbolGroupItemViewModel>();
        }

        public void AddSymbol(JadeCore.CppSymbols.SymbolCursorBase symbol)
        {
            _symbols.Add(new SymbolGroupItemViewModel(symbol));
        }

        public string Name
        {
            get { return _name; }
        }

        public ObservableCollection<SymbolGroupItemViewModel> Symbols
        {
            get { return _symbols; }
        }
    }
}
