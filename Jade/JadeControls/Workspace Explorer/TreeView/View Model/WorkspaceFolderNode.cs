using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows;

namespace JadeControls.Workspace.ViewModel
{
    public class WorkspaceFolder : TreeNodeBase
    {
        #region Data

        private JadeData.Workspace.IFolder _data;

        #endregion

        #region Constructor

        public WorkspaceFolder(TreeNodeBase parent, JadeData.Workspace.IFolder data)
            : base(data.Name, parent)
        {
            _data = data;
            foreach (JadeData.Workspace.IFolder f in _data.Folders)
            {
                WorkspaceFolder folder = new WorkspaceFolder(this, f);
                AddChildFolder(f);
            }

            foreach (JadeData.Workspace.IItem item in _data.Items)
            {
                if (item is JadeData.Project.IProject)
                {
                    AddChildProject(item as JadeData.Project.IProject);
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

        public void AddNewChildFolder(JadeData.Workspace.IFolder f)
        {
            _data.AddFolder(f);
            AddChildFolder(f);
            OnPropertyChanged("Children");
        }

        public void AddNewProject(JadeData.Project.IProject p)
        {
            _data.AddProject(p);
            AddChildProject(p);
            OnPropertyChanged("Children");
        }

        private void AddChildFolder(JadeData.Workspace.IFolder f)
        {
            WorkspaceFolder folder = new WorkspaceFolder(this, f);
            Children.Add(folder);
        }

        private void AddChildProject(JadeData.Project.IProject p)
        {
            Project project = new Project(this, p);
            Children.Add(project);
        }

        #endregion
    }
}