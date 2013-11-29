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
            Debug.Assert(_start.File == _end.File);
        }

        static SourceRange()
        {
            NullRange = new SourceRange(Dll.clang_getNullRange());
        }
        
        #endregion

        public bool ContainsOffset(int offset)
        {
            return offset >= Start.Offset && offset <= End.Offset;
        }

        #region Properties

        public SourceLocation Start { get { return _start; } }
        public SourceLocation End { get { return _end; } }

        public File File { get { return _start.File; } }

        #endregion
    }
}
