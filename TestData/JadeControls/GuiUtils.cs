using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JadeControls
{
    static internal class GuiUtils
    {
        public static bool ConfirmYNAction(string prompt, string caption = "Confirmation")
        {
            return MessageBox.Show(prompt, caption, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        }

        public static MessageBoxResult PromptYesNoCancelQuestion(string prompt, string caption = "Confirmation")
        {
            return MessageBox.Show(prompt, caption, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
        }

        public static void DisplayErrorAlert(string prompt)
        {
            MessageBox.Show(prompt, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static bool DisplayModalWindow(Window dlg)
        {
            Nullable<bool> result = dlg.ShowDialog();
            return result.HasValue && result.Value == true;
        }

        public static bool DisplayModalWindow(Window dlg, object model)
        {
            dlg.DataContext = model;
            return DisplayModalWindow(dlg);
        }

        public static bool PromptUserInput(string prompt, out string result)
        {
            UserInput dlg = new UserInput(prompt);
            bool dlgResult = DisplayModalWindow(dlg);
            if (dlgResult)
                result = dlg.edtText.Text;
            else
                result = "";
            return dlgResult;
        }
    }
}
