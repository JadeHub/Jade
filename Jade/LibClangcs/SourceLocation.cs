using System;

namespace LibClang
{
    public class SourceLocation : IComparable
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

        public override string ToString()
        {
            return string.Format("{0} {1}:{2}", File.Name, Line, Column);
        }

        public static bool operator ==(SourceLocation left, SourceLocation right)
        {
            return left.File == right.File && left.Offset == right.Offset;
        }

        public static bool operator !=(SourceLocation left, SourceLocation right)
        {
            return left.File != right.File || left.Offset != right.Offset;
        }

        public static bool operator <(SourceLocation first, SourceLocation second)
        {
            return first.Offset < second.Offset;
        }

        public static bool operator >(SourceLocation first, SourceLocation second)
        {
            return first.Offset > second.Offset;
        }

        public static bool operator <=(SourceLocation first, SourceLocation second)
        {
            return first.Offset < second.Offset || first.Offset == second.Offset;
        }

        public static bool operator >=(SourceLocation first, SourceLocation second)
        {
            return first.Offset > second.Offset || first.Offset == second.Offset;
        }

        public int CompareTo(object obj)
        {
            SourceLocation other = obj as SourceLocation;
            if (other == null)
            {
                throw new ArgumentException("Incorrect type for comparison", "obj");
            }
            return Offset.CompareTo(other.Offset);
        }

        public override bool Equals(object obj)
        {
            return obj is SourceLocation && (SourceLocation)obj == this;
        }

        public override int GetHashCode()
        {
            return (File.ToString() + Offset).GetHashCode();
        }
    }
}
