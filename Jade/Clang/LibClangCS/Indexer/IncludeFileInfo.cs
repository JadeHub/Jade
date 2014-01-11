
namespace LibClang.Indexer
{
    /*
    public class IncludeFileInfo
    {
        #region Data

        private Library.IndexerIncludeFileInfo _handle;
        private string _fileName;
        private SourceLocation _location;
        private File _file;

        #endregion

        #region Constructor

        internal unsafe IncludeFileInfo(Library.IndexerIncludeFileInfo handle)
        {
            _handle = handle;
            _fileName = new string(_handle.fileName);
        }

        #endregion

        #region Properties

        public SourceLocation Location
        {
            get { return _location ?? (_location = new SourceLocation(Library.clang_indexLoc_getCXSourceLocation(_handle.location))); }
        }

        public File File
        {
            get { return _file ?? (_file = new File(_handle.file)); }
        }

        public bool IsImport
        {
            get { return _handle.isImport != 0; }
        }

        public bool IsAngled
        {
            get { return _handle.isAngled != 0; }
        }

        public bool IsModuleImport
        {
            get { return _handle.isAngled != 0; }
        }

        #endregion
    }
     * */
}
