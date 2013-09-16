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

        public JadeData.Project.File Data { get { return _data;}}        
    }

    internal class ProjectFolder : TreeNodeBase
    {
        #region Data

          /// <summary>
        /// Underlying model data
        /// </summary>
        private JadeData.Project.IFolder _data;
     
        #endregion

        #region Constructor

        public ProjectFolder(TreeNodeBase parent, JadeData.Project.IFolder data)
            :   base(data.Name, parent)
        {
            _data = data;

            foreach (JadeData.Project.IFolder f in _data.Folders)
            {
                AddChildFolder(new ProjectFolder(this, f));              
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

        #region Public Methods

        public void AddNewChildFolder(JadeData.Project.IFolder f)
        {
            _data.AddFolder(f);
            AddChildFolder(new ProjectFolder(this, f));
        }

        public void AddNewFile(string path)
        {
            string name = System.IO.Path.GetFileName(path);
            if (_data.HasItem(name))
            {
                return;
            }

            JadeData.Project.File data = new JadeData.Project.File(name, path);
            _data.AddItem(data);
            AddChildFile(new File(this, data));
            OnPropertyChanged("Children");
        }

        #endregion
        
        #region Private Methods

        protected override void RemoveChildData(TreeNodeBase child)
        {
            if (child is ProjectFolder)
            {
                _data.RemoveFolder(child.DisplayName);
            }
            else if (child is File)
            {
                _data.RemoveItem(child.DisplayName);
            }
        }

        private void AddChildFile(File f)
        {
            Children.Add(f);
        }

        private void AddChildFolder(ProjectFolder f)
        {
            Children.Add(f);
        }

        #endregion
    }

    internal class Project : ProjectFolder
    {
        #region Data

        private readonly JadeData.Project.IProject _data;

        #endregion

        public Project(TreeNodeBase parent, JadeData.Project.IProject project)
            :base(parent, project)
        {
            _data = project;
        }
    }
}
