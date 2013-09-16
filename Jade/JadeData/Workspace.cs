using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadeData.Workspace
{
    public interface IItem
    {
        string Name { get; }
    }

    public interface IFolder
    {
        string Name { get; }
        IList<IFolder> Folders { get; }
        IList<IItem> Items { get; }
        void AddProject(Project.IProject p);
        bool RemoveProject(string name);
        IFolder AddFolder(IFolder f);
        bool RemoveFolder(string name);
        bool HasFolder(string name);
    }

    public interface IWorkspace : IFolder
    {
        string Path { get; set; }
        string Directory { get; }
    }

    public class Folder : IFolder
    {
        private readonly string name;
        private List<IFolder> folders;
        private List<IItem> items;

        public Folder(string name)
        {
            this.name = name;
            this.folders = new List<IFolder>();
            this.items = new List<IItem>();
        }

        public string Name { get { return name; } }
        public IList<IFolder> Folders { get { return folders; } }
        public IList<IItem> Items { get { return items; } }

        public void AddProject(Project.IProject p)
        {
            items.Add(new ProjectItem(p));
        }

        public bool RemoveProject(string name)
        {
            foreach (IItem i in items)
            {
                if (i is ProjectItem && i.Name == name)
                {
                    items.Remove(i);
                    return true;
                }
            }
            return false;
        }

        public IFolder AddFolder(IFolder f)
        {
            folders.Add(f);
            return f;
        }

        public bool RemoveFolder(string name)
        {
            foreach (IFolder f in folders)
            {
                if (f.Name == name)
                {
                    return folders.Remove(f);
                }
            }
            return false;
        }

        public bool HasFolder(string name)
        {
            foreach (IFolder f in folders)
            {
                if (f.Name == name)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public class ProjectItem : IItem, Project.IProject
    {
        private Project.IProject _project;

        public ProjectItem(Project.IProject p)
        {
            this._project = p;
        }

        #region IItem interface

        public string Name { get { return _project.Name; } }

        #endregion

        #region IProject interface

        public IList<Project.IItem> Items { get { return _project.Items; } }
        public IList<Project.IFolder> Folders { get { return _project.Folders; } }

        public void AddItem(JadeData.Project.IItem item) { _project.AddItem(item); }
        public bool RemoveItem(string file) { return _project.RemoveItem(file); }
        public bool HasItem(string name) { return _project.HasItem(name); }
        public void AddFolder(JadeData.Project.IFolder f) {_project.AddFolder(f);}
        public bool RemoveFolder(string name) { return _project.RemoveFolder(name); }
        public bool HasFolder(string name) { return _project.HasFolder(name); }
        public string Path { get { return _project.Path; } }
        public string Directory { get { return _project.Directory; } }

        #endregion
    }

    public class Workspace : IWorkspace
    {
        #region Data
        
        private string _name;
        private string _path; //Full path to workspace file
        private string _directory; //Dir containing workspace file
        private Folder _rootFolder;

        #endregion

        public Workspace(string name, string path)
        {
            _name = name;
            _path = path;
            if(_path.Length > 0)
                _directory = System.IO.Path.GetDirectoryName(path);
            _rootFolder = new Folder(_name);
        }

        #region IWorkspace Implementation

        public string Path 
        { 
            get 
            { 
                return _path; 
            }
            set
            {
                _path = value;
            }
        }

        public string Directory { get { return _directory; } }

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
