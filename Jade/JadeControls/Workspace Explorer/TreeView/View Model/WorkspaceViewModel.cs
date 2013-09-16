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
            :   base(data.Name, parent)
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

        protected override void RemoveChildData(TreeNodeBase child)
        {
            if (child is WorkspaceFolder)
            {
                _data.RemoveFolder(child.DisplayName);
            }
            else if (child is ProjectFolder)
            {
                _data.RemoveProject(child.DisplayName);
            }
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
    
    public class WorkspaceTree : WorkspaceFolder
    {
        #region Data

        private JadeData.Workspace.IWorkspace _data;
        private bool _modified;

        #endregion

        public WorkspaceTree(JadeData.Workspace.IWorkspace workspace)
            : base(null, workspace)
        {
            _data = workspace;
            _modified = false;
            this.Expanded = true;
        }
        
        #region Public Properties
        
        public IEnumerable<JadeControls.Workspace.ViewModel.WorkspaceTree> TreeRoot
        {
            get
            {
                yield return this;
            }
        }

        public bool Modified
        {
            get { return _modified; }
            set { _modified = value; }
        }

        #endregion
        
        #region Private Methods

        private TreeNodeBase GetSelectedAs(System.Type type)
        {
            TreeNodeBase ret = null;
            ret = base.FindSelected();
            if (ret.GetType() == type)
                return ret;
            return ret;            
        }

        #endregion

        #region Command Handlers

        #region Open Document

        public void OnOpenDocument()
        {
            File f = GetSelectedAs(typeof(File)) as File;
            if (f != null)
            {
                JadeCore.Services.Provider.JadeViewModel.Editor.OpenSourceFile(f.Data);
            }
        }

        public bool CanOpenDocument()
        {
            return GetSelectedAs(typeof(File)) != null;
        }

        #endregion

        #region Add Folder

        public void OnAddFolder()
        {
            TreeNodeBase sel = FindSelected();
            if (sel == null)
                return;

            string name;
            if (JadeCore.GuiUtils.PromptUserInput("Enter new Folder name", out name) == false || name.Length == 0)
            {
                return;
            }

            if (sel.ContainsChild(name))
            {
                JadeCore.GuiUtils.DisplayErrorAlert("Folder name is not unique.");
                return;
            }

            if (sel is WorkspaceFolder)
            {
                WorkspaceFolder parentVm = sel as WorkspaceFolder;
                JadeData.Workspace.IFolder newFolderData = new JadeData.Workspace.Folder(name);                
                parentVm.AddNewChildFolder(newFolderData);
                
            }
            else if (sel is ProjectFolder)
            {
                   ProjectFolder paremtVm = sel as ProjectFolder;
                   JadeData.Project.IFolder newFolderData = new JadeData.Project.Folder(name);
                   paremtVm.AddNewChildFolder(newFolderData);
            }
            else
            {
                return;
            }
            sel.Expanded = true;
            _modified = true;
        }        

        public bool CanAddFolder()
        {
            return GetSelectedAs(typeof(ProjectFolder)) != null && GetSelectedAs(typeof(WorkspaceFolder)) != null;
        }

        #endregion

        #region Add Project

        public void OnAddProject()
        {
            WorkspaceFolder vm = GetSelectedAs(typeof(WorkspaceFolder)) as WorkspaceFolder;
            if (vm == null)
                return;

            string name;
            if (JadeCore.GuiUtils.PromptUserInput("Enter new Project name", out name) == false)
            {
                return;
            }

            if (vm.ContainsChild(name))
            {
                JadeCore.GuiUtils.DisplayErrorAlert("Project name '" + name + "'is not unique.");
                return;
            }
            JadeData.Project.IProject project = new JadeData.Project.Project(name, ".\\" + name + ".jpj");
            vm.AddNewProject(project);
            vm.Expanded = true;
            _modified = true;            
        }

        public bool CanAddProject()
        {
            return GetSelectedAs(typeof(WorkspaceFolder)) != null;
        }

        #endregion

        #region Add File

        public void OnAddFile()
        {
            ProjectFolder vm = GetSelectedAs(typeof(ProjectFolder)) as ProjectFolder;
            if (vm == null)
                return;
        }

        public bool CanAddFile()
        {
            return GetSelectedAs(typeof(ProjectFolder)) != null;
        }

        #endregion

        #region Remove Folder

        public void OnRemoveFolder()
        {
            TreeNodeBase sel = FindSelected();
            if (sel == null)
                return;

            if (JadeCore.GuiUtils.ConfirmYNAction("Do you want remove Folder " + sel.DisplayName + "?") == false)
                return;

            if (sel.Parent.RemoveChild(sel))
            {
                _modified = true;
            }
        }

        public bool CanRemoveFolder()
        {
            return GetSelectedAs(typeof(ProjectFolder)) != null && GetSelectedAs(typeof(WorkspaceFolder)) != null;
        }

        #endregion

        #region Remove Project

        public void OnRemoveProject()
        {
            TreeNodeBase sel = FindSelected();
            if (sel == null)
                return;

            if (JadeCore.GuiUtils.ConfirmYNAction("Do you want remove Project " + sel.DisplayName + "?") == false)
                return;

            if (sel.Parent.RemoveChild(sel))
            {
                _modified = true;
            }
        }

        public bool CanRemoveProject()
        {
            return GetSelectedAs(typeof(Project)) != null;
        }

        #endregion

        #region Remove File

        public void OnRemoveFile()
        {
            TreeNodeBase sel = FindSelected();
            if (sel == null)
                return;

            if (JadeCore.GuiUtils.ConfirmYNAction("Do you want remove File " + sel.DisplayName + "?") == false)
                return;

            if (sel.Parent.RemoveChild(sel))
            {
                _modified = true;
            }
        }

        public bool CanRemoveFile()
        {
            return GetSelectedAs(typeof(Project)) != null;
        }

        #endregion

        #endregion
    }
}

