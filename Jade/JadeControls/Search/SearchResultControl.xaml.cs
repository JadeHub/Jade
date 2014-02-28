using System.Windows.Controls;

namespace JadeControls.SearchResultsControl 
{
    /// <summary>
    /// Interaction logic for SearchResultControl.xaml
    /// </summary>
    public partial class SearchResultsControl : UserControl
    {
        public SearchResultsControl()
        {
            InitializeComponent();
        }

        private void ListBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ViewModel.SearchResultsViewModel vm = (ViewModel.SearchResultsViewModel)DataContext;
            vm.OnDoubleClick();
        }
    }
}
