using System;
using System.Diagnostics;

namespace LibClang
{
    /// <summary>
    /// Cache of items contained in a TranslationUnit
    /// </summary>
    internal interface ITranslationUnitItemFactory
    {
        Cursor.CreateCursorDel CreateCursor { get; }
        File.CreateFileDel CreateFile { get; }
        SourceLocation.CreateSourceLocationDel CreateSourceLocation { get; }
        SourceRange.CreateSourceRangeDel CreateSourceRange { get; }
        Type.CreateTypeDel CreateType { get; }
    }

    internal interface ITranslationUnitItemStore : ITranslationUnitItemFactory
    {
        void Clear();
    }

    internal class TranslationUnitItemStore : ITranslationUnitItemStore
    {
        private CursorStore _cursorStore;
        private FileStore _fileStore;
        private SourceLocationStore _locationStore;
        private SourceRangeStore _sourceRangeStore;
        private TypeStore _typeStore;

        public TranslationUnitItemStore(TranslationUnit tu)
        {
            _cursorStore = new CursorStore(tu, this);
            _fileStore = new FileStore(this);
            _locationStore = new SourceLocationStore(this);
            _sourceRangeStore = new SourceRangeStore(tu, this);
            _typeStore = new TypeStore(this);
        }

        public Cursor.CreateCursorDel CreateCursor
        {
            get { return _cursorStore.FindOrAdd; }
        }

        public File.CreateFileDel CreateFile
        {
            get { return _fileStore.FindOrAdd; }
        }

        public SourceLocation.CreateSourceLocationDel CreateSourceLocation
        {
            get { return _locationStore.FindOrAdd; }
        }

        public SourceRange.CreateSourceRangeDel CreateSourceRange
        {
            get { return _sourceRangeStore.FindOrAdd; }
        }

        public Type.CreateTypeDel CreateType
        {
            get { return _typeStore.FindOrAdd; }
        }

        public void Clear()
        {
            _cursorStore.Clear();
            _fileStore.Clear();
            _locationStore.Clear();
            _sourceRangeStore.Clear();
            _typeStore.Clear();
        }
    }
}
