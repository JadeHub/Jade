using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace JadeData.Project
{
    public enum ItemType
    {
        File
    }

    public interface IItem
    {
        string Name { get; }
        ItemType Type { get; }
    }
    
    public class File : IItem
    {
        #region Data 

        private JadeUtils.IO.IFileHandle _file;
        
        #endregion

        #region Constructor

        public File(JadeUtils.IO.IFileHandle file)
        {
            _file = file;
        }

        #endregion

        #region Public Properties

        public string Name { get { return _file.Name; } }
        public ItemType Type { get { return ItemType.File; } }
        public string Path { get { return _file.Path.Str; } }
        public JadeUtils.IO.IFileHandle Handle { get { return _file; } }

        #endregion
    }

    public interface IFolder
    {
        string Name { get; }
        IList<IFolder> Folders { get; }
        IList<IItem> Items { get; }
        void AddItem(IItem item);
        bool RemoveItem(string name);
        bool HasItem(string name);
        void AddFolder(IFolder f);
        bool RemoveFolder(string name);
        bool HasFolder(string name);
    }

    public class Folder : IFolder
    {
        #region Data

        private string _name;
        private List<IItem> _items;
        private List<IFolder> _folders;

        #endregion

        #region Constructor

        public Folder(string name)
        {
            this._name = name;
            this._items = new List<IItem>();
            this._folders = new List<IFolder>();
        }

        #endregion

        #region IFolder Implementation

        public string Name { get { return _name; } }
        public IList<IItem> Items { get { return _items; } }
        public IList<IFolder> Folders { get { return _folders; } }

        public void AddItem(IItem item)
        {
            _items.Add(item);
        }

        public bool RemoveItem(string name)
        {
            foreach (IItem item in _items)
            {
                if (item.Name == name)
                {
                    return _items.Remove(item);
                }
            }
            return false;
        }

        public bool HasItem(string name)
        {
            foreach (IItem item in _items)
            {
                if (item.Name == name)
                {
                    return true;
                }
            }
            return false;
        }

        public void AddFolder(IFolder folder)
        {
            _folders.Add(folder);
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

        #endregion
    }

    public interface IProject : IFolder
    {
        string Path { get; }
        string Directory { get; }
    }

    public class Project : IProject
    {
        #region Data

        private string _name;
        private JadeUtils.IO.IFileHandle _file;
        private Dictionary<string, IItem> _items;
        private List<IFolder> _folders;

        #endregion

        #region Constructor

        public Project(string name, JadeUtils.IO.IFileHandle file)
        {
            _file = file;
            _name = name;            
            _items = new Dictionary<string, IItem>();
            _folders = new List<IFolder>();
        }

        #endregion

        #region IProject Implementation

        public string Name { get { return _name; } }
        public IList<IItem> Items { get { return _items.Values.ToList(); } }
        public IList<IFolder> Folders { get { return _folders; } }

        public string Path { get { return _file.Path.Str; } }
        public string Directory { get { return _file.Path.Directory; } }

        public void AddItem(IItem item)
        {
            if (_items.ContainsKey(item.Name))
                throw new System.Exception("Duplicate item in project.");
            _items[item.Name] = item;
        }

        public bool RemoveItem(string name)
        {
            return _items.Remove(name);            
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

        #endregion
    }
}
