using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows;

namespace JadeControls.Workspace.ViewModel
{
    public class WorkspaceFolder : TreeNodeBase
    {
        #region Data

        private Workspace _workspace;
        private JadeData.Workspace.IFolder _data;
   
        #endregion

        #region Constructor

        public WorkspaceFolder(TreeNodeBase parent, JadeData.Workspace.IFolder folder)
            :   base(folder.Name, parent)
        {
            _data = folder;
        }

        protected void Initialise(Workspace workspace)
        {
            _workspace = workspace;

            foreach (JadeData.Workspace.IFolder f in _data.Folders)
            {
                WorkspaceFolder folder = new WorkspaceFolder(this, f);
                folder.Initialise(_workspace);
                AddChildFolder(f);
            }

            foreach (JadeData.Workspace.IItem item in _data.Items)
            {
                if (item is JadeData.Project.IProject)
                {
                    AddChildProject(item as JadeData.Project.IProject);
                    //AddChildProject(new Project(_workspace, this, item as JadeData.Project.IProject));
                }
            }
        }

        #endregion

        #region Remove Command

        private ObjectCommand<WorkspaceFolder> _removeCommand;

        public ObjectCommand<WorkspaceFolder> RemoveCommand
        {
            get
            {
                if (_removeCommand == null)
                {
                    _removeCommand = new ObjectCommand<WorkspaceFolder>(this);
                }
                return _removeCommand;
            }
        }

        #endregion

        #region Add Project Command

        private RelayCommand _addProjectCommand;

        public RelayCommand AddProjectCommand
        {
            get
            {
                if (_addProjectCommand == null)
                {
                    _addProjectCommand = new RelayCommand(param => OnAddProject(), param => CanDoAddProject);
                }
                return _addProjectCommand;
            }
        }

        private void OnAddProject()
        {
            string name;
            if (GuiUtils.PromptUserInput("Enter new Project name", out name) == false)
            {
                return;
            }

            if (ContainsChild(name))
            {
                GuiUtils.DisplayErrorAlert("Project name '" + name + "'is not unique.");
                return;
            }
            JadeData.Project.IProject project = new JadeData.Project.Project(name, _workspace.Directory + "\\" + name);
            _data.AddProject(project);
            AddChildProject(project);
            _workspace.Modified = true;
            Expanded = true;
            OnPropertyChanged("Children");
        }

        private bool CanDoAddProject { get { return true; } }

        #endregion

        #region Add Folder Command

        private RelayCommand _addFolderCommand;

        public RelayCommand AddFolderCommand
        {
            get
            {
                if (_addFolderCommand == null)
                {
                    _addFolderCommand = new RelayCommand(param => OnAddFolder(), param => CanDoAddFolder);
                }
                return _addFolderCommand;
            }
        }

        private void OnAddFolder()
        {
            string name;
            if (GuiUtils.PromptUserInput("Enter new Folder name", out name) == false || name.Length == 0)
            {
                return;
            }
            
            if (ContainsChild(name))
            {
                GuiUtils.DisplayErrorAlert("Folder name is not unique.");
                return;
            }

            JadeData.Workspace.IFolder data = new JadeData.Workspace.Folder(name);            
            _data.AddFolder(data);
            AddChildFolder(data);
            _workspace.Modified = true;
            Expanded = true;
            OnPropertyChanged("Children");
        }

        private bool CanDoAddFolder { get { return true; } }

        #endregion

        #region Child Commands

        #region Remove Folder

        private void OnRemoveFolder(WorkspaceFolder folder)
        {
            if (JadeControls.GuiUtils.ConfirmYNAction("Do you want remove Folder " + folder.DisplayName + "?") == false)
                return;

            if (Children.Contains(folder) && _data.RemoveFolder(folder.DisplayName))
            {
                Children.Remove(folder);
                _workspace.Modified = true;
                OnPropertyChanged("Children");
                return;
            }
        }

        private bool CanDoRemoveFolder(WorkspaceFolder folder)
        {
            return true;
        }

        #endregion

        #region Remove Project

        private void OnRemoveProject(Project project)
        {
            if (JadeControls.GuiUtils.ConfirmYNAction("Do you want remove Project " + project.DisplayName + "?") == false)
                return;

            if (Children.Contains(project) && _data.RemoveProject(project.DisplayName))
            {
                Children.Remove(project);
                _workspace.Modified = true;
                OnPropertyChanged("Children");
                return;
            }
        }

        private bool CanDoRemoveProject(Project project)
        {
            return true;
        }

        #endregion

        #endregion

        #region Private Methods

        private void AddChildFolder(JadeData.Workspace.IFolder f)
        {
            WorkspaceFolder folder = new WorkspaceFolder(this, f);
            folder.Initialise(_workspace);
            folder.RemoveCommand.Attach(param => OnRemoveFolder(param), param => CanDoRemoveFolder(param));
            Children.Add(folder);
        }

        private void AddChildProject(JadeData.Project.IProject p)
        {
            Project project = new Project(_workspace, this, p);
            project.RemoveCommand.Attach(param => OnRemoveProject(param), param => CanDoRemoveProject(param));
            Children.Add(project);
        }

        #endregion
    }

    public class Workspace : WorkspaceFolder, JadeCore.ViewModels.IWorkspaceViewModel
    {
        #region Data

        private JadeData.Workspace.IWorkspace _data;
        private bool _modified;

        #endregion

        public Workspace(JadeData.Workspace.IWorkspace workspace)
            : base(null, workspace)
        {
            this.Initialise(this);

            _data = workspace;
            _modified = false;
            this.Expanded = true;
        }

        #region Public Properties
        
        public IEnumerable<JadeControls.Workspace.ViewModel.Workspace> WorkspaceTree
        {
            get
            {
                yield return this;
            }
        }
        
        public string Directory
        {
            get
            {
                return _data.Directory;
            }
        }

        public string Path
        {
            get { return _data.Path; }
        }

        public bool Modified
        {
            get { return _modified; }
            set { _modified = value; }
        }

        public string LongName
        {
            get { return _data.Name + " Workspace"; }
        }

        #endregion

        #region Public methods

        public bool Save(string path)
        {
            try
            {
                JadeData.Persistence.Workspace.Writer.Write(_data, path);
                //set new path
                _modified = false;
                return true;
            }
            catch (Exception e)
            {
                GuiUtils.DisplayErrorAlert("Error saving workspace. " + e.ToString());
            }
            return false;
        }

        public void SaveAs(string path)
        {
            Save(path);
        }

        public bool SaveOnExit()
        {
            if(Modified)
            {
                MessageBoxResult result = GuiUtils.PromptYesNoCancelQuestion("Do you wish to save Workspace " + DisplayName + " before exiting?", "Confirm Save");
                if (result == System.Windows.MessageBoxResult.Cancel)
                {
                    return false;
                }
                else if (result == MessageBoxResult.Yes)
                {
                    Save(Path);                    
                }                
            }
            return true;
        }
    
        #endregion
    }
}

