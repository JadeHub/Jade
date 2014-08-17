using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JadeUtils.IO;

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
        
        public JadeCore.Project.IProject OwningProject { get { return _project.OwningProject; } }
        public Collections.Observable.List<JadeCore.Project.IFileItem> SourceFiles { get { return _project.SourceFiles; } }
        public CppCodeBrowser.ProjectIndexBuilder IndexBuilder { get { return _project.IndexBuilder; } }
        public CppCodeBrowser.IProjectIndex Index { get { return _project.Index; } }
        public void AddItem(Project.IFolder folder, JadeCore.Project.IItem item) { _project.AddItem(folder, item); }
        public void AddItem(JadeCore.Project.IItem item) { _project.AddItem(item); }
        public bool RemoveItem(string itemName) { return _project.RemoveItem(itemName); }
        public bool HasItem(string itemName) { return _project.HasItem(itemName); }
        public void AddFolder(JadeCore.Project.IFolder f) { _project.AddFolder(f); }
        public bool RemoveFolder(string name) { return _project.RemoveFolder(name); }
        public JadeCore.Project.IFolder FindFolder(string name) { return _project.FindFolder(name); }
        public FilePath Path { get { return _project.Path; } }
        public string Directory { get { return _project.Directory; } }
        public void OnItemRemoved(JadeCore.Project.IItem item) { _project.OnItemRemoved(item); }
        public Project.IFileItem FindFileItem(FilePath path) { return _project.FindFileItem(path); }
        public IList<FilePath> Files { get { return _project.Files; } }
        #endregion
    }
}
