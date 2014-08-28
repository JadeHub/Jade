using JadeUtils.IO;
using System;
using System.IO;
using ICSharpCode.AvalonEdit.Document;

namespace JadeCore
{
    public class TextDocument : ITextDocument
    {
        private ICSharpCode.AvalonEdit.Document.TextDocument _avDoc;
        private bool _loaded;
        private bool _modified;
        private UInt64 _version;
        private bool _loading; //used to supress text change events while document is loading

        public TextDocument(IFileHandle handle)
        {
            File = handle;
            _loaded = false;
            _avDoc = new ICSharpCode.AvalonEdit.Document.TextDocument();
            _avDoc.TextChanged += OnAvDocTextChanged;
            _avDoc.Changed += OnAvDocChanged;
            _modified = false;
            _version = 0;
        }

        private void OnAvDocChanged(object sender, DocumentChangeEventArgs e)
        {
            if (_loading) return;

            _version++;
            Modified = true;
            EventHandler<DocumentChangeEventArgs> handler = Changed;
            if (handler != null)
                handler(sender, e);
        }

        public event TextChangedEvent TextChanged;
        public event EventHandler ModifiedChanged;
        public event EventHandler<DocumentChangeEventArgs> Changed;

        private void RaiseModifiedChangedEvent()
        {
            EventHandler handler = ModifiedChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        private void OnAvDocTextChanged(object sender, EventArgs e)
        {
            if (_loading) return;
         
            TextChangedEvent handler = TextChanged;
            if (handler != null)
                handler(Version);
        }

        #region Properties

        public ICSharpCode.AvalonEdit.Document.TextDocument AvDoc 
        { 
            get 
            {
                if (!_loaded)
                    Load();
                return _avDoc; 
            }
        }

        public string Name
        {
            get { return File.Name; }
        }

        public IFileHandle File
        {
            get;
            private set;
        }

        public int TextLength 
        {
            get { return AvDoc.TextLength; }
        }

        public string Text 
        {
            get { return AvDoc.Text; }
        }

        public bool Modified 
        {
            get { return _modified; }
            private set
            {
                if(_modified != value)
                {
                    _modified = value;
                    RaiseModifiedChangedEvent();
                }
            }
        }

        public UInt64 Version 
        {
            get { return _version; }
        }

        #endregion

        #region Public Methods

        public void DiscardChanges()
        {
            if (!_modified || !_loaded)
                return;
            Load();
        }

        public bool Save(IFileHandle file)
        {
            if (!_loaded || !_modified)
                return false;

            using (FileStream fs = new FileStream(file.Path.Str, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                using (StreamWriter writer = new StreamWriter(fs, System.Text.Encoding.ASCII))
                {
                    writer.Write(AvDoc.Text);
                    Modified = false;
                }
            }
            File = file;
            return true;
        }

        public bool GetLineAndColumnForOffset(int offset, out int line, out int col)
        {
            line = col = 0;
            if (offset > this.TextLength) return false;
         
            ISegment lineSeg = GetLineForOffset(offset);
            if (lineSeg == null) return false;            
            line = GetLineNumForOffset(offset);
            col = offset - lineSeg.Offset + 1;
            return true;
        }

        public int GetLineNumForOffset(int offset)
        {
            return AvDoc.GetLineByOffset(offset).LineNumber;
        }
        
        public ISegment GetLineForOffset(int offset)
        {
            return AvDoc.GetLineByOffset(offset);
        }

        public string GetText(ISegment segment)
        {
            return AvDoc.GetText(segment);
        }

        public TextDocumentSnapshot CreateSnapshot()
        {
            //lock?
            ICSharpCode.AvalonEdit.Document.ChangeTrackingCheckpoint checkPoint;
            string text = AvDoc.CreateSnapshot(out checkPoint).Text;
            return new TextDocumentSnapshot(this, Version, AvDoc.CreateSnapshot().Text);
        }

        #endregion
        
        #region Private Methods

        private void Load()
        {
            _loading = true;

            try
            {
                using (FileStream fs = new FileStream(File.Path.Str, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (StreamReader reader = new StreamReader(fs, System.Text.Encoding.ASCII, false))
                    {
                        _avDoc.Text = reader.ReadToEnd();
                        _loaded = true;
                        Modified = false;
                    }
                }
            }
            finally
            {
                _loading = false;
            }
        }
                
        #endregion
    }
}
