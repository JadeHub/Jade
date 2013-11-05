using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows;

namespace JadeControls.Workspace.ViewModel
{
    public class WorkspaceFolder : TreeNodeBase
    {
        #region Data

        private JadeCore.Workspace.IFolder _data;

        #endregion

        #region Constructor

        public WorkspaceFolder(TreeNodeBase parent, JadeCore.Workspace.IFolder data)
            : base(data.Name, parent)
        {
            _data = data;
            foreach (JadeCore.Workspace.IFolder f in _data.Folders)
            {
                WorkspaceFolder folder = new WorkspaceFolder(this, f);
                AddChildFolder(f);
            }

            foreach (JadeCore.Workspace.IItem item in _data.Items)
            {
                if (item is JadeCore.Project.IProject)
                {
                    AddChildProject(item as JadeCore.Project.IProject);
                }
            }
        }

        #endregion

        #region Private Methods

        protected override bool RemoveChildData(TreeNodeBase child)
        {
            if (child is WorkspaceFolder)
            {
                return _data.RemoveFolder(child.DisplayName);
            }
            else if (child is ProjectFolder)
            {
                return _data.RemoveProject(child.DisplayName);
            }
            return false;
        }

        public void AddNewChildFolder(JadeCore.Workspace.IFolder f)
        {
            _data.AddFolder(f);
            AddChildFolder(f);
            OnPropertyChanged("Children");
        }

        public void AddNewProject(JadeCore.Project.IProject p)
        {
            _data.AddProject(p);
            AddChildProject(p);
            OnPropertyChanged("Children");
        }

        private void AddChildFolder(JadeCore.Workspace.IFolder f)
        {
            WorkspaceFolder folder = new WorkspaceFolder(this, f);
            Children.Add(folder);
        }

        private void AddChildProject(JadeCore.Project.IProject p)
        {
            Project project = new Project(this, p);
            Children.Add(project);
        }

        #endregion
    }
}