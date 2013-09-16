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

namespace JadeControls.OpenDocuments
{
    /// <summary>
    /// Interaction logic for Control.xaml
    /// </summary>
    public partial class OpenDocList : UserControl
    {
        public OpenDocList()
        {
            InitializeComponent();
        }

        private void tb_Loaded_1(object sender, RoutedEventArgs e)
        {
            Border rootElement = sender as Border;
            ContentPresenter cp = rootElement.TemplatedParent as ContentPresenter;
            cp.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
        }

        private void expander_Loaded_1(object sender, RoutedEventArgs e)
        {
            Border rootElement = sender as Border;
            ContentPresenter cp = rootElement.TemplatedParent as ContentPresenter;
            cp.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
        }
    }
}
