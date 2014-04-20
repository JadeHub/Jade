using System.Collections.Generic;
using System.Linq;
using CppCodeBrowser;
using JadeUtils.IO;

namespace JadeCore.Project
{   
    public class Project : IProject
    {
        #region Data

        private string _name;
        private FilePath _path;
        private Dictionary<string, IItem> _items;
        private List<IFolder> _folders;
        //maintain a list of all source files in all folders
        private List<IItem> _allSourceFiles;

        private readonly IIndexBuilder _indexer;
        private bool _indexerInitialised;

        #endregion

        #region Constructor

        public Project(string name, FilePath path)
        {
            _path = path;
            _name = name;            
            _items = new Dictionary<string, IItem>();
            _folders = new List<IFolder>();
            _allSourceFiles = new List<IItem>();
            _indexer = new CppCodeBrowser.IndexBuilder(_name);
            _indexerInitialised = false;
        }

        #endregion

        #region IProject Implementation

        public string Name { get { return _name; } }
        public IList<IItem> Items { get { return _items.Values.ToList(); } }
        public IList<IFolder> Folders { get { return _folders; } }
        public IProject OwningProject { get { return this; } }

        public IProjectIndex SourceIndex 
        { 
            get 
            {
                if (!_indexerInitialised)
                    InitSourceIndex();
                return _indexer.Index;            
            } 
        }

        public string Path { get { return _path.Str; } }
        public string Directory { get { return _path.Directory; } }

        public IFileItem FindFileItem(FilePath path)
        {
            return FindFileItem(this, path);
        }

        private IFileItem FindFileItem(JadeCore.Project.IFolder folder, FilePath path)
        {
            foreach(IItem item in folder.Items)
            {
                if(item is FileItem)
                {
                    if ((item as FileItem).Path == path)
                        return item as FileItem;
                }
            }

            foreach(IFolder child in folder.Folders)
            {
                IFileItem result = FindFileItem(child, path);
                if (result != null)
                    return result;
            }

            return null;
        }

        public void AddItem(IItem item)
        {
            if (_items.ContainsKey(item.ItemName))
                throw new System.Exception("Duplicate item in project.");
            _items[item.ItemName] = item;
            OnItemAdded(item);
        }

        public bool RemoveItem(string itemName)
        {
            IItem item;

            if (_items.TryGetValue(itemName, out item))
            {
                _items.Remove(itemName);
                OnItemRemoved(item);
                return true;
            }
            return false;
        }

        public bool HasItem(string itemName)
        {
            return _items.ContainsKey(itemName);
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

        public IFolder FindFolder(string name)
        {
            foreach (IFolder f in _folders)
            {
                if (f.Name == name)
                {
                    return f;
                }
            }
            return null;
        }

        public void OnItemAdded(IItem item)
        {
            if (item is FileItem)
            {
                FileItem f = item as FileItem;
                if (f.Type == ItemType.CppSourceFile)
                {
                    AddSourceFile(f);
                }
            }
        }

        public void OnItemRemoved(IItem item)
        {
            if (item is FileItem)
            {
                FileItem f = item as FileItem;
                if (f.Type == ItemType.CppSourceFile)
                {
                    
                }
            }
        }

        private void AddSourceFile(FileItem f)
        {
            _allSourceFiles.Add(f);
            
        }

        private void RemoveSourceFile(FileItem f)
        {
            _allSourceFiles.Remove(f);            
        }

        private void InitSourceIndex()
        {
            if (_indexerInitialised) return;

            foreach(IItem item in _allSourceFiles)
            {
                _indexer.AddFile((item as IFileItem).Path, null);
            }
            _indexerInitialised = true;
        }

        #endregion
    }
}
