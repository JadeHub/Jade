using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadeControls.SymbolInspector
{   
    public class SymbolGroupViewModel : NotifyPropertyChanged
    {
        private string _name;
        private ObservableCollection<SymbolViewModelBase> _symbols;
        private bool _expanded;

        public SymbolGroupViewModel(string name)
        {
            _name = name;
            _symbols = new ObservableCollection<SymbolViewModelBase>();
            _expanded = false;
        }

        public void AddSymbol(SymbolViewModelBase symbol)
        {
            _symbols.Add(symbol);
            _expanded = true;
            OnPropertyChanged("IsExpanded");
            OnPropertyChanged("IsEnabled");
        }

        public string Name
        {
            get { return _name; }
        }

        public ObservableCollection<SymbolViewModelBase> Symbols
        {
            get { return _symbols; }
        }

        public bool IsEnabled
        {
            get { return _symbols.Count > 0; }
        }

        public bool IsExpanded
        {
            get { return _expanded; }
            set
            {
                if(value != _expanded)
                {
                    _expanded = value;
                    OnPropertyChanged("IsExpanded");
                }
            }
        }

        public System.Windows.Media.Brush ForegroundColour
        {
            get
            {
                return IsEnabled ? System.Windows.Media.Brushes.Blue : System.Windows.Media.Brushes.Gray;
            }
        }
    }
}
