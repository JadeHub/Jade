using System;
using System.Diagnostics;

namespace LibClang
{
    /// <summary>
    /// An immutable wrapper around libclang's File type.
    /// A File represents a single code file (source or header) in a Translation Unit
    /// </summary>
    public sealed class File
    {
        internal delegate File CreateFileDel(IntPtr handle);

        #region Data

        private string _name = null;
        private ITranslationUnitItemFactory _itemFactory;
        
        #endregion

        #region Constructor

        internal File(IntPtr handle, ITranslationUnitItemFactory itemFactory)
        {            
            Debug.Assert(handle != IntPtr.Zero);
            Handle = handle;
            _itemFactory = itemFactory;
        }

        #endregion

        #region Properties

        internal IntPtr Handle
        {
            get;
            private set;
        }       

        public string Name
        {
            get { return _name ?? (_name = Library.clang_getFileName(Handle).ManagedString); }
        }

        public DateTime Time
        {
            get
            {
                var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                return epoch.AddSeconds(Library.clang_getFileTime(Handle));
            }
        }

        #endregion

        #region Find References

        public bool FindAllReferences(Cursor c, Func<Cursor, SourceRange, bool> callback)
        {
            Library.CXCursorAndRangeVisitor visitor = new Library.CXCursorAndRangeVisitor();
            visitor.context = IntPtr.Zero;
            visitor.visit = delegate(IntPtr ctx, Library.CXCursor cur, Library.CXSourceRange range)
            {
                if (callback(_itemFactory.CreateCursor(cur), _itemFactory.CreateSourceRange(range)) == true)
                    return Library.CXVisitorResult.CXVisit_Continue;
                return Library.CXVisitorResult.CXVisit_Break;
            };
            return Library.clang_findReferencesInFile(c.Handle, Handle, visitor) != Library.CXResult.CXResult_Invalid;
        }

        #endregion

        #region object overrides

        public override string ToString()
        {
            return Name;
        }

        public override int GetHashCode()
        {
            return Handle.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if(obj != null && obj is File)
                return Handle.Equals(((File)obj).Handle);
            return false;
        }

        #endregion

        #region Static operator functions

        public static bool operator ==(File left, File right)
        {
            if ((object)left == null && (object)right == null)
                return true;
            if ((object)left == null || (object)right == null)
                return false;
            return left.Handle == right.Handle;
        }

        public static bool operator !=(File left, File right)
        {
            if ((object)left == null && (object)right == null)
                return false;
            if ((object)left == null || (object)right == null)
                return true;
            return left.Handle != right.Handle;
        }
        
        #endregion
    }
}
