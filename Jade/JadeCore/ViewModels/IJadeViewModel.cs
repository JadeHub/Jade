using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace JadeCore.ViewModels
{  
    public interface IJadeViewModel
    {        
        #region Commands

        void OnOpenDocument(JadeUtils.IO.IFileHandle file);

        /// <summary>
        /// Close the application.
        /// </summary>
      //  void OnExit();

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
