﻿using System;
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

        private ICodeLocation _start;
        private ICodeLocation _end;

        #endregion

        public CodeRange(LibClang.Cursor cur, ISourceFile file)
        {
            LibClang.SourceRange range = cur.Extent;
            
            _start = new CodeLocation(range.Start, file);
            _end = new CodeLocation(range.End, file);
        }

        #region Properties

        public ICodeLocation Start { get { return _start; } }
        public ICodeLocation End { get { return _end; } }

        #endregion

        public override string ToString()
        {
            return string.Format("{0}->{1}", Start, End);
        }
    }
}
