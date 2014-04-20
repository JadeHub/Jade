using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JadeUtils.IO;

namespace JadeCore.Project
{
    public interface IFolder
    {
        string Name { get; }
        IList<IFolder> Folders { get; }
        IList<IItem> Items { get; }
        void AddItem(IItem item);
        bool RemoveItem(string itemName);
        bool HasItem(string itemName);
        IFileItem FindFileItem(FilePath path);
        void AddFolder(IFolder f);
        bool RemoveFolder(string name);
        IFolder FindFolder(string name);
        IProject OwningProject { get; }
    }
}