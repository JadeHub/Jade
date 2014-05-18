using System;
using JadeUtils.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace JadeCore.Editor
{
    public class ProjectIndexBuilder : IDisposable
    {
        private Project.IProject _project;
        private ProjectParseThreads _parserThreads;
        private CppCodeBrowser.IIndexBuilder _indexBuilder;
        private EditorController _editorController;

        public ProjectIndexBuilder(Project.IProject project, EditorController editorController)
        {
            _project = project;
            _editorController = editorController;
            _indexBuilder = project.IndexBuilder;

            _parserThreads = new ProjectParseThreads(_project, _indexBuilder, editorController, delegate(FilePath p) { OnParseComplete(p); });
            _parserThreads.Run = true;
        }

        public void Dispose()
        {
            _parserThreads.Dispose();
        }

        public CppCodeBrowser.IProjectIndex Index { get { return _indexBuilder.Index; } }

        private void OnParseComplete(FilePath path)
        {
            
        }
    }

    public class EditorController : JadeCore.IEditorController
    {
        #region Data

        private Dictionary<FilePath, IEditorDoc> _openDocuments;
        private IEditorDoc _activeDocument;
        private IDictionary<Project.IProject, ProjectIndexBuilder> _projectBuilders;
                
        #endregion

        #region Constructor

        public EditorController()
        {
            _openDocuments = new Dictionary<FilePath, IEditorDoc>();
            _projectBuilders = new Dictionary<Project.IProject, ProjectIndexBuilder>();
        }

        public void Dispose()
        {
            CloseAllDocuments();
            foreach (var projectBuilder in _projectBuilders.Values)
                projectBuilder.Dispose();
            _projectBuilders.Clear();
        }

        #endregion

        #region Events

        public event ActiveDocumentChangeEvent ActiveDocumentChanged;
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
                    JadeCore.IEditorDoc oldValue = _activeDocument;
                    //set view model current document
                    _activeDocument = value;
                    OnDocumentSelect(_activeDocument, oldValue);
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
                IEditorDoc doc;

                //find project?
                ITextDocument textDoc = JadeCore.Services.Provider.WorkspaceController.DocumentCache.FindOrAdd(file);
                
                Project.IProject p = GetProjectForFile(file.Path);
                CppCodeBrowser.IProjectIndex index = null;
                if (p != null)
                {
                    CreateProjectBuilder(p);
                    index = p.IndexBuilder.Index;
                }
                doc = new SourceDocument(this, textDoc, index);
                _openDocuments.Add(file.Path, doc);
                OnDocumentOpened(doc);
                ActiveDocument = doc;        
            }
            //this doc is already open, if it's not the active document, activate it
            else if(ActiveDocument == null || ActiveDocument.File != file)
            {
                ActiveDocument = _openDocuments[file.Path];
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
            _openDocuments.Remove(doc.File.Path);
            if (ActiveDocument != null && ActiveDocument.Equals(doc))
                ActiveDocument = null;
            doc.Close();
            OnDocumentClosed(doc);
        }

        public void Reset()
        {
            CloseAllDocuments();
        }

        #endregion

        #region Private Methods

        private Project.IProject GetProjectForFile(FilePath path)
        {
            if (Services.Provider.WorkspaceController.CurrentWorkspace != null)
            {
                return Services.Provider.WorkspaceController.CurrentWorkspace.FindProjectForFile(path);
            }
            return null;
        }

        private void OnDocumentClosed(IEditorDoc doc)
        {
            RaiseDocEvent(DocumentClosed, doc);
        }

        private void OnDocumentOpened(IEditorDoc doc)
        {
            RaiseDocEvent(DocumentOpened, doc);
        }
                
        private void OnDocumentSelect(IEditorDoc newValue, IEditorDoc oldValue)
        {
            ActiveDocumentChangeEvent handler = ActiveDocumentChanged;
            if(handler != null)
            {
                handler(newValue, oldValue);
            }
        }

        private void RaiseDocEvent(EditorDocChangeEvent ev, IEditorDoc doc)
        {
            EditorDocChangeEvent handler = ev;
            if (handler != null)
                handler(new EditorDocChangeEventArgs(doc));
        }

        private ProjectIndexBuilder CreateProjectBuilder(Project.IProject project)
        {
            ProjectIndexBuilder result = null;
            if (_projectBuilders.TryGetValue(project, out result))
                return result;
            result = new ProjectIndexBuilder(project, this);
            _projectBuilders.Add(project, result);        
            return result;
        }
        
        #endregion
    }
}
