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
            this.DataContextChanged += SearchResultsControlDataContextChanged;
        }

        void SearchResultsControlDataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if(DataContext != null && DataContext is ViewModel.SearchResultsPaneViewModel)
            {
                (DataContext as ViewModel.SearchResultsPaneViewModel).StartNewCurrentFileSearch += delegate
                {
                    CurrentFileTestSearchEdit.Focus();
                };
            }
        }

        private void ListBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ViewModel.SearchResultsPaneViewModel vm = (ViewModel.SearchResultsPaneViewModel)DataContext;
            vm.OnDoubleClick();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
