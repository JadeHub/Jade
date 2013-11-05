using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CppView
{
    public interface ICodeRange
    {
        ICodeLocation Start { get; }
        ICodeLocation End { get; }
    }

    public class CodeRange : ICodeRange
    {
        #region Data

        private LibClang.SourceRange _range;

        #endregion

        public CodeRange(LibClang.SourceRange range)
        {
            _range = range;
        }

        #region Properties

        public ICodeLocation Start { get { return null; } }
        public ICodeLocation End { get { return null; } }

        #endregion
    }
}
