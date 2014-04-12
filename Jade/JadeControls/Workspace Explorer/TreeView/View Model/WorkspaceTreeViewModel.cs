using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows;
using JadeCore;
using JadeUtils.IO;

namespace JadeControls.Workspace.ViewModel
{
    public class WorkspaceTree : WorkspaceFolder
    {
        #region Data

        private JadeCore.Workspace.IWorkspace _data;

        #endregion

        #region Constructor

        public WorkspaceTree(JadeCore.Workspace.IWorkspace workspace)
            : base(null, workspace)
        {
            _data = workspace;
            this.Expanded = true;
        }

        #endregion

        #region Public Properties

        public IEnumerable<object> TreeRoot
        {
            get
            {
                yield return this;
            }
        }

        #endregion

        #region Private Properties

        JadeCore.Workspace.IWorkspaceController WorkspaceController
        {
            get
            {
                System.Diagnostics.Debug.Assert(JadeCore.Services.Provider.WorkspaceController.WorkspaceOpen);
                return JadeCore.Services.Provider.WorkspaceController;
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
                JadeCore.Workspace.IFolder newFolderData = new JadeCore.Workspace.Folder(name);                
                parentVm.AddNewChildFolder(newFolderData);
                
            }
            else if (sel is ProjectFolder)
            {
                ProjectFolder paremtVm = sel as ProjectFolder;
                JadeCore.Project.IFolder parent = paremtVm.Data;
                JadeCore.Project.IFolder newFolderData = new JadeCore.Project.Folder(parent.OwningProject, name);
                paremtVm.AddNewChildFolder(newFolderData);
            }
            else
            {
                return;
            }
            sel.Expanded = true;            
            WorkspaceController.CurrentWorkspaceModified = true;
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

            JadeCore.Project.IProject project = new JadeCore.Project.Project(name, FilePath.Make(".\\" + name + ".jpj"));
                                                    //Services.Provider.FileService.MakeFileHandle(".\\" + name + ".jpj"));
            vm.AddNewProject(project);
            vm.Expanded = true;
            WorkspaceController.CurrentWorkspaceModified = true;
        }

        public bool CanAddProject()
        {
            return GetSelectedAs(typeof(WorkspaceFolder)) != null;
        }

        #endregion

        #region Add File

        public void OnAddFile(ProjectFolder vm)
        {
            if (vm == null)
                throw new ArgumentException("Command param is null.");

            IFileHandle handle = JadeCore.GuiUtils.PromptOpenFile(".cs", "C# Source files (.cs)|*.cs", true);
            if (handle == null)
            {
                return;
            }           
            vm.AddNewFile(handle);
            vm.Expanded = true;
            WorkspaceController.CurrentWorkspaceModified = true;
        }

        public bool CanAddFile()
        {
            return GetSelectedAs(typeof(ProjectFolder)) != null;
        }

        #endregion

        #region Remove Item

        public void OnRemoveItem(object param)
        {
            TreeNodeBase sel = param as TreeNodeBase;
            if (sel == null)
                throw new ArgumentException("Command param is null.");

            if (JadeCore.GuiUtils.ConfirmYNAction("Do you want remove " + sel.DisplayName + "?") == false)
                return;

            if (sel.Parent.RemoveChild(sel))
            {
                WorkspaceController.CurrentWorkspaceModified = true;
            }
        }

        public bool CanRemoveItem(object param)
        {
            return FindSelected() != null;
        }

        #endregion

        #endregion

        public void OnDoubleClick()
        {
            TreeNodeBase node = FindSelected();
            if (node != null && node is File)
            {
                File f = node as File;
                ApplicationCommands.Open.Execute(f.Handle, null);
            }
        }
    }
}

