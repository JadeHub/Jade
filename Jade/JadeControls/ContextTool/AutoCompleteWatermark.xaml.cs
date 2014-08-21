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
    /// Interaction logic for AutoCompleteWatermark.xaml
    /// </summary>
    public partial class AutoCompleteWatermark : UserControl
    {
        private bool _watermarked;
        private string _watermark;

        public AutoCompleteWatermark()
        {
            InitializeComponent();
            _watermarked = false;
         //   DisplayWaterMark(true);
            autocompleteBox.Text = "prompt";
        }

        public string WatermarkText
        {
            //get { return _watermark; }
            set
            {
                _watermark = value;
                if (_watermarked)
                {
                //    autocompleteBox.Text = _watermark;
                  //  autocompleteBox.Text = "";
                }
            }
        }

        private void autocompleteBox_LostFocus(object sender, RoutedEventArgs e)
        {
            //DisplayWaterMark(true);
         //   if(autocompleteBox.Text.Length > 0)
            //    autocompleteBox.Text = "prompt";

        //    autocompleteBox.Text = "";
        }

        private void DisplayWaterMark(bool visible)
        {
            if (visible && !_watermarked && string.IsNullOrEmpty(autocompleteBox.Text))
            {
               // autocompleteBox.Text = _watermark;
                autocompleteBox.Foreground = System.Windows.Media.Brushes.DarkGray;
                _watermarked = true;
            }

            if(!visible && _watermarked)
            {
                WatermarkText = "";
                                
                autocompleteBox.Foreground = System.Windows.Media.Brushes.Black;
                _watermarked = false;
            }
        }

        private void autocompleteBox_GotFocus(object sender, RoutedEventArgs e)
        {
            //DisplayWaterMark(false);
            autocompleteBox.Text = "";
        }
    }
}
