using JadeUtils.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace JadeCore.Editor
{
    public class EditorController : JadeCore.IEditorController
    {
        #region Data

        private Dictionary<IFileHandle, IEditorDoc> _allDocuments;
        private Dictionary<IFileHandle, IEditorDoc> _openDocuments;
        private IEditorDoc _activeDocument;
                
        #endregion

        #region Constructor

        public EditorController()
        {
            _openDocuments = new Dictionary<IFileHandle, IEditorDoc>();
            _allDocuments = new Dictionary<IFileHandle, IEditorDoc>();
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
            if (_openDocuments.ContainsKey(file) == false)
            {
                IEditorDoc doc = FindOrAddDocument(file);
                _openDocuments.Add(file, doc);
                OnDocumentOpen(doc);
                ActiveDocument = doc;        
            }
            //this doc is already open, if it's not the active document, activate it
            else if(ActiveDocument == null || ActiveDocument.File != file)
            {
                ActiveDocument = _openDocuments[file];
            }
         }

        public void SaveActiveDocument()
        {
            Debug.Assert(ActiveDocument != null);
            ActiveDocument.Save();
        }

        public void CloseActiveDocument()
        {
            if(ActiveDocument != null)
                CloseDocument(ActiveDocument);
        }

        public void DiscardChangesToActiveDocument()
        {
            if (ActiveDocument == null)
                return;
            ActiveDocument.TextDocument.DiscardChanges();
        }

        public void CloseAllDocuments()
        {
            List<IEditorDoc> docs = new List<IEditorDoc>(_openDocuments.Values);

            foreach (IEditorDoc doc in docs)
            {
                CloseDocument(doc);
            }
        }

        private void CloseDocument(IEditorDoc doc)
        {
            _openDocuments.Remove(doc.File);
            if (ActiveDocument != null && ActiveDocument.Equals(doc))
                ActiveDocument = null;
            doc.Close();
        }

        public void Reset()
        {
            CloseAllDocuments();
            _allDocuments.Clear();
        }

        #endregion

        #region Private Methods

        private CppCodeBrowser.IProjectIndex GetProjectIndexForFile(FilePath path)
        {
            if(Services.Provider.WorkspaceController.CurrentWorkspace != null)
            {
                Project.IProject project = Services.Provider.WorkspaceController.CurrentWorkspace.FindProjectForFile(path);
                if (project != null)
                    return project.SourceIndex;
            }
            return null;
        }

        private IEditorDoc FindOrAddDocument(IFileHandle file)
        {
            IEditorDoc result;

            if(!_allDocuments.TryGetValue(file, out result))
            {
                //find project?
                ITextDocument doc = Services.Provider.ContentProvider.OpenTextDocument(file);
                result = new EditorSourceDocument(doc, GetProjectIndexForFile(file.Path));
                _allDocuments.Add(file, result);
            }
            return result;
        }

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
