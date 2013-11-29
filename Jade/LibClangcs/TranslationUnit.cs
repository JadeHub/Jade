using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace LibClang
{
    public class TranslationUnit : IDisposable
    {
        #region Data

        private IntPtr _handle;
        private string _filename;
        private Index _index;

        private File _file;
        
        #endregion        
                
        public TranslationUnit(Index idx, string filename)
        {
            _index = idx;
            _filename = filename;
        }

        public void Dispose()
        {
            if (Valid)
            {
                Dll.clang_disposeTranslationUnit(_handle);
                _handle = IntPtr.Zero;
            }
        }

        public bool Parse()
        {
            if (Valid)
                return Reparse();

            if (!System.IO.File.Exists(Filename))
            {
                throw new System.IO.FileNotFoundException("Couldn't find input file.", Filename);
            }
                      
            _handle = Dll.clang_parseTranslationUnit(_index.Handle, Filename, null, 0, null, 0, 0);//(int)TranslationUnitFlags.PrecompiledPreamble);
            return Valid;
        }

        private bool Reparse()
        {
            Debug.Assert(Valid);
            if (Dll.clang_reparseTranslationUnit(_handle, 0, null, 0) != 0)
            {
                Dll.clang_disposeTranslationUnit(_handle);
                _handle = IntPtr.Zero;
            }
            return Valid;
        }
        
        public Cursor GetCursorAt(SourceLocation location)
        {
            Dll.Cursor cur = Dll.clang_getCursor(Handle, location.Handle);
            if (cur.IsNull())
                return null;
            return new Cursor(cur);
        }

        public Cursor GetCursorAt(string path, uint offset)
        {
            IntPtr f = Dll.clang_getFile(Handle, path);
            if (f == IntPtr.Zero)
            {
                return null;
            }

            Dll.SourceLocation loc = Dll.clang_getLocationForOffset(Handle, f, offset);
            return GetCursorAt(new SourceLocation(loc));
        }

        #region Properties

        public string Filename
        {
            get { return _filename; }
        }

        public Cursor Cursor
        {
            get { return new Cursor(Dll.clang_getTranslationUnitCursor(_handle)); }
        }

        public string Spelling
        {
            get { return Dll.clang_getTranslationUnitSpelling(_handle).ManagedString; }
        }

        public File File
        {
            get
            {
                return _file ?? (_file = new File(Dll.clang_getFile(_handle, _filename)));
            }
        }

        public bool Valid
        {
            get { return _handle != IntPtr.Zero;}
        }

        public IntPtr Handle
        {
            get { return _handle; }
        }
        
        #endregion

        public override int GetHashCode()
        {
            return Handle.ToInt32();
        }
    }
}
