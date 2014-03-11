using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadeCore.Workspace
{
    public class ProjectItem : IItem, Project.IProject
    {
        private Project.IProject _project;

        public ProjectItem(Project.IProject p)
        {
            this._project = p;
        }

        #region IItem interface

        public string Name { get { return _project.Name; } }

        #endregion

        #region IProject interface

        public IList<Project.IItem> Items { get { return _project.Items; } }
        public IList<Project.IFolder> Folders { get { return _project.Folders; } }
        public CppCodeBrowser.IProjectIndex SourceIndex { get { return _project.SourceIndex; } }
        public JadeCore.Project.IProject OwningProject { get { return _project.OwningProject; } }

        public void AddItem(JadeCore.Project.IItem item) { _project.AddItem(item); }
        public bool RemoveItem(string file) { return _project.RemoveItem(file); }
        public bool HasItem(string name) { return _project.HasItem(name); }
        public void AddFolder(JadeCore.Project.IFolder f) { _project.AddFolder(f); }
        public bool RemoveFolder(string name) { return _project.RemoveFolder(name); }
        public bool HasFolder(string name) { return _project.HasFolder(name); }
        public string Path { get { return _project.Path; } }
        public string Directory { get { return _project.Directory; } }
        public void OnItemAdded(JadeCore.Project.IItem item) { _project.OnItemAdded(item); }
        public void OnItemRemoved(JadeCore.Project.IItem item) { _project.OnItemRemoved(item); }

        #endregion
    }
}
