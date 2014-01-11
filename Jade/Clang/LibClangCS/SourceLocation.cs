using System;
using System.Diagnostics;

namespace LibClang
{
    /// <summary>
    /// An immutable wrapper around libclang's SourceLocation type.
    /// A SourceLocation represents a position within a File.
    /// </summary>
    public sealed class SourceLocation : IComparable
    {
        internal delegate SourceLocation CreateSourceLocationDel(Library.SourceLocation handle);
                
        #region Data
                
        private ITranslationUnitItemFactory _itemFactory;

        #endregion

        #region Constructor

        unsafe static internal bool IsValid(Library.SourceLocation handle, IntPtr expectedFile, int expectedOffset)
        {
            IntPtr file = IntPtr.Zero;
            uint line, column, offset;
            Library.clang_getInstantiationLocation(handle, &file, out line, out column, out offset);
            return (file == expectedFile && offset == expectedOffset);
        }

        unsafe internal SourceLocation(Library.SourceLocation handle, ITranslationUnitItemFactory itemFactory)
        {
            Debug.Assert(!handle.IsNull);
            Handle = handle;
            _itemFactory = itemFactory;
            IntPtr file = IntPtr.Zero;
            uint line, column, offset;
            Library.clang_getInstantiationLocation(Handle, &file, out line, out column, out offset);
            Line = (int)line;
            Column = (int)column;
            Offset = (int)offset;
            if (file != IntPtr.Zero)
            {
                File = _itemFactory.CreateFile(file);    
            }            
        }
        
        #endregion

        #region Properties

        internal Library.SourceLocation Handle  { get; private set; }

        public int Line { get; private set; }
        public int Column { get; private set; }
        public int Offset { get; private set; }
        public File File { get; private set; }

        #endregion

        #region object overrides

        public override string ToString()
        {
            return string.Format("{0} {1}:{2} {3}", File.Name, Line, Column, Offset);
        }

        public override bool Equals(object obj)
        {
            return obj is SourceLocation && (SourceLocation)obj == this;
        }

        public override int GetHashCode()
        {
            return (File.ToString() + Offset).GetHashCode();
        }

        #endregion

        #region Static operator functions

        public static bool operator ==(SourceLocation left, SourceLocation right)
        {
            if ((object)left == null && (object)right == null)
                return true;
            if ((object)left == null || (object)right == null)
                return false;
            return left.File == right.File && left.Offset == right.Offset;
        }

        public static bool operator !=(SourceLocation left, SourceLocation right)
        {
            if ((object)left == null && (object)right == null)
                return false;
            if ((object)left == null || (object)right == null)
                return true;
            return left.File != right.File || left.Offset != right.Offset;
        }

        public static bool operator <(SourceLocation first, SourceLocation second)
        {
            Debug.Assert(first != null && second != null);
            if (first.File != second.File)
                throw new ArgumentException("Attempt to compare SourceLocation objects in different files.");
            return first.Offset < second.Offset;
        }

        public static bool operator >(SourceLocation first, SourceLocation second)
        {
            Debug.Assert(first != null && second != null);
            if (first.File != second.File)
                throw new ArgumentException("Attempt to compare SourceLocation objects in different files.");
            return first.Offset > second.Offset;
        }

        public static bool operator <=(SourceLocation first, SourceLocation second)
        {
            Debug.Assert(first != null && second != null);
            if (first.File != second.File)
                throw new ArgumentException("Attempt to compare SourceLocation objects in different files.");
            return first.Offset < second.Offset || first.Offset == second.Offset;
        }

        public static bool operator >=(SourceLocation first, SourceLocation second)
        {
            Debug.Assert(first != null && second != null);
            if (first.File != second.File)
                throw new ArgumentException("Attempt to compare SourceLocation objects in different files.");
            return first.Offset > second.Offset || first.Offset == second.Offset;
        }

        #endregion

        #region IComparable implementation

        public int CompareTo(object obj)
        {
            SourceLocation other = obj as SourceLocation;
            if (other == null)
            {
                throw new ArgumentException("Incorrect type for comparison", "obj");
            }
            if (this.File != other.File)
            {
                throw new ArgumentException("Attempt to compare SourceLocation objects in different files.");
            }
            return Offset.CompareTo(other.Offset);
        }

        #endregion
    }
}
