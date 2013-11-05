using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibClang.Indexer
{
    public class EntityReference
    {
        #region Data

        private Dll.IndexerEntityReferenceInfo _handle;
        private Cursor _cursor;
        private SourceLocation _location;
        private EntityInfo _refedEntity;
        private EntityInfo _parentEntity;
        private Cursor _container;

        #endregion

        internal unsafe EntityReference(Dll.IndexerEntityReferenceInfo handle)
        {
            _handle = handle;
            if (_handle.referencedEntity != (Dll.EntityInfo*)IntPtr.Zero)
                _refedEntity = new EntityInfo(*_handle.referencedEntity);
            if (_handle.parentEntity != (Dll.EntityInfo*)IntPtr.Zero)
                _parentEntity = new EntityInfo(*_handle.parentEntity);
            if(_handle.container != (Dll.IndexerContainerInfo*)IntPtr.Zero)
                _container = new Cursor(_handle.container->cursor);
        }

        public override string ToString()
        {
            return string.Format("Ref To {0} from {1}", _refedEntity.Name, Location);
        }

        #region Properties

        public EntityReferenceKind Kind
        {
            get { return _handle.kind; }
        }

        public Cursor Cursor
        {
            get { return _cursor ?? (_cursor = new Cursor(_handle.cursor)); }
        }

        public SourceLocation Location
        {
            get { return _location ?? (_location = new SourceLocation(Dll.clang_indexLoc_getCXSourceLocation(_handle.location))); }
        }

        public EntityInfo ReferencedEntity
        {
            get { return _refedEntity;}
        }

        public EntityInfo ParentEntity
        {
            get { return _parentEntity; }
        }

        public Cursor Container
        {
            get { return _container; }
        }

        #endregion
    }
}
