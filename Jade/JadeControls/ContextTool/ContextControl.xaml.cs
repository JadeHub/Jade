using System;
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

namespace JadeControls.ContextTool
{
    /// <summary>
    /// Interaction logic for ContextControl.xaml
    /// </summary>
    public partial class ContextControl : UserControl
    {
        public ContextControl()
        {
            InitializeComponent();
        }

        private void TreeView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ContextPaneViewModel vm = DataContext as ContextPaneViewModel;
            vm.BrowseToSelectedSymbol();
        }
    }
}
