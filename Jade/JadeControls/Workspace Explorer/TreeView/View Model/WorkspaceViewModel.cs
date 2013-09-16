using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows;
using JadeCore.IO;

namespace JadeControls.Workspace.ViewModel
{
    public class WorkspaceTree : WorkspaceFolder
    {
        #region Data

        private JadeCore.ViewModels.IWorkspaceViewModel _workspace;
        private JadeData.Workspace.IWorkspace _data;

        #endregion

        #region Constructor

        public WorkspaceTree(JadeData.Workspace.IWorkspace workspace, JadeCore.ViewModels.IWorkspaceViewModel vm)
            : base(null, workspace)
        {
            _workspace = vm;
            _data = workspace;
            this.Expanded = true;
        }

        #endregion

        #region Public Properties

        public IEnumerable<JadeControls.Workspace.ViewModel.WorkspaceTree> TreeRoot
        {
            get
            {
                yield return this;
            }
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
                //JadeCore.Services.Provider.JadeViewModel.Editor.OpenSourceFile(f.Data);
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
            _workspace.Modified = true;
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

            JadeData.Project.IProject project = new JadeData.Project.Project(name, FileHandleFactory.Create(".\\" + name + ".jpj"));
            vm.AddNewProject(project);
            vm.Expanded = true;
            _workspace.Modified = true;
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

            IFileHandle handle = JadeCore.GuiUtils.PromptOpenFile(".cs", "C# Source files (.cs)|*.cs", true);
            if (handle == null)
            {
                return;
            }           
            vm.AddNewFile(handle);
            vm.Expanded = true;
            _workspace.Modified = true;
        }

        public bool CanAddFile()
        {
            return GetSelectedAs(typeof(ProjectFolder)) != null;
        }

        #endregion

        #region Remove Item

        public void OnRemoveItem()
        {
            TreeNodeBase sel = FindSelected();
            if (sel == null)
                return;

            if (JadeCore.GuiUtils.ConfirmYNAction("Do you want remove " + sel.DisplayName + "?") == false)
                return;

            if (sel.Parent.RemoveChild(sel))
            {
                _workspace.Modified = true;
            }
        }

        public bool CanRemoveItem()
        {
            return FindSelected() != null;
        }

        #endregion

        #endregion
    }
}

