using System;
using System.Diagnostics;

namespace LibClang
{
    public class SourceRange
    {
        #region NullSourceRange

        static private SourceRange NullRange;

        #endregion

        #region Data

        private Dll.SourceRange _handle;
        private SourceLocation _start;
        private SourceLocation _end;

        #endregion

        #region Constructor

        internal SourceRange(Dll.SourceRange handle)
        {
            _handle = handle;
            _start = new SourceLocation(Dll.clang_getRangeStart(_handle));
            _end = new SourceLocation(Dll.clang_getRangeEnd(_handle));
            Debug.Assert(_start <= _end);
        }

        static SourceRange()
        {
            NullRange = new SourceRange(Dll.clang_getNullRange());
        }
        
        #endregion

        #region Properties

        public SourceLocation Start { get { return _start; } }
        public SourceLocation End { get { return _end; } }

        #endregion
    }
}
