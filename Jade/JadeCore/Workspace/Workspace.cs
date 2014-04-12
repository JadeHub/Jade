using JadeUtils.IO;
using System.Collections.Generic;

namespace JadeCore.Workspace
{
    public class Workspace : IWorkspace
    {
        #region Data
        
        private string _name;
        private FilePath _path;
        private Folder _rootFolder;
        private ITextDocumentCache _textDocCache;

        #endregion  

        #region Constructor

        public Workspace(string name, FilePath path)
        {
            _name = name;
            _path = path;
            _rootFolder = new Folder(_name);
            _textDocCache = new TextDocumentCache();
        }

        #endregion

        #region IWorkspace Implementation

        public string Path { get { return _path.Str; } set { } }

        public string Directory { get { return _path.Directory; } }

        public JadeCore.Project.IProject ActiveProject 
        { 
            get 
            {
                //todo - just return first project for now
                foreach (IItem item in _rootFolder.Items)
                {
                    if (item is Project.IProject)
                        return item as Project.IProject;
                }
                return null;
            }
        }

        public ITextDocumentCache DocumentCache 
        {
            get { return _textDocCache; }
        }

        public JadeCore.Project.IProject FindProjectForFile(FilePath path)
        {
            return _rootFolder.FindProjectForFile(path);
        }

        #endregion

        #region IFolder Implementation

        public string Name { get { return _name; } }
        public IList<IFolder> Folders { get { return _rootFolder.Folders; } }
        public IList<IItem> Items { get { return _rootFolder.Items; } }
        public void AddProject(Project.IProject p) { _rootFolder.AddProject(p); }
        public bool RemoveProject(string name) { return _rootFolder.RemoveProject(name); }
        public IFolder AddFolder(IFolder f) { return _rootFolder.AddFolder(f); }
        public bool RemoveFolder(string name) { return _rootFolder.RemoveFolder(name); }
        public bool HasFolder(string name) { return _rootFolder.HasFolder(name); }

        #endregion
    }
}
