using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibClang
{
    public class TranslationUnit
    {
        #region Data

        public IntPtr Handle 
        { 
            get; 
            private set; 
        }

        private File _file;
        
        #endregion

        public string Filename
        {
            get;
            private set;
        }
                
        internal TranslationUnit(string filename, IntPtr handle)
        {
            Filename = filename;
            Handle = handle;
        }

        public Cursor Cursor
        {
            get { return new Cursor(Dll.clang_getTranslationUnitCursor(Handle)); }
        }

        public string Spelling
        {
            get { return Dll.clang_getTranslationUnitSpelling(Handle).ManagedString; }
        }

        public File File
        {
            get
            {
                return _file ?? (_file = new File(Dll.clang_getFile(Handle, Filename)));
            }
        }
    }
}
