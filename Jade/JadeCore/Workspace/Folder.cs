using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JadeUtils.IO;

namespace JadeCore.Workspace
{
    public class Folder : IFolder
    {
        #region Data

        private readonly string name;
        private List<IFolder> folders;
        private List<IItem> items;

        #endregion

        public Folder(string name)
        {
            this.name = name;
            this.folders = new List<IFolder>();
            this.items = new List<IItem>();
        }

        #region Properties

        public string Name { get { return name; } }
        public IList<IFolder> Folders { get { return folders; } }
        public IList<IItem> Items { get { return items; } }
        
        #endregion

        #region Public Methods

        public void AddProject(Project.IProject p)
        {
            items.Add(new ProjectItem(p));
        }

        public bool RemoveProject(string name)
        {
            foreach (IItem i in items)
            {
                if (i is ProjectItem && i.Name == name)
                {
                    items.Remove(i);
                    return true;
                }
            }
            return false;
        }

        public IFolder AddFolder(IFolder f)
        {
            folders.Add(f);
            return f;
        }

        public bool RemoveFolder(string name)
        {
            foreach (IFolder f in folders)
            {
                if (f.Name == name)
                {
                    return folders.Remove(f);
                }
            }
            return false;
        }

        public bool HasFolder(string name)
        {
            foreach (IFolder f in folders)
            {
                if (f.Name == name)
                {
                    return true;
                }
            }
            return false;
        }

        public JadeCore.Project.IProject FindProjectForFile(FilePath path)
        {
            JadeCore.Project.IProject result = null;
            foreach (IItem item in Items)
            {
                if (item is Project.IProject)
                {
                    if((item as Project.IProject).FindFileItem(path) != null)
                        return item as Project.IProject;
                }
            }

            foreach(IFolder folder in Folders)
            {
                result = folder.FindProjectForFile(path);
                if (result != null)
                    return result;
            }

            return result;
        }

        #endregion
    }
}
