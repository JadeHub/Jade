using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace JadeControls.Workspace.ViewModel
{
    internal class File : TreeNodeBase
    {
        #region Data

        private readonly JadeData.Project.File _data;

        #endregion

        public File(TreeNodeBase parent, JadeData.Project.File file)
            :   base(file.Name, parent)
        {
            _data = file;
        }

        #region Open Command

        private RelayCommand _openCommand;

        public ICommand OpenCommand
        {
            get
            {
                if (_openCommand == null)
                {
                    _openCommand = new RelayCommand(param => this.OnOpenCommand(), param => this.CanDoOpenCommand);
                }
                return _openCommand;
            }
        }

        public void OnOpenCommand()
        {
            JadeCore.Services.Provider.JadeViewModel.Editor.OpenSourceFile(_data);
        }

        private bool CanDoOpenCommand
        {
            get { return true; }
        }

        #endregion

        #region Remove Command

        private ObjectCommand<File> _removeCommand;

        public ObjectCommand<File> RemoveCommand
        {
            get
            {
                if (_removeCommand == null)
                {
                    _removeCommand = new ObjectCommand<File>(this);
                }
                return _removeCommand;
            }
        }
        
        #endregion
    }

    internal class ProjectFolder : TreeNodeBase
    {
        #region Data

        /// <summary>
        /// Owning Workspace
        /// </summary>
        private Workspace _workspace;
          /// <summary>
        /// Underlying model data
        /// </summary>
        private JadeData.Project.IFolder _data;
     
        #endregion

        #region Constructor

        public ProjectFolder(Workspace workspace, TreeNodeBase parent, JadeData.Project.IFolder data)
            :   base(data.Name, parent)
        {
            _workspace = workspace;
            _data = data;

            foreach (JadeData.Project.IFolder f in _data.Folders)
            {
                AddChildFolder(new ProjectFolder(workspace, this, f));
                
            }

            foreach (JadeData.Project.IItem i in _data.Items)
            {
                if (i is JadeData.Project.File)
                {
                    AddChildFile(new File(this, i as JadeData.Project.File));
                    
                }
            }
        }

        #endregion

        #region Child Commands

        #region Remove File

        private void OnRemoveFile(File f)
        {
            if (JadeCore.GuiUtils.ConfirmYNAction("Do you want remove File " + f.DisplayName + "?") == false)
                return;

            if (Children.Contains(f) && _data.RemoveItem(f.DisplayName))
            {
                Children.Remove(f);
                _workspace.Modified = true;
                OnPropertyChanged("Children");
                return;
            }
        }

        private bool CanDoRemoveFile(File f)
        { 
            return true; 
        }

        #endregion

        #region Remove Folder

        private void OnRemoveFolder(ProjectFolder f)
        {
            if (JadeCore.GuiUtils.ConfirmYNAction("Do you want remove Project Folder " + f.DisplayName + "?") == false)
                return;

            if (Children.Contains(f) && _data.RemoveFolder(f.DisplayName))
            {
                Children.Remove(f);
                _workspace.Modified = true;
                OnPropertyChanged("Children");
                return;
            }
        }

        private bool CanDoRemoveFolder(ProjectFolder f)
        {
            return true;
        }

        #endregion

        #endregion

        #region Remove Command

        private ObjectCommand<ProjectFolder> _removeCommand;

        public ObjectCommand<ProjectFolder> RemoveCommand
        {
            get
            {
                if (_removeCommand == null)
                {
                    _removeCommand = new ObjectCommand<ProjectFolder>(this);
                }
                return _removeCommand;
            }
        }

        #endregion

        #region Add Folder Command

        private RelayCommand _addFolderCommand;

        public ICommand AddFolderCommand
        {
            get
            {
                if (_addFolderCommand == null)
                {
                    _addFolderCommand = new RelayCommand(param => this.OnAddFolderCommand(), param => this.CanDoAddFolderCommand);
                }
                return _addFolderCommand;
            }
        }

        public void OnAddFolderCommand()
        {
            //_parent.RemoveFolder(this);
        }

        private bool CanDoAddFolderCommand
        {
            get { return true; }
        }

        #endregion

        #region Add File Command

        private RelayCommand _addFileCommand;

        public ICommand AddFileCommand
        {
            get
            {
                if (_addFileCommand == null)
                {
                    _addFileCommand = new RelayCommand(param => this.OnAddFileCommand(), param => this.CanDoAddFileCommand);
                }
                return _addFileCommand;
            }
        }

        public void OnAddFileCommand()
        {
            AddProjectFileViewModel vm = new AddProjectFileViewModel();
            vm.Location = _workspace.Directory;
            if (JadeCore.GuiUtils.DisplayModalWindow(new AddProjectFileWindow(), vm))
            {
                if (vm.IsValid)
                {
                    string loc = vm.Location;
                    foreach (string path in vm.Paths)
                    {
                        AddNewFile(path);
                    }
                    Expanded = true;
                }
            }
        }

        private bool CanDoAddFileCommand
        {
            get { return true; }
        }

        #endregion

        #region Private Methods

        private void AddChildFile(File f)
        {
            f.RemoveCommand.Attach(param => this.OnRemoveFile(param), param => this.CanDoRemoveFile(param));
            Children.Add(f);
        }

        private void AddChildFolder(ProjectFolder f)
        {
            f.RemoveCommand.Attach(param => this.OnRemoveFolder(param), param => this.CanDoRemoveFolder(param));
            Children.Add(f);
        }

        private void AddNewFile(string path)
        {
            string name = System.IO.Path.GetFileName(path);
            if (_data.HasItem(name))
            {
                return;
            }

            JadeData.Project.File data = new JadeData.Project.File(name, path);
            _data.AddItem(data);
            AddChildFile(new File(this, data));
            _workspace.Modified = true;
            Expanded = true;
            OnPropertyChanged("Children");
        }
               
        #endregion
    }

    internal class Project : ProjectFolder
    {
        #region Data

        private readonly JadeData.Project.IProject _data;

        #endregion

        public Project(Workspace workspace, TreeNodeBase parent, JadeData.Project.IProject project)
            :base(workspace, parent, project)
        {
            _data = project;
        }

        #region Remove Command

        private ObjectCommand<Project> _removeCommand;

        public new ObjectCommand<Project> RemoveCommand
        {
            get
            {
                if (_removeCommand == null)
                {
                    _removeCommand = new ObjectCommand<Project>(this);
                }
                return _removeCommand;
            }
        }

        #endregion
    }
}
