using System.Collections.Generic;
using System.Linq;
using CppCodeBrowser;
using JadeUtils.IO;

namespace JadeCore.Project
{   
    public interface IProject : IFolder
    {
        string Path { get; }
        string Directory { get; }

        CppCodeBrowser.IProjectIndex SourceIndex { get; }

        File FindFile(FilePath path);

        void OnItemAdded(IItem item);
        void OnItemRemoved(IItem item);
    }

    public class Project : IProject
    {
        #region Data

        private string _name;
        private FilePath _path;
        private Dictionary<string, IItem> _items;
        private List<IFolder> _folders;
        //maintain a list of all source files in all folders
        private List<IItem> _allSourceFiles;

        private CppCodeBrowser.IProject _browserProject;

        #endregion

        #region Constructor

        public Project(string name, FilePath path)
        {
            _path = path;
            _name = name;            
            _items = new Dictionary<string, IItem>();
            _folders = new List<IFolder>();
            _allSourceFiles = new List<IItem>();
            _browserProject = new CppCodeBrowser.Project(_name, new CppCodeBrowser.IndexBuilder(_name));            
        }

        #endregion

        #region IProject Implementation

        public string Name { get { return _name; } }
        public IList<IItem> Items { get { return _items.Values.ToList(); } }
        public IList<IFolder> Folders { get { return _folders; } }
        public IProject OwningProject { get { return this; } }

        public IProjectIndex SourceIndex { get { return _browserProject.Index; } }

        public string Path { get { return _path.Str; } }
        public string Directory { get { return _path.Directory; } }

        public File FindFile(FilePath path)
        {
            foreach(IItem item in _items.Values)
            {
                if(item is File)
                {
                    if ((item as File).Path == path)
                        return item as File;
                }
            }
            return null;
        }

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
