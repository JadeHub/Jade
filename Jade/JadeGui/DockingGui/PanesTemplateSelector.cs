using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.AvalonDock.Layout;

using JadeControls.EditorControl.ViewModel;
using JadeControls.OutputControl.ViewModel;
using JadeControls.SearchResultsControl.ViewModel;
using JadeControls.Workspace.ViewModel;

namespace JadeGui.DockingGui
{   
    /// <summary>
    /// Map item view models to DataTemplate objects
    /// </summary>
    class PanesTemplateSelector : DataTemplateSelector
    {
        public PanesTemplateSelector()
        {

        }
        
        public DataTemplate DocumentViewTemplate
        {
            get;
            set;
        }

        public DataTemplate WorkspaceViewTemplate
        {
            get;
            set;
        }

        public DataTemplate OutputViewTemplate
        {
            get;
            set;
        }

        public DataTemplate SearchResultsViewTemplate
        {
            get;
            set;
        }

        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            var itemAsLayoutContent = item as LayoutContent;
            
            if (item is DocumentViewModel)
                return DocumentViewTemplate;

            if (item is WorkspaceViewModel)
                return WorkspaceViewTemplate;

            if (item is OutputViewModel)
                return OutputViewTemplate;

            if (item is SearchResultsPaneViewModel)
                return SearchResultsViewTemplate;
            
            return base.SelectTemplate(item, container);
        }
    }
}
