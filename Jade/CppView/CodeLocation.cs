using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CppView
{
    using JadeUtils.IO;

    public interface ICodeLocation
    {
        int Line { get; }
        int Column { get; }
        int Offset { get; }
        FilePath Path { get; }
    }

    internal class CodeLocation : ICodeLocation
    {
        public CodeLocation(LibClang.SourceLocation loc, FilePath path)
        {
            Handle = loc;
            Line = loc.Line;
            Column = loc.Column;
            Offset = loc.Offset;        
            Path = path;
        }

        public LibClang.SourceLocation Handle
        {
            get;
            private set;
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

        public FilePath Path
        {
            get;
            private set;
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}:{2}", Line, Column, Offset);
        }

        public override bool Equals(object obj)
        {
            if (obj is CodeLocation)
            {
                CodeLocation rhs = obj as CodeLocation;
                return rhs.Path == Path && rhs.Offset == Offset;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Path.GetHashCode() + Offset;
        }
    }
}
