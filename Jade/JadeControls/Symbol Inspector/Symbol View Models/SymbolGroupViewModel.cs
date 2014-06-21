using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.Threading.Tasks;

namespace JadeControls.SymbolInspector
{
    public class GroupItemDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DefaultnDataTemplate { get; set; }
        public DataTemplate CtorDataTemplate { get; set; }
        public DataTemplate MethodDataTemplate { get; set; }
        public DataTemplate DataMemberDataTemplate { get; set; }
        public DataTemplate BaseTypeDataTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ConstructorViewModel)
            {
                return CtorDataTemplate;
            }

            if( item is MethodDeclarationViewModel)
            {
                return MethodDataTemplate;
            }

            if (item is DataMemberViewModel)
            {
                return DataMemberDataTemplate;
            }

            if (item is ClassDeclarationViewModel)
            {
                return BaseTypeDataTemplate;
            }
            
            return DefaultnDataTemplate;
        }
    }

    public class SymbolGroupViewModel : NotifyPropertyChanged
    {
        private string _name;
        private ObservableCollection<SymbolViewModelBase> _symbols;
        private bool _expanded;
        private SymbolViewModelBase _selectedSymbol;

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

        public SymbolViewModelBase SelectedSymbol
        {
            get { return _selectedSymbol; }
            set
            {
                if(_selectedSymbol != value)
                {
                    _selectedSymbol = value;
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

        public void OnDoubleClick()
        {
            if (SelectedSymbol == null) return;
            CppCodeBrowser.ICodeLocation loc = new CppCodeBrowser.CodeLocation(SelectedSymbol.SymbolCursor.Cursor.Location);
            if (loc == null) return;
            JadeCore.Services.Provider.CommandHandler.OnDisplayCodeLocation(new JadeCore.DisplayCodeLocationCommandParams(loc, true, true));
        }
    }
}
