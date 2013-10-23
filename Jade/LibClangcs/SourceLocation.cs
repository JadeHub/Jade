using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibClang
{
    public class SourceLocation //: IComparable
    {
        internal Dll.SourceLocation Handle
        {
            get;
            private set;
        }

        public File File 
        { 
            get; 
            private set; 
        }

        public readonly int Line;
        public readonly int Column;
        public readonly int Offset;

        unsafe internal SourceLocation(Dll.SourceLocation handle)
        {
            Handle = handle;
            IntPtr file = IntPtr.Zero;
            uint line, column, offset;
            Dll.clang_getInstantiationLocation(Handle, &file, out line, out column, out offset);
            Line = (int)line;
            Column = (int)column;
            Offset = (int)offset;
            File = new File(file);
        }
    }
}
