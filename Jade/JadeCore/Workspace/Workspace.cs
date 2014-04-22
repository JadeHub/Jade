using JadeUtils.IO;
using System.Collections.Generic;

namespace JadeCore.Workspace
{
    public class NewWorkspace : INewWorkspace
    {
        #region Data

        private Collections.Observable.List<Project.IProject> _projects;
        private Collections.Observable.List<string> _groups;

        #endregion

        public NewWorkspace(string name, FilePath path)
        {
            Name = name;
            Path = path;
            _projects = new Collections.Observable.List<Project.IProject>();
            _groups = new Collections.Observable.List<string>();
        }

        #region INewWorkspace

        public string Name { get; private set; }
        public FilePath Path { get; private set; }

        public Collections.Observable.List<Project.IProject> Projects 
        { 
            get 
            { 
                return _projects; 
            } 
        }

        public Collections.Observable.List<string> Groups 
        {
            get { return _groups; }
        }

        public void AddProject(string group, Project.IProject p)
        {
            if (!HasGroup(group))
                AddGroup(group);
            _projects.Add(p);
        }

        public void AddGroup(string group)
        {
            if (!HasGroup(group))
                _groups.Add(group);
        }

        private bool HasGroup(string group)
        {
            foreach(string g in _groups)
            {
                if (g == group)
                    return true;
            }
            return false;

        }

        #endregion
    }

    public class Workspace : IWorkspace
    {
        #region Data
        
        private string _name;
        private FilePath _path;
        private IFolder _rootFolder;
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

        public IEnumerable<Project.IProject> AllProjects
        {
            get
            {
                return _rootFolder.AllProjects;
            }
        }

        public string Path { get { return _path.Str; } set { } }

        public string Directory { get { return _path.Directory; } }

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
