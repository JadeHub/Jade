using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CppView
{
    using JadeUtils.IO;

    public interface ISourceFile
    {
        JadeUtils.IO.FilePath Path { get; }
    }

    public class SourceFile : ISourceFile
    {
        private FilePath _path;

        public SourceFile(string path)
        {
            _path = JadeUtils.IO.FilePath.Make(path);            
        }

        public SourceFile(FilePath path)
        {
            _path = path;
        }

        #region Properties

        public JadeUtils.IO.FilePath Path
        {
            get { return _path; }
        }

        #endregion

        public override string ToString()
        {
            return _path.ToString();
        }

        public override bool Equals(object obj)
        {
            return _path.Equals(obj);
        }

        public override int GetHashCode()
        {
            return _path.GetHashCode();
        }
    }
}
