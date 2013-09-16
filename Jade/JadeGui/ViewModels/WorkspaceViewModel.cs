using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using JadeControls;
using System.Windows;

namespace JadeGui.ViewModels
{
    using JadeData;

    internal class WorkspaceViewModel : JadeControls.NotifyPropertyChanged, JadeCore.ViewModels.IWorkspaceViewModel
    {
        #region Data

        private JadeData.Workspace.IWorkspace _data;
        private JadeControls.Workspace.ViewModel.WorkspaceTree _tree;
        private bool _modified;

        #endregion

        public WorkspaceViewModel(JadeData.Workspace.IWorkspace data)
        {
            _data = data;
            _modified = false;
            _tree = new JadeControls.Workspace.ViewModel.WorkspaceTree(_data, this);
        }

        public JadeControls.Workspace.ViewModel.WorkspaceTree Tree
        {
            get
            {
                return _tree;
            }
        }

        public string Name
        {
            get
            {
                return _data.Name;
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
            get
            {
                return _data.Path;
            }
            set
            {
                _data.Path = value;
                OnPropertyChanged("Path");
                OnPropertyChanged("Directory");
            }
        }

        public bool Modified
        {
            get { return _modified; }
            set { _modified = value; }
        }
    }
}