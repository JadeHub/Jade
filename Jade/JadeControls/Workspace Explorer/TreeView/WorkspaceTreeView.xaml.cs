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

namespace JadeControls.Workspace
{
    /// <summary>
    /// Interaction logic for WorkspaceTreeView.xaml
    /// </summary>
    public partial class WorkspaceTreeView
    {
        public WorkspaceTreeView()
        {
            InitializeComponent();

            //CommandBindings.Add(new CommandBinding(ApplicationCommands.Undo, ExecuteUndo, CanExecuteUndo));
            CommandBindings.AddRange(ViewModel.TreeViewCommandAdaptor.CommandBindings);
        }

        void ExecuteUndo(object sender, ExecutedRoutedEventArgs e)
        {
            
        }

        void CanExecuteUndo(object sender, CanExecuteRoutedEventArgs e)
        {           
            e.Handled = true;
            e.CanExecute = true;
        }

        private void TreeView_MouseDoubleClick_1(object sender, MouseButtonEventArgs e)
        {
            ViewModel.WorkspaceTree w = DataContext as ViewModel.WorkspaceTree;
            w.OnOpenDocument();
        }
		
    }
}
