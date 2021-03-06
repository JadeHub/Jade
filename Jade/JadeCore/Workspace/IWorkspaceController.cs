﻿using System;
using JadeUtils.IO;

namespace JadeCore.Workspace 
{
    public enum WorkspaceChangeOperation
    {
        Created,
        Opened,
        Closed,
        Saved
    }

    public delegate void WorkspaceChangeEventHandler(WorkspaceChangeOperation op);

    public interface IWorkspaceController
    {
        /// <summary>
        /// Raised when a workspace is created, opened, closed or saved.
        /// </summary>
        event WorkspaceChangeEventHandler WorkspaceChanged;
        
        void SaveSettings();

        /// <summary>
        /// Returns true if there is an open workspace
        /// </summary>
        bool WorkspaceOpen { get; }

        /// <summary>
        /// Returns true if the open workspace has been modified
        /// </summary>
        bool RequiresSave { get; }

        /// <summary>
        /// Close the open workspace. throws if RequiresSave is true
        /// </summary>
        bool CloseWorkspace();

        /// <summary>
        /// Close any open workspace and create a new, empty workspace with the given name.
        /// </summary>
        void NewWorkspace(string name);

        /// <summary>
        /// 
        /// </summary>
        void OpenWorkspace(FilePath path);

        /// <summary>
        /// Prompt the use user to either Save, discard or cancel.
        /// </summary>
        /// 
        /// <returns>true if saved or discarded</returns>
        bool SaveOrDiscardWorkspace();

        /// <summary>
        /// Save the current workspace to the specified path.
        /// </summary>
        /// <returns>true if saved</returns>
        bool SaveWorkspace(string path);

        /// <summary>
        /// Returns the current workspace
        /// </summary>
        IWorkspace CurrentWorkspace { get; }
                
        /// <summary>
        /// True if the current workspace has beeb modified
        /// </summary>
        bool CurrentWorkspaceModified { get; set; }

        RecentFileList RecentFiles { get; }

        ITextDocumentCache DocumentCache { get; }

     //   Parser.IWorkspaceIndexer Indexer { get; }
    }
}
