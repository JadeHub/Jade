using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadeCore.Workspace
{
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
}
