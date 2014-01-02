using System.Collections.Generic;
using System.Linq;
using CppCodeBrowser;

namespace JadeCore.Project
{   
    public interface IProject : IFolder
    {
        string Path { get; }
        string Directory { get; }

        IProjectIndex SourceIndex { get; }

        void OnItemAdded(IItem item);
        void OnItemRemoved(IItem item);
    }

    public class Project : IProject
    {
        #region Data

        private string _name;
        private JadeUtils.IO.IFileHandle _file;
        private Dictionary<string, IItem> _items;
        private List<IFolder> _folders;
        //maintain a list of all source files in all folders
        private List<IItem> _allSourceFiles;

        private CppCodeBrowser.IProject _browserProject;

        #endregion

        #region Constructor

        public Project(string name, JadeUtils.IO.IFileHandle file)
        {
            _file = file;
            _name = name;            
            _items = new Dictionary<string, IItem>();
            _folders = new List<IFolder>();
            _allSourceFiles = new List<IItem>();

            _browserProject = new CppCodeBrowser.Project(_name, new CppCodeBrowser.IndexBuilder());            
        }

        #endregion

        #region IProject Implementation

        public string Name { get { return _name; } }
        public IList<IItem> Items { get { return _items.Values.ToList(); } }
        public IList<IFolder> Folders { get { return _folders; } }
        public IProject OwningProject { get { return this; } }

        public IProjectIndex SourceIndex { get { return _browserProject.Index; } }

        public string Path { get { return _file.Path.Str; } }
        public string Directory { get { return _file.Path.Directory; } }

        public void AddItem(IItem item)
        {
            if (_items.ContainsKey(item.Name))
                throw new System.Exception("Duplicate item in project.");
            _items[item.Name] = item;
            OnItemAdded(item);
        }

        public bool RemoveItem(string name)
        {
            IItem item;

            if (_items.TryGetValue(name, out item))
            {
                _items.Remove(name);
                OnItemRemoved(item);
                return true;
            }
            return false;
        }

        public bool HasItem(string name)
        {
            return _items.ContainsKey(name);
        }

        public void AddFolder(IFolder f)
        {
            _folders.Add(f);
        }

        public bool RemoveFolder(string name)
        {
            foreach (IFolder f in _folders)
            {
                if (f.Name == name)
                {
                    return _folders.Remove(f);
                }
            }
            return false;
        }

        public bool HasFolder(string name)
        {
            foreach (IFolder f in _folders)
            {
                if (f.Name == name)
                {
                    return true;
                }
            }
            return false;
        }

        public void OnItemAdded(IItem item)
        {
            if (item is File)
            {
                File f = item as File;
                if (f.Type == ItemType.CppSourceFile)
                {
                    AddSourceFile(f);
                }
            }
        }

        public void OnItemRemoved(IItem item)
        {
            if (item is File)
            {
                File f = item as File;
                if (f.Type == ItemType.CppSourceFile)
                {
                    
                }
            }
        }

        private void AddSourceFile(File f)
        {
            _allSourceFiles.Add(f);
            _browserProject.AddSourceFile(f.Path, null);
            //_indexBuilder.AddSourceFile(f.Handle, CppView.IndexBuilderItemPriority.Immediate);
            //_sourceIndex.Dump();
        }

        private void RemoveSourceFile(File f)
        {
            _allSourceFiles.Remove(f);

            
        }

        #endregion
    }
}
