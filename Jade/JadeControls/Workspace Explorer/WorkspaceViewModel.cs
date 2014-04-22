using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using JadeControls;
using System.Windows;

namespace JadeControls.Workspace.ViewModel
{
    using JadeCore;

    public class WorkspaceViewModel : JadeControls.Docking.ToolPaneViewModel
    {
        #region Data

        private JadeCore.Workspace.IWorkspace _data;
        private WorkspaceTree _tree;

        #endregion

        public WorkspaceViewModel()
        {
            Title = "Workspace";
            ContentId = "WorkspaceToolPane";            
        }

        #region Public Properties

        public JadeCore.Workspace.IWorkspace Data
        {
            set 
            {
                _data = value;
                if (_data != null)
                    _tree = new JadeControls.Workspace.ViewModel.WorkspaceTree(_data);
                else
                    _tree = null;
                IsVisible = _data != null;
                OnPropertyChanged("Tree");
            }
        }

        public WorkspaceTree Tree
        {
            get
            {
                return _tree;
            }
        }

        #endregion
    }
}