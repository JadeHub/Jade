using System;
using System.Collections.Generic;
using System.IO;
using JadeUtils.IO;
using JadeCore;

namespace JadeGui
{
    public class EditorSourceDocument : JadeCore.IEditorDoc
    {
        #region Data

        private IFileHandle _file;
        private string _content;
        private bool _modified;

        #endregion

        #region Constructor

        public EditorSourceDocument(IFileHandle file)
        {
            _file = file;
        }

        #endregion

        #region Public Properties

        public string Name 
        {
            get { return _file.Name; }
        }

        public FilePath Path 
        {
            get { return _file.Path; }
        }

        public bool Modified 
        {
            get { return _modified; }
            set { _modified = value; }
        }

        public string Content
        {
            get
            {
                if (_content == null)
                {
                    Load();
                }
                return _content;
            }
        }

        #endregion

        #region Private Methods

        private void Load()
        {
            using (FileStream fs = new FileStream(Path.Str, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (StreamReader reader = new StreamReader(fs, System.Text.Encoding.UTF8))
                {
                    _content = reader.ReadToEnd();
                }
            }
        }

        #endregion
    }

    public class EditorController : JadeCore.IEditorController
    {
        #region Data

        private Dictionary<FilePath, IEditorDoc> _openDocuments;
        private IEditorDoc _activeDocument;

        #endregion

        #region Constructor

        public EditorController()
        {
            _openDocuments = new Dictionary<FilePath, IEditorDoc>();
        }

        #endregion

        #region Events

        public event EditorDocChangeEvent DocumentOpened;
        public event EditorDocChangeEvent DocumentClosed;
        public event EditorDocChangeEvent DocumentSelected;

        #endregion

        #region Public Properties

        public JadeCore.IEditorDoc ActiveDocument
        {
            get { return _activeDocument; }
            set { _activeDocument = value; }
        }

        public bool HasOpenDocuments { get { return _openDocuments.Count > 0; } }

        #endregion

        #region Public Methods

        public void OpenSourceFile(IFileHandle file)
        {
            if (_openDocuments.ContainsKey(file.Path) == false)
            {
                IEditorDoc doc = new EditorSourceDocument(file);
                _openDocuments.Add(file.Path, doc);
                ActiveDocument = doc;
                OnDocumentOpen(doc);
            }
        }

        public void CloseAllDocuments()
        {
            List<IEditorDoc> docs = new List<IEditorDoc>(_openDocuments.Values);

            foreach (IEditorDoc doc in docs)
            {
                CloseDocument(doc);
            }
        }

        public void CloseDocument(IEditorDoc doc)
        {
            if (_openDocuments.ContainsKey(doc.Path) == false)
                return;

            _openDocuments.Remove(doc.Path);
            OnDocumentClose(doc);
        }

        #endregion

        #region Private Methods

        private void OnDocumentOpen(IEditorDoc doc)
        {
            RaiseDocEvent(DocumentOpened, doc);
        }

        private void OnDocumentClose(IEditorDoc doc)
        {
            RaiseDocEvent(DocumentClosed, doc);
        }

        private void OnDocumentSelect(IEditorDoc doc)
        {
            RaiseDocEvent(DocumentSelected, doc);
        }

        private void RaiseDocEvent(EditorDocChangeEvent ev, IEditorDoc doc)
        {
            EditorDocChangeEvent handler = ev;
            if (handler != null)
                handler(new EditorDocChangeEventArgs(doc));
        }
        
        #endregion
    }
}
