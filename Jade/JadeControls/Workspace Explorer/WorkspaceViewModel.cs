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

    public class WorkspaceViewModel : JadeControls.NotifyPropertyChanged
    {
        #region Data

        private JadeCore.Workspace.IWorkspace _data;
        private WorkspaceTree _tree;

        #endregion

        public WorkspaceViewModel(JadeCore.Workspace.IWorkspace data)
        {
            _data = data;
            _tree = new JadeControls.Workspace.ViewModel.WorkspaceTree(_data);
        }

        #region Public Properties

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