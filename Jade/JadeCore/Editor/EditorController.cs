using JadeUtils.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace JadeCore.Editor
{
    public class EditorController : JadeCore.IEditorController
    {
        #region Data

       // private Dictionary<IFileHandle, IEditorDoc> _allDocuments;
        private Dictionary<IFileHandle, IEditorDoc> _openDocuments;
        private IEditorDoc _activeDocument;
        private IDictionary<Project.IProject, CppCodeBrowser.IIndexBuilder> _projectBuilders;
                
        #endregion

        #region Constructor

        public EditorController()
        {
            _openDocuments = new Dictionary<IFileHandle, IEditorDoc>();
         //   _allDocuments = new Dictionary<IFileHandle, IEditorDoc>();

            _projectBuilders = new Dictionary<Project.IProject, CppCodeBrowser.IIndexBuilder>();
        }

        public void Dispose()
        {
            CloseAllDocuments();
        }

        #endregion

        #region Events

        public event EditorDocChangeEvent ActiveDocumentChanged;
        public event EditorDocChangeEvent DocumentOpened;
        public event EditorDocChangeEvent DocumentClosed;

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
                IEditorDoc doc;

                //find project?
                ITextDocument textDoc = JadeCore.Services.Provider.WorkspaceController.DocumentCache.FindOrAdd(file);
                //result = new EditorSourceDocument(doc, GetProjectIndexForFile(file.Path));
                CppCodeBrowser.IIndexBuilder ib = null;
                Project.IProject p = GetProjectForFile(file.Path);
                if (p != null)
                    ib = GetProjectBuilder(p);
                doc = new SourceDocument(this, textDoc, ib);
                _openDocuments.Add(file, doc);
                OnDocumentOpened(doc);
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
            {
                CloseDocument(ActiveDocument);
            }
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
            OnDocumentClosed(doc);
            doc.Dispose();
        }

        public void Reset()
        {
            CloseAllDocuments();
         //   _allDocuments.Clear();
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

        private Project.IProject GetProjectForFile(FilePath path)
        {
            if (Services.Provider.WorkspaceController.CurrentWorkspace != null)
            {
                return Services.Provider.WorkspaceController.CurrentWorkspace.FindProjectForFile(path);
            }
            return null;
        }

        private IEditorDoc FindOrAddDocument(IFileHandle file)
        {
            IEditorDoc result;

            if (!_openDocuments.TryGetValue(file, out result))
            {
                //find project?
                ITextDocument doc = JadeCore.Services.Provider.WorkspaceController.DocumentCache.FindOrAdd(file);
                //result = new EditorSourceDocument(doc, GetProjectIndexForFile(file.Path));
                CppCodeBrowser.IIndexBuilder ib = null;
                Project.IProject p = GetProjectForFile(file.Path);
                if(p != null)
                    ib = GetProjectBuilder(p);
                result = new SourceDocument(this, doc, ib);
                _openDocuments.Add(file, result);
            }
            return result;
        }

        private void OnDocumentClosed(IEditorDoc doc)
        {
            RaiseDocEvent(DocumentClosed, doc);
        }

        private void OnDocumentOpened(IEditorDoc doc)
        {
            RaiseDocEvent(DocumentOpened, doc);
        }
                
        private void OnDocumentSelect(IEditorDoc doc)
        {
            RaiseDocEvent(ActiveDocumentChanged, doc);
        }

        private void RaiseDocEvent(EditorDocChangeEvent ev, IEditorDoc doc)
        {
            EditorDocChangeEvent handler = ev;
            if (handler != null)
                handler(new EditorDocChangeEventArgs(doc));
        }

        private CppCodeBrowser.IIndexBuilder GetProjectBuilder(Project.IProject project)
        {
            CppCodeBrowser.IIndexBuilder result = null;
            if (_projectBuilders.TryGetValue(project, out result))
                return result;
            result = new CppCodeBrowser.IndexBuilder();
            _projectBuilders.Add(project, result);
        
            return result;
        }
        
        #endregion
    }
}
