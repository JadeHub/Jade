using System;
using System.Windows.Input;

namespace JadeCore
{
    public static class Commands
    {
        public static readonly RoutedCommand OpenDocument = new RoutedCommand("OpenDocument", typeof(object));        
        public static readonly RoutedCommand NewWorkspace = new RoutedCommand("NewWorkspace", typeof(object));
        public static readonly RoutedCommand CloseWorkspace = new RoutedCommand("CloseWorkspace", typeof(object));
        public static readonly RoutedCommand PromptOpenWorkspace = new RoutedCommand("PromptOpenWorkspace", typeof(object));
        public static readonly RoutedCommand OpenWorkspace = new RoutedCommand("OpenWorkspace", typeof(object));
        public static readonly RoutedCommand SaveWorkspace = new RoutedCommand("SaveWorkspace", typeof(object));
        public static readonly RoutedCommand SaveAsWorkspace = new RoutedCommand("SaveAsWorkspace", typeof(object));
        public static readonly RoutedCommand SaveAllFiles = new RoutedCommand("SaveAllFiles", typeof(object));
        public static readonly RoutedCommand CloseFile = new RoutedCommand("CloseFile", typeof(object));
        public static readonly RoutedCommand Exit = new RoutedCommand("Exit", typeof(object));

        public static readonly RoutedCommand ViewLineNumbers = new RoutedCommand("ViewLineNumbers", typeof(object));
        public static readonly RoutedCommand CloseAllDocuments = new RoutedCommand("CloseAllDocuments", typeof(object));

        public static readonly RoutedCommand DisplayCodeLocation = new RoutedCommand("DisplayCodeLocation", typeof(object));
               
        static Commands()
        {
            PromptOpenWorkspace.InputGestures.Add(new KeyGesture(Key.O, ModifierKeys.Shift | ModifierKeys.Control));
            NewWorkspace.InputGestures.Add(new KeyGesture(Key.N, ModifierKeys.Shift | ModifierKeys.Control));
        }
    }

    public class DisplayCodeLocationParams
    {
        public CppCodeBrowser.ICodeLocation Location
        {
            get;
            private set;
        }

        public bool SetFocus
        {
            get;
            private set;
        }

        public DisplayCodeLocationParams(CppCodeBrowser.ICodeLocation location, bool setFocus)
        {
            Location = location;
            SetFocus = setFocus;
        }
    }

    public interface IJadeCommandHandler
    {
        #region Commands

        void OnOpenDocument(JadeUtils.IO.IFileHandle file);

        /// <summary>
        /// Close the application.
        /// </summary>
        void OnExit();

        /// <summary>
        /// Create a new Workspace.
        /// </summary>
        void OnNewWorkspace();

        /// <summary>
        /// Close the current workspace.
        /// </summary>
        void OnCloseWorkspace();
        bool CanCloseWorkspace();

        /// <summary>
        /// Open an existing Workspace.
        /// </summary>
        void OnOpenWorkspace(string path);
        bool CanOpenWorkspace();

        /// <summary>
        /// Prompt the user for a file name and open existing Workspace.
        /// </summary>
        void OnPromptOpenWorkspace();
        bool CanPromptOpenWorkspace();

        /// <summary>
        /// Save the current Workspace.
        /// </summary>
        void OnSaveWorkspace();
        bool CanSaveWorkspace();

        /// <summary>
        /// Save the current Workspace.
        /// </summary>
        void OnSaveAsWorkspace();
        bool CanSaveAsWorkspace();

        /// <summary>
        /// Toggle Line number display.
        /// </summary>
        void OnViewLineNumbers();
        bool CanViewLineNumbers();

        void OnNewFile();
        bool CanNewFile();

        void OnOpenFile();
        bool CanOpenFile();

        void OnSaveFile();
        bool CanSaveFile();

        void OnSaveAsFile();
        bool CanSaveAsFile();

        void OnSaveAllFiles();
        bool CanSaveAllFiles();

        void OnCloseFile();
        bool CanCloseFile();

        void OnCloseAllDocuments();
        bool CanCloseAllDocuments();

        void OnDisplayCodeLocation(DisplayCodeLocationParams param);

        #endregion
    }
}
