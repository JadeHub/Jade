using System.Linq;
using Xceed.Wpf.AvalonDock.Layout;

namespace JadeGui.DockingGui
{
    class LayoutInitializer : ILayoutUpdateStrategy
    {
        public bool BeforeInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableToShow, ILayoutContainer destinationContainer)
        {
            /*//AD wants to add the anchorable into destinationContainer
            //just for test provide a new anchorablepane 
            //if the pane is floating let the manager go ahead
            LayoutAnchorablePane destPane = destinationContainer as LayoutAnchorablePane;
            if (destinationContainer != null &&
                destinationContainer.FindParent<LayoutFloatingWindow>() != null)
                return false;

            var toolsPane = layout.Descendents().OfType<LayoutAnchorablePane>().FirstOrDefault(d => d.Name == "ToolsPane");
            if (toolsPane != null)
            {
                toolsPane.Children.Add(anchorableToShow);
                return true;
            }

            return false;*/

            //Determine panel name for given view model type
            string destPaneName = string.Empty;
            if (anchorableToShow.Content is JadeControls.Workspace.ViewModel.WorkspaceViewModel)
            {
                destPaneName = "LeftToolPanel";
            }
            else if (anchorableToShow.Content is JadeControls.OutputControl.ViewModel.OutputViewModel ||
                    anchorableToShow.Content is JadeControls.SearchResultsControl.ViewModel.SearchResultsPaneViewModel)
            {
                destPaneName = "LowerToolPanel";
            }
            else
            {
                return false;
            }

            //Find pane
            var toolsPane = layout.Descendents().OfType<LayoutAnchorablePane>().FirstOrDefault(d => d.Name == destPaneName);
            if (toolsPane != null)
            {
                //Add
                toolsPane.Children.Add(anchorableToShow);
                return true;
            }
            return false;

        }


        public void AfterInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableShown)
        {
        }


        public bool BeforeInsertDocument(LayoutRoot layout, LayoutDocument anchorableToShow, ILayoutContainer destinationContainer)
        {
            return false;
        }

        public void AfterInsertDocument(LayoutRoot layout, LayoutDocument anchorableShown)
        {

        }
    }
}
