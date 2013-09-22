using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace JadeGui
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            EventManager.RegisterClassHandler(typeof(TreeViewItem), TreeViewItem.PreviewMouseRightButtonDownEvent, new RoutedEventHandler(TreeViewItem_PreviewMouseRightButtonDownEvent));

            base.OnStartup(e);

            MainWindow window = new MainWindow();

            //These services are required to create the MainViewModel
            JadeCore.Services.Provider.FileService = new JadeUtils.IO.FileService();
            JadeCore.Services.Provider.WorkspaceController = new WorkspaceController();
            JadeCore.Services.Provider.EditorController = new EditorController();

            //Create the main view model object
            var viewModel = new ViewModels.JadeViewModel();

            viewModel.RequestClose += delegate 
            { 
                window.Close(); 
            };
                    
            // Allow all controls in the window to 
            // bind to the ViewModel by setting the 
            // DataContext, which propagates down 
            // the element tree.
            window.DataContext = viewModel;
            viewModel.Commands.Bind(window.CommandBindings);
            window.Show();
        }

        private void TreeViewItem_PreviewMouseRightButtonDownEvent(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = VisualUpwardSearch<TreeViewItem>(e.OriginalSource as DependencyObject) as TreeViewItem;
            if (item.IsSelected == false)
            {
                item.IsSelected = true;
                e.Handled = true;
            }
        }

        static DependencyObject VisualUpwardSearch<T>(DependencyObject source)
        {
            while (source != null && source.GetType() != typeof(T))
                source = System.Windows.Media.VisualTreeHelper.GetParent(source);

            return source;
        }
    }
}
