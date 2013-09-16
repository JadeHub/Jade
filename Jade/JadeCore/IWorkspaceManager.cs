using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadeCore
{
    /*
    public interface IWorkspaceManager
    {
        event EventHandler WorkspaceOpened;

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
        /// Create a new workspace. Throws if WorkspaceOpen is true
        /// </summary>
        /// <param name="name"></param>
        /// <param name="path"></param>
        void NewWorkspace(string name, string path);

        /// <summary>
        /// Open a workspace file. Throws if WorkspaceOpen is true
        /// </summary>
        /// <param name="path">full path to .jws file</param>
        void OpenWorkspace(string path);

        /// <summary>
        /// Save the current workspace. Will use the current path or prompt the user if unknown.
        /// </summary>
        /// <returns>true if saved</returns>
        bool SaveWorkspace();

        /// <summary>
        /// Prompt the use user to either Save, discard or cancel.
        /// </summary>
        /// <returns>true if saved or discarded</returns>
        bool SaveOrDiscardWorkspace();

        /// <summary>
        /// Save the current workspace to the specified path.
        /// </summary>
        /// <returns>true if saved</returns>
        bool SaveWorkspaceAs(string path);

        ViewModels.IWorkspaceViewModel ViewModel { get; }
    }
     */
}
