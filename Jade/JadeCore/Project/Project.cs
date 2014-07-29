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
        private Collections.Observable.List<IFileItem> _allSourceFiles;
        private CppCodeBrowser.ProjectIndexBuilder _indexBuilder;

        private Symbols.ProjectTable _symbolTable;

        #endregion

        #region Constructor

        public Project(string name, FilePath path)
        {
            _path = path;
            _name = name;            
            _items = new Dictionary<string, IItem>();
            _folders = new List<IFolder>();
            _allSourceFiles = new Collections.Observable.List<IFileItem>();
            _indexBuilder = new CppCodeBrowser.ProjectIndexBuilder(delegate(FilePath p) {return FindFileItem(p) != null;},
                JadeCore.Services.Provider.GuiScheduler, JadeCore.Services.Provider.EditorController);

            _symbolTable = new Symbols.ProjectTable(this);
        }

        #endregion

        #region IProject Implementation

        public string Name { get { return _name; } }
        public IList<IItem> Items { get { return _items.Values.ToList(); } }
        public IList<IFolder> Folders { get { return _folders; } }
        public IProject OwningProject { get { return this; } }
        public Collections.Observable.List<IFileItem> SourceFiles { get { return _allSourceFiles; } }
        public CppCodeBrowser.ProjectIndexBuilder IndexBuilder { get { return _indexBuilder; } }
        public CppCodeBrowser.IProjectIndex Index { get { return _indexBuilder.Index; } }
        public FilePath Path { get { return _path; } }
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

        public void AddItem(IFolder folder, IItem item)
        {
            if (_items.ContainsKey(item.ItemName))
                throw new System.Exception("Duplicate item in project.");
            
            if (folder == null)
            {
                _items[item.ItemName] = item;
            }
            else
            {
                folder.AddItem(item);
            }

            if (item is FileItem)
            {
                FileItem f = item as FileItem;
                if (f.Type == ItemType.CppSourceFile)
                {
                    AddSourceFile(f);
                }
            }
        }

        public void AddItem(IItem item)
        {
            System.Diagnostics.Debug.Assert(false);
            /*
            if (_items.ContainsKey(item.ItemName))
                throw new System.Exception("Duplicate item in project.");
            _items[item.ItemName] = item;
            OnItemAdded(item);*/
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

        private void AddSourceFile(IFileItem f)
        {
            _allSourceFiles.Add(f);            
        }

        private void RemoveSourceFile(IFileItem f)
        {
            _allSourceFiles.Remove(f);            
        }

        #endregion
    }
}
