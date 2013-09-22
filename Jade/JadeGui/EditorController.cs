using System;
using System.Collections.Generic;
using JadeUtils.IO;
using JadeCore;

namespace JadeGui
{
    public class EditorSourceDocument : JadeCore.IEditorDoc
    {
        #region Data

        IFileHandle _file;

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
            get { return false; }
        }

        #endregion
    }

    public class EditorController : JadeCore.IEditorController
    {
        #region Data


        private Dictionary<FilePath, IEditorDoc> _openDocuments;

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
            get { return null; }
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
