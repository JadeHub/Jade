
namespace LibClang.Indexer
{
    public class EntityInfo
    {
        #region Data

        private Dll.EntityInfo _handle;
        private string _name;
        private string _usr;
        private Cursor _cur;

        #endregion

        internal unsafe EntityInfo(Dll.EntityInfo handle)
        {
            _handle = handle;
            _name = new string(_handle.name);
            _usr = new string(_handle.USR);
        }

        public override string ToString()
        {
            return Cursor.Kind.ToString() + ":" + _name + " " + _cur.ToString();
        }

        #region Properties

        public string Name
        {
            get { return _name;}
        }

        public string Usr
        {
            get { return _usr; }
        }

        public EntityKind Kind
        {
            get { return _handle.kind; }
        }

        public EntityCXXTemplateKind TemplateKind
        {
            get { return _handle.templateKind; }
        }

        public EntityLanguage Language
        {
            get { return _handle.language; }
        }

        public Cursor Cursor
        {
            get { return _cur ?? (_cur = new Cursor(_handle.cursor)); }
        }

        #endregion
    }
}
