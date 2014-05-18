using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JadeUtils.IO;

namespace JadeCore.Editor
{   
    internal class ParseFile
    {
        private bool _parsed; //parsed at least once?
        private FilePath _path;
        private bool _parsing;

        public ParseFile(FilePath path)
        {
            _parsed = false;
            _parsing = false;
            _path = path;
        }

        public FilePath Path { get { return _path; } }

        public bool Parsing
        {
            get { return _parsing; }
            set 
            {
                Debug.Assert(value); //only this class may turn this off via _parsing
                _parsing = value; 
            }
        }

        public bool RequiresParse 
        { 
            get 
            {
                return _parsed == false && _parsing == false;
            }
        }

        public void Parse(CppCodeBrowser.IIndexBuilder indexBuilder, TaskScheduler guiScheduler)
        {
            Debug.Assert(Parsing);
            indexBuilder.Index.RemoveProjectItem(Path);
            indexBuilder.ParseFile(Path, null);
            _parsed = true;
            _parsing = false;
        }

        protected virtual UInt64 GetVersion()
        {
            return 0;
        }
    }

    internal class ActiveParseFile : ParseFile
    {
        public ActiveParseFile(IEditorDoc doc)
            :base(doc.File.Path)
        {
            Document = doc;
        }

        public IEditorDoc Document
        {
            get;
            private set;
        }

        protected override UInt64 GetVersion()
        {
            return Document.TextDocument.Version; ;
        }
    }

    internal class ProjectParseThreads : IDisposable
    {
        private bool _disposed;
        private object _lock;
        private CppCodeBrowser.IIndexBuilder _indexBuilder;
        private Project.IProject _project;
                
        private ManualResetEvent _stopEvent;

        private ManualResetEvent _workerParseEvent;
        private Thread _workerThread;

        private AutoResetEvent _activeDocParseEvent;
        private Thread _activeDocThread;

        private Action<FilePath> _onParseComplete;
        private TaskScheduler _guiScheduler;

        private IDictionary<FilePath, ParseFile> _files;
        private IList<ParseFile> _work;
        private ActiveParseFile _activeFile;
        
        public ProjectParseThreads(Project.IProject project, CppCodeBrowser.IIndexBuilder indexBuilder, IEditorController controller, Action<FilePath> onParseComplete)
        {
            _disposed = false;
            _lock = new object();
            _project = project;
            
            _indexBuilder = indexBuilder;
            _onParseComplete = onParseComplete;

            _files = new Dictionary<FilePath, ParseFile>();
            _work = new List<ParseFile>();

            _guiScheduler = JadeCore.Services.Provider.GuiScheduler;

            _stopEvent = new ManualResetEvent(false);

            _workerParseEvent = new ManualResetEvent(false);
            _workerThread = new Thread(WorkerThreadLoop);

            _activeDocParseEvent = new AutoResetEvent(false);
            _activeDocThread = new Thread(ActiveDocThreadLoop);

            controller.ActiveDocumentChanged += OnActiveDocumentChanged;

            _project.SourceFiles.Observe(OnFileAdded, OnFileRemoved);
        }

        private void OnActiveDocumentChanged(IEditorDoc doc, IEditorDoc oldValue)
        {
            lock (_lock)
            {
                if (doc == null) //becoming null
                {
                    if (_activeFile != null)
                    {
                        _activeFile.Document.TextDocument.TextChanged -= ActiveDocumentModified;
                        _activeFile = null;
                    }
                    return;
                }

                if (_activeFile != null && _activeFile.Document == doc) //no change
                    return;

                if (_activeFile != null) //unsubscribe
                {
                    _activeFile.Document.TextDocument.TextChanged -= ActiveDocumentModified;
                    _activeFile = null;
                }
                if (_files.ContainsKey(doc.File.Path))
                {
                    _activeFile = new ActiveParseFile(doc);
                    _activeFile.Document.TextDocument.TextChanged += ActiveDocumentModified;
                    _activeDocParseEvent.Set();
                }                
            }
        }

        private void ActiveDocumentModified(UInt64 version)
        {
            lock (_lock)
            {
                Debug.Assert(_activeFile != null);
                _activeDocParseEvent.Set();
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            Run = false;            
            _disposed = true;
        }

        private void ActiveDocThreadLoop()
        {
            WaitHandle[] waitHandles = new WaitHandle[2] { _stopEvent, _activeDocParseEvent };
            while (true)
            {
                int wait = WaitHandle.WaitAny(waitHandles);
                if (wait == 0)
                    return;

                ParseFile pf = null;
                bool parse = false;
                lock(_lock)
                {
                    pf = _activeFile;
                    if(pf != null && pf.RequiresParse)
                    {
                        parse = true;
                        pf.Parsing = true;
                    }
                }

                if(parse)
                {
                    Debug.WriteLine("Active doc parsing " + pf.Path.Str);
                    Parse(pf);
                }
            }                
        }

        private void WorkerThreadLoop()
        {
            WaitHandle[] waitHandles = new WaitHandle[2] { _stopEvent, _workerParseEvent };
            while (true)
            {
                int wait = WaitHandle.WaitAny(waitHandles);
                if (wait == 0)
                    return;

                ParseFile work = null;

                lock(_lock)
                {
                    foreach(ParseFile pf in _work)
                    {
                        if(pf.RequiresParse && IsActiveFile(pf) == false)
                        {
                            pf.Parsing = true;
                            work = pf;
                            break;
                        }
                    }
                    if(work == null)
                    {
                        //wait for work
                        _workerParseEvent.Reset();
                    }
                }               

                if (work != null)                
                {
                    Debug.WriteLine("Background parsing " + work.Path.Str);
                    Parse(work);
                }
            }
        }

        private bool IsActiveFile(ParseFile pf)
        {
            lock (_lock)
            {
                return _activeFile != null && _activeFile.Path == pf.Path;
            }
        }

        private void Parse(ParseFile pf)
        {
            pf.Parse(_indexBuilder, _guiScheduler);
            
        }
                
        public bool Run
        {
            set
            {
                if (value)
                {
                    _stopEvent.Reset();
                    _workerThread.Start();
                    _activeDocThread.Start();
                }
                else
                {
                    _stopEvent.Set();
                    _activeDocThread.Join();
                    _workerThread.Join();                    
                }
            }
        }

        private void OnFileAdded(Project.IFileItem file)
        {
            lock(_lock)
            {
                Debug.Assert(_files.ContainsKey(file.Path) == false);
                ParseFile pf = new ParseFile(file.Path);
                _files.Add(file.Path, pf);
                _work.Add(pf);
                _workerParseEvent.Set();
            }            
        }

        private void OnFileRemoved(Project.IFileItem file)
        {
            lock (_lock)
            {
                Debug.Assert(_files.ContainsKey(file.Path));
                ParseFile pf = _files[file.Path];
                _files.Remove(file.Path);
                _work.Remove(pf);
            }
        }
    }    
}
