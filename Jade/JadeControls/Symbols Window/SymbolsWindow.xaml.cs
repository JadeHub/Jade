using System.Windows;

namespace JadeControls.Symbols
{
    /// <summary>
    /// Interaction logic for SymbolsWindow.xaml
    /// </summary>
    public partial class SymbolsWindow : Window
    {
        public SymbolsWindow()
        {
            InitializeComponent();
            DeclListCtrl.MouseDoubleClick += DeclListCtrl_MouseDoubleClick;
            RefListCtrl.MouseDoubleClick += RefListCtrl_MouseDoubleClick;
        }
                
        void RefListCtrl_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            JadeControls.Symbols.SymbolsWindowViewModel vm = (JadeControls.Symbols.SymbolsWindowViewModel)DataContext;
            vm.OnRefDoubleClick(vm.SelectedReference);
        }

        void DeclListCtrl_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            JadeControls.Symbols.SymbolsWindowViewModel vm = (JadeControls.Symbols.SymbolsWindowViewModel)DataContext;
            vm.OnDeclDoubleClick(vm.SelectedDeclaration, this);
        }
    }
}
