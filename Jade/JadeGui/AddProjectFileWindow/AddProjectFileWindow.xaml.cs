﻿using System;
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

namespace JadeGui
{
    /// <summary>
    /// Interaction logic for AddProjectFileWindow.xaml
    /// </summary>
    public partial class AddProjectFileWindow : Window
    {
        public AddProjectFileWindow()
        {
            InitializeComponent();
        }

        private void Button_Click_OK(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
