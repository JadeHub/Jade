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

        #region Constructor

        public File(TreeNodeBase parent, JadeData.Project.File file)
            : base(file.Name, parent)
        {
            _data = file;
        }

        #endregion
        
        public JadeCore.IO.IFileHandle Handle { get { return _data.Handle; } }
    }
}