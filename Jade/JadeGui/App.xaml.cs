using System;
using System.Windows;
using System.Windows.Controls;

namespace JadeGui
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private DockingGui.MainWindow _mainWindow;

        protected override void OnStartup(StartupEventArgs e)
        {
            EventManager.RegisterClassHandler(typeof(TreeViewItem), TreeViewItem.PreviewMouseRightButtonDownEvent, new RoutedEventHandler(TreeViewItem_PreviewMouseRightButtonDownEvent));

            base.OnStartup(e);
            
            //These services are required to create the MainViewModel
            JadeCore.Services.Provider.ContentProvider = new JadeCore.ContentProvider();
            JadeCore.Services.Provider.FileService = new JadeUtils.IO.FileService();
            JadeCore.Services.Provider.WorkspaceController = new JadeCore.Workspace.WorkspaceController();
            JadeCore.Services.Provider.EditorController = new JadeCore.Editor.EditorController();
            JadeCore.Services.Provider.OutputController = new JadeCore.Output.OutputController();
            JadeCore.Services.Provider.SearchController = new JadeCore.Search.SearchController();

            _mainWindow = new DockingGui.MainWindow();

            JadeCore.Services.Provider.MainWindow = _mainWindow;

            //Create the main view model object
            var viewModel = new ViewModels.JadeViewModel(_mainWindow);

            //the view model is this service - todo fix dependancies
            JadeCore.Services.Provider.CommandHandler = viewModel;

            viewModel.RequestClose += delegate 
            {
                _mainWindow.Close(); 
            };

            // Allow all controls in the _mainWindow to 
            // bind to the ViewModel by setting the 
            // DataContext, which propagates down 
            // the element tree.
            _mainWindow.DataContext = viewModel;

            //bind commands
            viewModel.Commands.Bind(_mainWindow.CommandBindings);           
            
            JadeCore.Services.Provider.OutputController.Create(JadeCore.Output.Source.JadeDebug, JadeCore.Output.Level.Info, "Hello world");
            
            JadeCore.Properties.Settings settings = JadeCore.Services.Provider.Settings;
            if (settings.MainWindowPosition != null)
            {
                _mainWindow.RestoreWindowPosition(settings.MainWindowPosition);
            }

            _mainWindow.Closed += _window_Closed;
            _mainWindow.Show();

            //viewModel.OnOpenWorkspace(@"C:\Code\GitHub\JadeMaster\TestData\CppTest\CppTest.jws");
            viewModel.OnOpenWorkspace(@"C:\Code\GitHub\JadeMaster\TestData\CppTest\CppTest\CppTest.sln");
        }

        protected override void OnExit(ExitEventArgs e)
        {
            JadeCore.Services.Provider.Settings.Save();
            base.OnExit(e);
        }

        void _window_Closed(object sender, EventArgs e)
        {
            JadeCore.Services.Provider.Settings.MainWindowPosition = _mainWindow.WindowPosition;
            JadeCore.Services.Provider.WorkspaceController.SaveSettings();
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
