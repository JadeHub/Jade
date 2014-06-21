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
        private EditorController _editorController;

        public ProjectIndexBuilder(Project.IProject project, EditorController editorController)
        {
            _project = project;
            _editorController = editorController;

            _parserThreads = new ProjectParseThreads(_project, project.IndexBuilder, editorController, delegate(FilePath p) { OnParseComplete(p); });
            _parserThreads.Run = true;
        }

        public void Dispose()
        {
            _parserThreads.Dispose();

        }

        public CppCodeBrowser.IProjectIndex Index { get { return _project.Index; } }

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

        private ActiveDocParseThread _activeDocParseThread;

        private object _lockObject;
                
        #endregion

        #region Constructor

        public EditorController()
        {
            _openDocuments = new Dictionary<FilePath, IEditorDoc>();
            _projectBuilders = new Dictionary<Project.IProject, ProjectIndexBuilder>();
            _lockObject = new object();
            _activeDocParseThread = new ActiveDocParseThread(this);
            _activeDocParseThread.Run = true;
        }

        public void Dispose()
        {
            CloseAllDocuments();
            foreach (var projectBuilder in _projectBuilders.Values)
                projectBuilder.Dispose();
            _projectBuilders.Clear();
            _activeDocParseThread.Run = false;
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
            get 
            {
                lock (_lockObject)
                {
                    return _activeDocument;
                }
            }

            set 
            {
                lock (_lockObject)
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
        }

        public bool HasModifiedDocuments
        {
            get
            {
                lock(_lockObject)
                {
                    foreach(IEditorDoc doc in _openDocuments.Values)
                    {
                        if (doc.Modified)
                            return true;
                    }
                }
                return false;
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
            lock (_lockObject)
            {
                if (_openDocuments.ContainsKey(file.Path) == false)
                {
                    IEditorDoc doc = null;

                    //find project?
                    ITextDocument textDoc = JadeCore.Services.Provider.WorkspaceController.DocumentCache.FindOrAdd(file);

                    Project.IProject p = GetProjectForFile(file.Path);
                    if (p != null)
                        CreateProjectBuilder(p);
                    doc = new SourceDocument(this, textDoc, p);
                    _openDocuments.Add(file.Path, doc);
                    OnDocumentOpened(doc);
                    ActiveDocument = doc;
                }

                //this doc is already open, if it's not the active document, activate it
                else if (ActiveDocument == null || ActiveDocument.File != file)
                {
                    ActiveDocument = _openDocuments[file.Path];
                }
            }
         }

        public void SaveActiveDocument()
        {
            lock (_lockObject)
            {
                if(ActiveDocument != null)
                    ActiveDocument.Save();
            }
        }

        public void CloseActiveDocument()
        {
            lock (_lockObject)
            {
                if (ActiveDocument != null)
                {
                    CloseDocument(ActiveDocument);
                }
            }
        }

        public void DiscardChangesToActiveDocument()
        {
            lock (_lockObject)
            {
                if (ActiveDocument != null)
                {
                    ActiveDocument.TextDocument.DiscardChanges();
                }
            }
        }

        public void CloseAllDocuments()
        {
            lock (_lockObject)
            {
                List<IEditorDoc> docs = new List<IEditorDoc>(_openDocuments.Values);

                foreach (IEditorDoc doc in docs)
                {
                    //todo - raises events while locked
                    CloseDocument(doc);
                }
            }
        }

        private void CloseDocument(IEditorDoc doc)
        {
            lock (_lockObject)
            {
                _openDocuments.Remove(doc.File.Path);
                if (ActiveDocument != null && ActiveDocument.Equals(doc))
                    ActiveDocument = null;
            }
            OnDocumentClosed(doc);
        }

        public void Reset()
        {
            CloseAllDocuments();
        }

        public IList<TextDocumentSnapshot> GetSnapshots()
        {
            lock (_lockObject)
            {
                List<TextDocumentSnapshot> result = new List<TextDocumentSnapshot>();
                foreach (IEditorDoc doc in _openDocuments.Values)
                {
                    if (doc.Modified)
                    {
                        result.Add(doc.TextDocument.CreateSnapshot());
                    }
                }
                return result;
            }
        }

        public IList<Tuple<string, string>> GetUnsavedFiles()
        {
            List<Tuple<string, string>> result = new List<Tuple<string, string>>();

            foreach(TextDocumentSnapshot doc in GetSnapshots())
            {
                result.Add(new Tuple<string, string>(doc.Document.File.Path.Str, doc.Text));
            }

            return result;
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

        private void CreateProjectBuilder(Project.IProject project)
        {
            if(!_projectBuilders.ContainsKey(project))
                _projectBuilders.Add(project, new ProjectIndexBuilder(project, this));
            return;
        }
        
        #endregion
    }
}
