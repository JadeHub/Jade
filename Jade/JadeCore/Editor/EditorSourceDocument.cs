using JadeUtils.IO;
using System;
using System.IO;

namespace JadeCore.Editor
{
    public class EditorSourceDocument : JadeCore.IEditorDoc
    {
        #region Data

        private ITextDocument _document;        
        
        #endregion

        #region Constructor

        public EditorSourceDocument(ITextDocument document)
        {
            _document = document;
        }

        #endregion

        #region Events

        public event EventHandler OnClosing;

        private void RaiseOnClosing()
        {
            EventHandler handler = OnClosing;
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
            _document.Save(_document.File);
        }

        #endregion

        #region Public Properties

        public ITextDocument TextDocument { get { return _document; } }
        
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
                return _document.Modified;
            }
            
        }

        #endregion
    }
}
