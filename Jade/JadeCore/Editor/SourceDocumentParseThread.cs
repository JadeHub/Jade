using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JadeUtils.IO;

namespace JadeCore.Editor
{   
    /// <summary>
    /// A c++ source file
    /// </summary>
    internal class ParseFile
    {
        private bool _parsed; //parsed at least once?
        private Project.IFileItem _projectItem;
        private bool _parsing;
        private CppCodeBrowser.IIndexBuilder _indexBuilder;

        public ParseFile(Project.IFileItem projectItem, CppCodeBrowser.IIndexBuilder indexBuilder)
        {
            _parsed = false;
            _parsing = false;
            _projectItem = projectItem;
            _indexBuilder = indexBuilder;
        }

        public FilePath Path { get { return _projectItem.Path; } }

        public bool Parsing
        {
            get { return _parsing; }
            set 
            {
                Debug.Assert(value == true); //only this class may turn this off via _parsing
                _parsing = value; 
            }
        }

        public bool RequiresParse 
        { 
            get 
            {
                return _parsed == false && _parsing == false;
            }
            set
            {
                _parsed = false;
            }
        }

        public void Parse(TaskScheduler guiScheduler)
        {
         //   if (_projectItem.Type == Project.ItemType.CppSourceFile)
            {
                Debug.Assert(Parsing);
                _indexBuilder.ParseFile(Path, null);                
            }
         /*   else if(_projectItem.Type == Project.ItemType.CppHeaderFile)
            {
                CppCodeBrowser.IHeaderFile header = _indexBuilder.Index.FindHeaderFile(Path);
                List<CppCodeBrowser.ISourceFile> sources = new List<CppCodeBrowser.ISourceFile>(header.SourceFiles);
                foreach(CppCodeBrowser.ISourceFile sf in sources)
                {
                    //check bailout flag
                    _indexBuilder.ParseFile(sf.Path, null);
                }                
            }
            */
            _parsed = true;
            _parsing = false;
        }

        protected virtual UInt64 GetVersion()
        {
            return 0;
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

        private Action<FilePath> _onParseComplete;
        private TaskScheduler _guiScheduler;

        private IDictionary<FilePath, ParseFile> _files;
        private IList<ParseFile> _work;
        
        public ProjectParseThreads(Project.IProject project, 
                                    CppCodeBrowser.IIndexBuilder indexBuilder, 
                                    IEditorController controller, 
                                    Action<FilePath> onParseComplete)
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

            //controller.ActiveDocumentChanged += OnActiveDocumentChanged;
            _project.SourceFiles.Observe(OnFileAdded, OnFileRemoved);
        }
       
        public void Dispose()
        {
            if (_disposed) return;
            Run = false;            
            _disposed = true;
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
                        if(pf.RequiresParse)
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

        private void Parse(ParseFile pf)
        {
            pf.Parse(_guiScheduler);            
        }
                
        public bool Run
        {
            set
            {
                if (value)
                {
                    _stopEvent.Reset();
                    _workerThread.Start();
                }
                else
                {
                    _stopEvent.Set();
                    _workerThread.Join();
                }
            }
        }

        private void OnFileAdded(Project.IFileItem file)
        {
            lock(_lock)
            {
                Debug.Assert(_files.ContainsKey(file.Path) == false);
                ParseFile pf = new ParseFile(file, _indexBuilder);
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
