using System;

namespace LibClang
{
    public class File
    {
        private string _name = null;
                
        internal IntPtr Handle
        { 
            get; 
            private set; 
        }

        internal File(IntPtr handle)
        {
            Handle = handle;
        }

        public string Name
        {
            get 
            { 
                return _name ?? (_name = Dll.clang_getFileName(Handle).ManagedString); 
            }
        }

        public DateTime Time
        {
            get
            {
                var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                return epoch.AddSeconds(Dll.clang_getFileTime(Handle));
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public static bool operator ==(File first, File second)
        {
            return first.Handle == second.Handle;
        }

        public static bool operator !=(File first, File second)
        {
            return first.Handle != second.Handle;
        }

        public override int GetHashCode()
        {
            return Handle.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Handle.Equals(((File)obj).Handle);
        }
    }
}
