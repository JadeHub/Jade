using System.Linq;
using System.Collections.Generic;
using Xceed.Wpf.AvalonDock.Layout;

namespace JadeGui.DockingGui
{
    class LayoutInitializer : ILayoutUpdateStrategy
    {
        public bool BeforeInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableToShow, ILayoutContainer destinationContainer)
        {           
            //Determine panel name for given view model type
            string destPaneName = string.Empty;
            if (anchorableToShow.Content is JadeControls.Workspace.ViewModel.WorkspaceViewModel ||
                anchorableToShow.Content is JadeControls.SymbolInspector.SymbolInspectorPaneViewModel ||
                anchorableToShow.Content is JadeControls.CursorInspector.CursorInspectorPaneViewModel)
            {
                destPaneName = "UpperLeftToolPanel";
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
