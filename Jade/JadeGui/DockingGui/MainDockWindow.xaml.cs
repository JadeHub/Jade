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
using System.Windows.Shapes;
using JadeCore.Settings;

namespace JadeGui.DockingGui
{
    /// <summary>
    /// Interaction logic for MainDockWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Icon = new BitmapImage(new Uri("pack://application:,,,/Images/JadeIcon.ico", UriKind.Absolute));
           // Icon = new BitmapImage(new Uri("../../Resources/JadeIcon.ico", UriKind.Relative));
          //  Icon = new Bitmap(JadeGui.Resource1.JadeIcon);
        }
    
        public void RestoreWindowPosition(WindowPosition position)
        {
            this.Left = position.Origin.X;
            this.Top = position.Origin.Y;
            this.Width = position.Size.Width;
            this.Height = position.Size.Height;
            IsMaximised = position.Maximised;
        }

        public WindowPosition WindowPosition
        {
            get
            {
                WindowPosition pos = new WindowPosition();

                pos.Origin.X = (int)this.Left;
                pos.Origin.Y = (int)this.Top;
                pos.Size.Width = (int)this.Width;
                pos.Size.Height = (int)this.Height;
                pos.Maximised = IsMaximised;
                return pos;
            }
        }

        public bool IsMaximised
        {
            get
            {
                return WindowState == WindowState.Maximized;
            }
            set
            {
                if (value)
                {
                    WindowState = WindowState.Maximized;
                }
                else if (WindowState == WindowState.Maximized)
                {
                    WindowState = WindowState.Normal;
                }
            }        
        }

        private ViewModels.JadeViewModel ViewModel
        {
            get { return (ViewModels.JadeViewModel)this.DataContext; }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {            
            e.Cancel = !ViewModel.RequestExit();
        }
    }
}
