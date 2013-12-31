using JadeControls.EditorControl.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace JadeGui.DockingGui
{
    public class PanesStyleSelector : StyleSelector
    {
        public Style DocumentStyle
        {
            get;
            set;
        }

        public Style ToolStyle
        {
            get;
            set;
        }

        public override System.Windows.Style SelectStyle(object item, System.Windows.DependencyObject container)
        {
            if (item is DocumentViewModel)
                return DocumentStyle;

            if (item is JadeControls.DockingToolViewModel)
                return ToolStyle;

            return base.SelectStyle(item, container);
        }
    }
}
