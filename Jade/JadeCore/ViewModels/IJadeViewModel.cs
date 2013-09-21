using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace JadeCore.ViewModels
{
    public interface IEditorDocument
    {
        string DisplayName
        {
            get;
        }
    }

    public interface IEditorViewModel
    {
        void OpenSourceFile(IO.IFileHandle file);
        void CloseAllDocuments();
        ObservableCollection<IEditorDocument> OpenDocuments { get; }
    }

    public interface IWorkspaceViewModel
    {
        string Name { get; }
        string Path { get; set; }
        string Directory { get; }
        bool Modified { get; set; }
    }

    public interface IJadeViewModel
    {
   /*     IWorkspaceViewModel Workspace
        {
            get;
      //      set;
        }*/

        IEditorViewModel Editor
        {
            get;
        }
        
        #region Commands

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
