using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadeCore.Project
{
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
        IProject OwningProject { get; }
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
        public IProject OwningProject { get { return _project; } }

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

}
