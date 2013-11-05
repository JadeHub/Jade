using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CppView
{
    public interface ICodeLocation
    {
        int Line { get; }
        int Column { get; }
        int Offset { get; }
    }

    internal class CodeLocation : ICodeLocation
    {
        public CodeLocation(LibClang.SourceLocation loc)
        {
            Line = loc.Line;
            Column = loc.Column;
            Offset = loc.Offset;
        }

        public int Line
        {
            get;
            private set;
        }

        public int Column
        {
            get;
            private set;
        }

        public int Offset
        {
            get;
            private set;
        }
    }
}
