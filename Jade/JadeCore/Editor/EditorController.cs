﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using JadeUtils.IO;

namespace JadeCore.Editor
{
    public class CodeLocation
    {
        public CodeLocation(int line, int column, int offset)
        {
            Line = line;
            Column = column;
            Offset = offset;
        }

        public int Line
        {
            get;
            set;
        }

        public int Column
        {
            get;
            set;
        }

        public int Offset
        {
            get;
            set;
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}", Line, Column);
        }

        public override bool Equals(object obj)
        {
            if (obj is CodeLocation)
            {
                CodeLocation rhs = obj as CodeLocation;
                return Offset == rhs.Offset;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Offset;
        }
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
        public event EditorDocChangeEvent DocumentSelected;

        #endregion

        #region Public Properties

        public JadeCore.IEditorDoc ActiveDocument
        {
            get { return _activeDocument; }
            set 
            {
                if (_activeDocument != value)
                {
                    //set view model current document
                    _activeDocument = value;
                    OnDocumentSelect(value);
                }
            }
        }

        public bool HasOpenDocuments { get { return _openDocuments.Count > 0; } }

        public IEnumerable<IEditorDoc> ModifiedDocuments
        {
            get
            {
                return _openDocuments.Where(doc => doc.Value.Modified).Select(doc => doc.Value);
            }
        }

        #endregion

        #region Public Methods

        public void OpenDocument(IFileHandle file)
        {
            if (_openDocuments.ContainsKey(file.Path) == false)
            {
                IEditorDoc doc = new EditorSourceDocument(file);
                _openDocuments.Add(file.Path, doc);
                ActiveDocument = doc;
                OnDocumentOpen(doc);
            }
            else
            {
                ActiveDocument = _openDocuments[file.Path];
            }
        }

        public void SaveActiveDocument()
        {
            Debug.Assert(ActiveDocument != null);
            Debug.Assert(ActiveDocument.File.Exists);
            ActiveDocument.Save();
        }

        public void CloseActiveDocument()
        {
            if(ActiveDocument != null)
                CloseDocument(ActiveDocument);
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
            if (ActiveDocument != null && ActiveDocument.Equals(doc))
                ActiveDocument = null;
            doc.Close();
        }

        public void Display(JadeUtils.IO.FilePath path, int line, int col)
        {
            IFileHandle f = Services.Provider.FileService.MakeFileHandle(path);

            OpenDocument(f);
        }

        #endregion

        #region Private Methods

        private void OnDocumentOpen(IEditorDoc doc)
        {
            RaiseDocEvent(DocumentOpened, doc);
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
