using System;
using System.Diagnostics;

namespace LibClang.Indexer
{
    public class DeclInfo
    {
        #region Data

        private Dll.IndexerDeclarationInfo _handle;

        private EntityInfo _entityInfo;
        private Cursor _cur;
        private SourceLocation _location;
        private Cursor _lexicalContainer;
        private Cursor _semanticContainer;
        private Cursor _declAsContainer;

        //todo
        
        #endregion

        internal unsafe DeclInfo(Dll.IndexerDeclarationInfo handle)
        {
            _handle = handle;

            _entityInfo = new EntityInfo(*_handle.entityInfo);

            if (handle.semanticContainer != (Dll.IndexerContainerInfo*)IntPtr.Zero)
                _semanticContainer = new Cursor(handle.semanticContainer->cursor);

            if(handle.lexicalContainer != (Dll.IndexerContainerInfo*)IntPtr.Zero)
                _lexicalContainer = new Cursor(handle.lexicalContainer->cursor);

            if(handle.declAsContainer != (Dll.IndexerContainerInfo*)IntPtr.Zero)
                _declAsContainer = new Cursor(handle.declAsContainer->cursor);
            Debug.Assert(Location == Cursor.Location);
            
        }

        #region Properties

        public EntityInfo EntityInfo
        {
            get { return _entityInfo; }
        }

        public Cursor Cursor
        {
            get { return _cur ?? (_cur = new Cursor(_handle.cursor)); }
        }

        public SourceLocation Location
        {
            get { return _location ?? (_location = new SourceLocation(Dll.clang_indexLoc_getCXSourceLocation(_handle.location))); }
        }

        public Cursor LexicalContainer
        {
            get { return _lexicalContainer; }
        }

        public Cursor SemanticContainer
        {
            get { return _semanticContainer; }
        }

        public Cursor DeclarationAsContainer
        {
            get { return _declAsContainer; }
        }

        public bool IsDefinition
        {
            get { return Dll.clang_isCursorDefinition(_cur.Handle) != 0; }
        }

        public bool IsRedefinition
        {
            get { return _handle.isRedeclaration != 0; }
        }

        #endregion
    }
}
