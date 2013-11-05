using System.Collections.Generic;
using System.Linq;

namespace JadeCore.Project
{
    public enum ItemType
    {
        Unknown,
        CppSourceFile,
        CppHeaderFile
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
        private ItemType _type;
        
        #endregion

        #region Constructor

        public File(JadeUtils.IO.IFileHandle file)
        {
            _file = file;
            SetFileType();            
        }

        private void SetFileType()
        {
            string ext = _file.Path.Extention.ToLower();
            if (ext == ".c" || ext == ".cc" || ext == ".cpp")
            {
                _type = ItemType.CppSourceFile;
            }
            else if (ext == ".h")
            {
                _type = ItemType.CppHeaderFile;
            }
            else
            {
                _type = ItemType.Unknown;
            }
        }

        #endregion

        #region Public Properties

        public string Name { get { return _file.Name; } }
        public ItemType Type { get { return _type; } }
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
      //  IProject Project { get; }
    }

    public class Folder : IFolder
    {
        #region Data

        private IProject _project;
        private string _name;
        private List<IItem> _items;
        private List<IFolder> _folders;

        #endregion

        #region Constructor

        public Folder(IProject project, string name)
        {
            this._project = project;
            this._name = name;
            this._items = new List<IItem>();
            this._folders = new List<IFolder>();
        }

        #endregion

        #region IFolder Implementation

      //  public IProject Project { get { return _project; } }
        public string Name { get { return _name; } }
        public IList<IItem> Items { get { return _items; } }
        public IList<IFolder> Folders { get { return _folders; } }

        public void AddItem(IItem item)
        {
            if (!HasItem(item.Name))
            {
                _items.Add(item);
                _project.OnItemAdded(item);
            }
        }

        public bool RemoveItem(string name)
        {
            foreach (IItem item in _items)
            {
                if (item.Name == name)
                {
                    if (_items.Remove(item))
                    {
                        _project.OnItemRemoved(item);
                        return true;
                    }
                    return false;
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

        CppView.IProjectIndex SourceIndex { get; }

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

        private CppView.IProjectIndex _sourceIndex;
        private CppView.IIndexBuilder _indexBuilder;

        #endregion

        #region Constructor

        public Project(string name, JadeUtils.IO.IFileHandle file)
        {
            _file = file;
            _name = name;            
            _items = new Dictionary<string, IItem>();
            _folders = new List<IFolder>();
            _allSourceFiles = new List<IItem>();

            _sourceIndex = new CppView.ProjectIndex();
            _indexBuilder = new CppView.IndexBuilder(_sourceIndex);
        }

        #endregion

        #region IProject Implementation

        public string Name { get { return _name; } }
        public IList<IItem> Items { get { return _items.Values.ToList(); } }
        public IList<IFolder> Folders { get { return _folders; } }

        public CppView.IProjectIndex SourceIndex { get { return _sourceIndex; } }

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
                    _allSourceFiles.Add(f);
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
                    _allSourceFiles.Remove(f);
                }
            }
        }

        #endregion
    }
}
