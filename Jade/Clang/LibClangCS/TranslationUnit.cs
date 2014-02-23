using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LibClang
{    
    /// <summary>
    /// A wrapper around libclang's TranslationUnit type.
    /// A TranslationUnit is not Valid until Parse() is called.
    /// Each call to Parse() can change the Valid state.
    /// If Parse() is called when Valid is true a Reparse() is attempted.
    /// </summary>
    public sealed class TranslationUnit : IDisposable
    {
        public struct HeaderInfo
        {
            public HeaderInfo(File file, IEnumerable<SourceLocation> stack)
            {
                File = file;
                InclusionStack = stack;
            }

            public readonly File File;
            public readonly IEnumerable<SourceLocation> InclusionStack;

            public override string ToString()
            {
                return File.Name;
            }

            public override int GetHashCode()
            {
                return File.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if (obj != null)
                {
                    return ((HeaderInfo)obj).File.Equals(File);
                }
                return false;
            }
        }

        #region Data

        private string _filename;
        private Index _index;
        private Cursor _cursor;
        private File _file;
        private DiagnosticSet _diagSet;

        private ITranslationUnitItemStore _itemStore;

        private HashSet<HeaderInfo> _headerFiles;
                
        #endregion        

        #region Lifetime

        internal TranslationUnit(Index idx, string filename, ITranslationUnitItemStore itemStore)
        {
            _index = idx;
            _filename = filename;
            _itemStore = itemStore;
            _headerFiles = new HashSet<HeaderInfo>();
        }

        public TranslationUnit(Index idx, string filename)
        {
            _index = idx;
            _filename = filename;
            _itemStore = new TranslationUnitItemStore();
            _headerFiles = new HashSet<HeaderInfo>();
        }

        public void Dispose()
        {
            if (Valid)
            {
                DisposeDiagnosticSet();
                Library.clang_disposeTranslationUnit(Handle);
                Handle = IntPtr.Zero;
                GC.SuppressFinalize(this);
            }
        }

        private void DisposeDiagnosticSet()
        {
            if (_diagSet != null)
            {
                _diagSet.Dispose();
                _diagSet = null;
            }
        }
        
        #endregion

        #region Public Methods

        private void ResetState()
        {
            _itemStore.Clear();
            _headerFiles.Clear();
            DisposeDiagnosticSet();
        }

        public bool Parse(string[] cmdLineParams)
        {
            ResetState();
            if (!System.IO.File.Exists(Filename))
            {
                throw new System.IO.FileNotFoundException("Couldn't find input file.", Filename);
            }

            if (Valid)
                return Reparse();

            Handle = Library.clang_parseTranslationUnit(_index.Handle, Filename,
                                                    cmdLineParams, cmdLineParams != null ? cmdLineParams.Length : 0,
                                                    null, 0,
                                                    (int)TranslationUnitFlags.DetailedPreprocessingRecord);
            if (!Valid)
            {
                ResetState();
            }
            return Valid;
        }

        private bool Reparse()
        {
            Debug.Assert(Valid);
            if (Library.clang_reparseTranslationUnit(Handle, 0, null, 0) != 0)
            {
                _itemStore.Clear();
                Library.clang_disposeTranslationUnit(Handle);
                Handle = IntPtr.Zero;
            }
            return Valid;
        }

        public SourceRange GetSourceRange(SourceLocation start, SourceLocation end)
        {
            Library.SourceRange handle = Library.clang_getRange(start.Handle, end.Handle);
            return _itemStore.CreateSourceRange(handle);
        }

        public File GetFile(string path)
        {
            IntPtr f = Library.clang_getFile(Handle, path);
            if (f == IntPtr.Zero)
            {
                return null;
            }
            return _itemStore.CreateFile(f);
        }      

        public SourceLocation GetLocation(string path, int offset)
        {
            IntPtr f = Library.clang_getFile(Handle, path);
            if (f == IntPtr.Zero)
            {
                return null;
            }
            Library.SourceLocation sloc = Library.clang_getLocationForOffset(Handle, f, (uint)offset);
            if (sloc.IsNull)
                return null;

            //Library.clang_getLocationForOffset() does not range check the offset. If the offset is not valid 
            //Library.clang_getLocationForOffset() will return a non-null Library.SourceLocation object which, when 
            //translated back, produces an offset which does not match the offset passed in.
            if (!SourceLocation.IsValid(sloc, f, offset))
            {
                return null;
            }           

            SourceLocation result = _itemStore.CreateSourceLocation(sloc);
            if (result.Offset != offset)
                return null;
            return result;
        }

        public Cursor GetCursorAt(string path, int offset)
        {
            SourceLocation loc = GetLocation(path, offset);
            if (loc == null)
                return null;
            return GetCursorAt(loc);
        }

        public Cursor GetCursorAt(SourceLocation location)
        {
            if (location == null)
                throw new ArgumentException("null Location passed to GetCursotAt().");

            Library.Cursor cur = Library.clang_getCursor(Handle, location.Handle);
            if (cur.IsNull)
                return null;
            cur = Library.clang_getCursor(Handle, location.Handle);
            return _itemStore.CreateCursor(cur);
        }

        public bool FindAllReferences(Cursor c, Func<Cursor, SourceRange, bool> callback)
        {
            Library.CXCursorAndRangeVisitor visitor = new Library.CXCursorAndRangeVisitor();
            visitor.context = IntPtr.Zero;
            visitor.visit = delegate(IntPtr ctx, Library.Cursor cur, Library.SourceRange range)
            {
                if (callback(_itemStore.CreateCursor(cur), _itemStore.CreateSourceRange(range)) == true)
                    return Library.CXVisitorResult.CXVisit_Continue;
                return Library.CXVisitorResult.CXVisit_Break;
            };
            
            //Search source file
            if (Library.clang_findReferencesInFile(c.Handle, File.Handle, visitor) == Library.CXResult.CXResult_Invalid)
                return false;

            foreach (HeaderInfo header in _headerFiles)
            {
                if (Library.clang_findReferencesInFile(c.Handle, header.File.Handle, visitor) == Library.CXResult.CXResult_Invalid)
                    return false;
            }

            return true;
        }
        
        #endregion

        #region Properties

        internal ITranslationUnitItemFactory ItemFactory { get { return _itemStore; } }

        private unsafe void LoadHeaderFiles()
        {
            Library.CXInclusionVisitor callBack =
                delegate(IntPtr fileHandle, Library.SourceLocation* inclusionStack, uint includeStackSize, IntPtr clientData)
                {
                    if (includeStackSize > 0)
                    {
                        List<SourceLocation> locationStack = new List<SourceLocation>();

                        for (uint i = 0; i < includeStackSize; i++)
                        {
                            locationStack.Add(_itemStore.CreateSourceLocation(inclusionStack[i]));
                        }

                        Cursor includeStatement = GetCursorAt(locationStack[0]);

                        File file = _itemStore.CreateFile(fileHandle);

                        HeaderInfo header = new HeaderInfo(file, locationStack);
                        _headerFiles.Add(header);
                    }
                };

            Library.clang_getInclusions(Handle, callBack, IntPtr.Zero);
        }

        public IEnumerable<HeaderInfo> HeaderFiles
        {
            get
            {
                if (_headerFiles.Count == 0)
                    LoadHeaderFiles();
                return _headerFiles;
            }
        }

        public string Filename
        {
            get { return _filename; }
        }

        public Cursor Cursor
        {
            get { return _cursor ?? (_cursor = _itemStore.CreateCursor(Library.clang_getTranslationUnitCursor(Handle))); }
        }

        public string Spelling
        {
            get { return Library.clang_getTranslationUnitSpelling(Handle).ManagedString; }
        }

        public File File
        {
            get
            {
                return _file ?? (_file = _itemStore.CreateFile(Library.clang_getFile(Handle, _filename)));
            }
        }

        public bool Valid
        {
            get { return Handle != IntPtr.Zero; }
        }

        public IEnumerable<Diagnostic> Diagnostics
        {
            get
            {
                if (_diagSet == null)
                {
                    _diagSet = new DiagnosticSet(Library.clang_getDiagnosticSetFromTU(Handle), _itemStore);
                }
                return _diagSet.Diagnostics;
            }
        }

        internal IntPtr Handle
        {
            get;
            private set;
        }
        
        #endregion

        #region object overrides

        public override string ToString()
        {
            return Spelling;
        }

        public override int GetHashCode()
        {
            return Handle.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is TranslationUnit)
            {
                return Handle.Equals(((File)obj).Handle);
            }
            return false;
        }

        #endregion

        #region Static operator functions

        public static bool operator ==(TranslationUnit left, TranslationUnit right)
        {
            if ((object)left == null && (object)right == null)
                return true;
            if ((object)left == null || (object)right == null)
                return false;
            return left.Handle == right.Handle;
        }

        public static bool operator !=(TranslationUnit left, TranslationUnit right)
        {
            if ((object)left == null && (object)right == null)
                return false;
            if ((object)left == null || (object)right == null)
                return true;
            return left.Handle != right.Handle;
        }

        #endregion

        #region Find References to Cursor

        /// <summary>
        /// Callback for FindReferencesTo
        /// </summary>
        /// <param name="c"></param>
        /// <param name="r"></param>
        /// <returns>True to continue searching, False to stop.</returns>
        public delegate bool FindReferencesDelegate(Cursor c, SourceRange r);
        
        public bool FindReferencesTo(Cursor c, File f, FindReferencesDelegate callback)
        {
            Library.CXCursorAndRangeVisitor visitor = new Library.CXCursorAndRangeVisitor();
            visitor.context = IntPtr.Zero;
            visitor.visit = delegate(IntPtr ctx, Library.Cursor cur, Library.SourceRange range)
            {
                if(callback(_itemStore.CreateCursor(cur), _itemStore.CreateSourceRange(range)) == true)
                    return Library.CXVisitorResult.CXVisit_Continue;
                return Library.CXVisitorResult.CXVisit_Break;
            };
            return Library.clang_findReferencesInFile(c.Handle, f.Handle, visitor) != Library.CXResult.CXResult_Invalid;
        }
        
        #endregion
    }
}
