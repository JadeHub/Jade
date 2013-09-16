﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace JadeControls.Workspace.ViewModel
{
    internal class Project : ProjectFolder
    {
        #region Data

        private readonly JadeData.Project.IProject _data;

        #endregion

        #region Constructor

        public Project(TreeNodeBase parent, JadeData.Project.IProject project)
            :base(parent, project)
        {
            _data = project;
        }

        #endregion
    }
}
