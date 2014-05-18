using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CppCodeBrowser;
using JadeUtils.IO;

namespace JadeCore.Project
{    
    public interface IProject : IFolder
    {
        /// <summary>
        /// Full path to the file that was opened. (.vcxproj or .jpj)
        /// </summary>
        FilePath Path { get; }

        /// <summary>
        /// Directory containing project file
        /// </summary>
        string Directory { get; }

        /// <summary>
        /// List of all source files in the project
        /// </summary>        
        Collections.Observable.List<IFileItem> SourceFiles { get; }

        /// <summary>
        /// Index builder for this project
        /// </summary>
        CppCodeBrowser.IndexBuilder IndexBuilder { get; }

        /// <summary>
        /// Add an item to an optional subfolder
        /// </summary>
        /// <param name="folder">Folder to contain item. null to place in project root.</param>
        /// <param name="item">Item to add</param>
        void AddItem(IFolder folder, IItem item);
        
        void OnItemRemoved(IItem item);
    }
}