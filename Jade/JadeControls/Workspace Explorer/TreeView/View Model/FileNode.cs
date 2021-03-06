﻿using System;
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

        private readonly JadeCore.Project.FileItem _data;

        #endregion

        #region Constructor

        public File(TreeNodeBase parent, JadeCore.Project.FileItem file)
            : base(file.Path.FileName, parent)
        {
            _data = file;
        }

        #endregion
        
        public JadeUtils.IO.IFileHandle Handle { get { return _data.Handle; } }

        public string Path { get { return _data.Path.Str; } }
    }
}