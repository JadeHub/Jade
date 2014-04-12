using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;


namespace JadeControls.Workspace.ViewModel
{
    public class ProjectFolder : TreeNodeBase
    {
        #region Data

        /// <summary>
        /// Underlying model data
        /// </summary>
        private JadeCore.Project.IFolder _data;

        #endregion

        #region Constructor

        public ProjectFolder(TreeNodeBase parent, JadeCore.Project.IFolder data)
            : base(data.Name, parent)
        {
            _data = data;

            foreach (JadeCore.Project.IFolder f in _data.Folders)
            {
                AddChildFolder(new ProjectFolder(this, f));
            }

            foreach (JadeCore.Project.IItem i in _data.Items)
            {
                if (i is JadeCore.Project.FileItem)
                {
                    AddChildFile(new File(this, i as JadeCore.Project.FileItem));
                }
            }
        }

        #endregion

        #region Public Methods

        public void AddNewChildFolder(JadeCore.Project.IFolder f)
        {
            _data.AddFolder(f);
            AddChildFolder(new ProjectFolder(this, f));
        }

        public void AddNewFile(JadeUtils.IO.IFileHandle fileHandle)
        {
            if (_data.HasItem(fileHandle.Path.Str))
            {
                throw new Exception("Attempt to add duplicate file name to project.");
            }

            JadeCore.Project.FileItem data = new JadeCore.Project.FileItem(fileHandle);
            _data.AddItem(data);
            AddChildFile(new File(this, data));
            OnPropertyChanged("Children");
        }

        #endregion

        #region Properties

        public JadeCore.Project.IFolder Data { get { return _data; } }

        #endregion

        #region Private Methods

        protected override bool RemoveChildData(TreeNodeBase child)
        {
            if (child is ProjectFolder)
            {
                return _data.RemoveFolder(child.DisplayName);
            }
            else if (child is File)
            {
                return _data.RemoveItem((child as File).Path);
            }
            return false;
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
}