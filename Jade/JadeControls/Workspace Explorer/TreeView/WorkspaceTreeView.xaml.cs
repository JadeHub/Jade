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

            CommandBindings.Add(new CommandBinding(ApplicationCommands.Undo, ExecuteUndo, CanExecuteUndo));
        }

        void ExecuteUndo(object sender, ExecutedRoutedEventArgs e)
        {
            
        }

        void CanExecuteUndo(object sender, CanExecuteRoutedEventArgs e)
        {
           
                e.Handled = true;
                e.CanExecute = true;
        }
		
    }
}
