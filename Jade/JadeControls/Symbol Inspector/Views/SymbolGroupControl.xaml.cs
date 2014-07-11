using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JadeControls.SymbolInspector
{
    /// <summary>
    /// Interaction logic for SymbolGroupControl.xaml
    /// </summary>
    public partial class SymbolGroupControl : UserControl
    {
        public SymbolGroupControl()
        {
            InitializeComponent();
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {

        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Debug.Assert(DataContext is SymbolGroupViewModel);
            SymbolGroupViewModel vm = DataContext as SymbolGroupViewModel;
            vm.OnDoubleClick();
            //Debug.Assert(vm.SelectedSymbol != null);
            //vm.SelectedSymbol.
        }
    }
}
