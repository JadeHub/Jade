using JadeUtils.IO;
using System;
using System.IO;

namespace JadeCore.Editor
{
    public class EditorSourceDocument : JadeCore.IEditorDoc
    {
        #region Data

        private ITextDocument _document;        
        private string _content;
        private bool _modified;

        #endregion

        #region Constructor

        public EditorSourceDocument(ITextDocument document)
        {
            _document = document;
        }

        #endregion

        #region Events

        public event EventHandler OnClosing;
        public event EventHandler OnSaved;

        private void RaiseOnClosing()
        {
            EventHandler handler = OnClosing;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        private void RaiseOnSaved()
        {
            EventHandler handler = OnSaved;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion

        #region Public Methods

        public void Close()
        {
            RaiseOnClosing();
        }

        public void Save()
        {
            using (FileStream fs = new FileStream(File.Path.Str, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                using (StreamWriter writer = new StreamWriter(fs, System.Text.Encoding.ASCII))
                {
                    System.Diagnostics.Debug.Write(Content);
                    writer.Write(Content);
                    Modified = false;
                    RaiseOnSaved();
                }
            }
        }

        #endregion

        #region Public Properties

        public string Name
        {
            get { return _document.Name; }
        }

        public IFileHandle File
        {
            get { return _document.File; }
        }

        public bool Modified
        {
            get
            {
                return _modified;
            }
            set 
            { 
                _modified = value; 
            }
        }

        public string Content
        {
            get
            {
                return Modified ? _content : _document.Content;
            }

            set
            {
                Modified = true;
                _content = value;
            }
        }

        #endregion

        public void AddFileObserver(IFileObserver observer) { }
        public void RemoveFileObserver(IFileObserver observer) { }
    }
}
