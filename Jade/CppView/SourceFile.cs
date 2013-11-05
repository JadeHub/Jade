using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CppView
{
    public interface ISourceFile
    {
        
    }

    public class SourceFile : ISourceFile
    {
        private JadeUtils.IO.IFileHandle _file;

        public SourceFile(JadeUtils.IO.IFileHandle file)
        {
            _file = file;
        }

        #region Properties

        public JadeUtils.IO.IFileHandle File
        {
            get { return _file; }
        }

        #endregion
    }
}
