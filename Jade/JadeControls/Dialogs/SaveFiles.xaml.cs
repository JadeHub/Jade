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

namespace JadeControls.Dialogs
{
    /// <summary>
    /// Interaction logic for SaveFiles.xaml
    /// </summary>
    public partial class SaveFiles : Window
    {
        private bool? _result;

        public SaveFiles()
        {
            InitializeComponent();
        }

        public bool? ShowDialog(object owner)
        {
            Owner = owner as Window;
            ShowDialog();
            return _result;
        }

        private void Button_Click_Yes(object sender, RoutedEventArgs e)
        {
            _result = true;
            Close();
        }

        private void Button_Click_No(object sender, RoutedEventArgs e)
        {
            _result = false;
            Close();
        }
    }
}
