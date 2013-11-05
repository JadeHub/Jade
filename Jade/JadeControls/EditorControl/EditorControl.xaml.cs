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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JadeControls.EditorControl
{
    /// <summary>
    /// Interaction logic for EditorControl.xaml
    /// </summary>
    public partial class EditorControl : UserControl
    {
        public EditorControl()
        {
            InitializeComponent();
            this.DataContextChanged += EditorControl_DataContextChanged;
            
            //textEditor.ShowLineNumbers = true;
        }

        void EditorControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ((ViewModel.EditorControlViewModel)(DataContext)).Commands.Bind(CommandBindings);
        }

        private void TabCtrl_Loaded_1(object sender, RoutedEventArgs e)
        {

        }

        
    }
}