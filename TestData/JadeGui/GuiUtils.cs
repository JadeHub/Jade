using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JadeGui
{
    static internal class GuiUtils
    {
        public static bool ConfirmYNAction(string prompt)
        {
            return MessageBox.Show(prompt, "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        }

        public static bool DisplayModalWindow(Window dlg, object model)
        {
            dlg.DataContext = model;
            Nullable<bool> result = dlg.ShowDialog();
            return result.HasValue && result.Value == true;
        }
    }
}
