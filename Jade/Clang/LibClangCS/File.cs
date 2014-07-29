using System;
using System.Diagnostics;

namespace LibClang
{
    /// <summary>
    /// An immutable wrapper around libclang's File type.
    /// A File represents a single code file (source or header) in a Translation Unit
    /// </summary>
    public sealed class File : IComparable
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

        /// <summary>
        /// Returns the complete path and file name.
        /// </summary>
        public string Name
        {
            get { return _name ?? (_name = Library.clang_getFileName(Handle).ManagedString); }
        }

        /// <summary>
        /// Returns the file's last modified time
        /// </summary>
        public DateTime LastModifiedTime
        {
            get
            {
                var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                return epoch.AddSeconds(Library.clang_getFileTime(Handle));
            }
        }

        /// <summary>
        /// Returns the file's unique id
        /// </summary>
        public Tuple<UInt64, UInt64, UInt64> UniqueId
        {
            get
            {
                unsafe
                {
                    Library.CXFileUniqueID id;
                    if (Library.clang_getFileUniqueID(Handle, &id) != 0)
                    {
                        return new Tuple<ulong, ulong, ulong>(id.data1, id.data2, id.data3);
                    }
                }
                return null;
            }
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

        #region IComparable implementation

        public int CompareTo(object obj)
        {
            File other = obj as File;
            if (other == null)
            {
                throw new ArgumentException("Incorrect type for comparison", "obj");
            }
            if (Handle == other.Handle)
            {
                return 0;
            }
            if (Handle.ToInt32() < other.Handle.ToInt32())
            {
                return -1;
            }
            return 1;
        }

        #endregion
    }
}
