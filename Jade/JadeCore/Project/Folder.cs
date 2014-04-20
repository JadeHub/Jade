using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JadeUtils.IO;

namespace JadeCore.Project
{
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
            if (!HasItem(item.ItemName))
            {
                _items.Add(item);
                _project.OnItemAdded(item);
            }
        }

        public bool RemoveItem(string itemName)
        {
            foreach (IItem item in _items)
            {
                if (item.ItemName == itemName)
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

        public bool HasItem(string itemName)
        {
            foreach (IItem item in _items)
            {
                if (item.ItemName == itemName)
                {
                    return true;
                }
            }
            return false;
        }

        public IFileItem FindFileItem(FilePath path)
        {
            foreach (IItem item in _items)
            {
                if(item is IFileItem && (item as IFileItem).Path == path)
                {
                    return item as IFileItem;
                }
            }
            return null;
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

        #endregion
    }

}
