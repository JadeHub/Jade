using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JadeCore
{
    static public class GuiUtils
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

        public static bool PromptSaveFile(string ext, string filter, string defaultFilePath, out string result)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = defaultFilePath;
            dlg.DefaultExt = ext;
            dlg.Filter = filter;
            dlg.CheckFileExists = false;
            dlg.CheckPathExists = false;

            if (dlg.ShowDialog() == true)
            {
                result = dlg.FileName;
                return true;
            }
            result = "";
            return false;
        }

        public static JadeUtils.IO.IFileHandle PromptOpenFile(string ext, string filter, bool mustExist)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ext;
            dlg.Filter = filter;
            dlg.CheckFileExists = mustExist;
            dlg.CheckPathExists = mustExist;

            if(dlg.ShowDialog() == true)
            {
                return Services.Provider.FileService.MakeFileHandle(dlg.FileName);
            }
            return null;
        }
    }
}
