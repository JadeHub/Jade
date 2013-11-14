using System;

namespace LibClang
{
    public class Cursor
    {
        #region Data

        //private
        private SourceRange _extent;
        private string _spelling;
        private Type _type;
        private string _usr;

        #endregion

        #region Properties

        private SourceRange BuildExtent()
        {
            return new SourceRange(Dll.clang_getCursorExtent(Handle));
        }

        public SourceRange Extent
        {
            get { return _extent ?? (_extent = BuildExtent()); }
        }

        #endregion

        internal Dll.Cursor Handle
        {
            get;
            private set;
        }

        internal Cursor(Dll.Cursor handle)
        {
            Handle = handle;
            Kind = Dll.clang_getCursorKind(Handle);
            
        }

        public CursorKind Kind
        {
            get;
            private set;
        }
                
        public string Spelling
        {
            get { return _spelling ?? (_spelling = Dll.clang_getCursorSpelling(Handle).ManagedString); }
        }

        public SourceLocation Location
        {
            get
            {
                return new SourceLocation(Dll.clang_getCursorLocation(Handle));
            }
        }

        public Type Type
        {
            get { return _type ?? (_type = new Type(Dll.clang_getCursorType(Handle))); }
        }

        public string Usr
        {
            get { return _usr ?? (_usr = Dll.clang_getCursorUSR(Handle).ManagedString);}
        
        }

        public bool IsCanonical
        {
            get { return Dll.clang_getCanonicalCursor(Handle) == Handle;}
        }

        public enum ChildVisitResult
        {
            Break,
            Continue,
            Recurse,
        };

        public delegate ChildVisitResult CursorVisitor(Cursor cursor, Cursor parent);

        public void VisitChildren(CursorVisitor visitor)
        {
            Dll.clang_visitChildren(
                Handle,
                (cursor, parent, data) => visitor(new Cursor(cursor), new Cursor(parent)),
                IntPtr.Zero);
        }

        public static bool operator == (Cursor left, Cursor right)
        {
            return left.Handle == right.Handle;
        }

        public static bool operator !=(Cursor left, Cursor right)
        {
            return left.Handle != right.Handle;
        }

        public override string ToString()
        {
            return Kind + " in " + Location.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is Cursor)
            {
                return (obj as Cursor).Handle == Handle;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Handle.GetHashCode();
        }
    }
}
