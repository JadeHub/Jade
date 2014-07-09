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

namespace JadeControls.SymbolInspector
{
    /// <summary>
    /// Interaction logic for SymbolInspector.xaml
    /// </summary>
    public partial class SymbolInspector : UserControl
    {
        public SymbolInspector()
        {
            InitializeComponent();
        }

        private void ToolBar_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ToolBar toolBar = sender as ToolBar;
            var overflowGrid = toolBar.Template.FindName("OverflowGrid", toolBar) as FrameworkElement;
            if (overflowGrid != null)
            {
                overflowGrid.Visibility = toolBar.HasOverflowItems ? Visibility.Visible : Visibility.Collapsed;
            }

            var mainPanelBorder = toolBar.Template.FindName("MainPanelBorder", toolBar) as FrameworkElement;
            if (mainPanelBorder != null)
            {
                var defaultMargin = new Thickness(0, 0, 11, 0);
                mainPanelBorder.Margin = toolBar.HasOverflowItems ? defaultMargin : new Thickness(0);
            }
        }
    }
}
