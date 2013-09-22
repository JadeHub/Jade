using System;
using System.Windows.Input;

namespace JadeCore
{
    public static class Commands
    {
        public static readonly RoutedCommand OpenDocument = new RoutedCommand("OpenDocument", typeof(object));        
        public static readonly RoutedCommand NewWorkspace = new RoutedCommand("NewWorkspace", typeof(object));
        public static readonly RoutedCommand CloseWorkspace = new RoutedCommand("CloseWorkspace", typeof(object));
        public static readonly RoutedCommand OpenWorkspace = new RoutedCommand("OpenWorkspace", typeof(object));
        public static readonly RoutedCommand SaveWorkspace = new RoutedCommand("SaveWorkspace", typeof(object));
        public static readonly RoutedCommand SaveAsWorkspace = new RoutedCommand("SaveAsWorkspace", typeof(object));
        public static readonly RoutedCommand Exit = new RoutedCommand("Exit", typeof(object));

        public static readonly RoutedCommand ViewLineNumbers = new RoutedCommand("ViewLineNumbers", typeof(object));
        public static readonly RoutedCommand CloseAllDocuments = new RoutedCommand("CloseAllDocuments", typeof(object));
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
        void OnOpenWorkspace();
        bool CanOpenWorkspace();

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

        void OnCloseAllDocuments();
        bool CanCloseAllDocuments();

        #endregion
    }
}
