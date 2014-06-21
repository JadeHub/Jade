using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JadeUtils.IO;

namespace JadeCore.Editor
{    
    internal class ActiveParseFile// : ParseFile
    {
        private bool _parsed; //parsed at least once?
        private Project.IFileItem _projectItem;
        private bool _parsing;
        private CppCodeBrowser.IIndexBuilder _indexBuilder;
        private ActiveDocParseThread _parseThread;
        
        public ActiveParseFile(ActiveDocParseThread parseThread, IEditorDoc doc, Project.IFileItem projectItem)
        {
            _parseThread = parseThread;
            _parsed = false;
            _projectItem = projectItem;
            _parsing = false;
            _indexBuilder = doc.Project.IndexBuilder;
            doc.TextDocument.TextChanged += TextDocumentChanged;
        }

        private void TextDocumentChanged(ulong version)
        {
            RequiresParse = true;

            _parseThread.WakeUp();
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

        public void Parse()
        {
            Debug.Assert(Parsing);
            if (_projectItem.Type == Project.ItemType.CppSourceFile)
            {                
                _indexBuilder.ParseFile(Path, null);
            }
            else if(_projectItem.Type == Project.ItemType.CppHeaderFile)
            {
                CppCodeBrowser.IHeaderFile header = _indexBuilder.Index.FindHeaderFile(Path);
                List<CppCodeBrowser.ISourceFile> sources = new List<CppCodeBrowser.ISourceFile>(header.SourceFiles);
                if(sources.Count > 0)
                {
                    _indexBuilder.ParseFile(sources[0].Path, null);
                }
            }
            _parsed = true;
            _parsing = false;
        }

    }

    public class ActiveDocParseThread
    {
        private bool _disposed;
        private object _lock;
        
        private ManualResetEvent _stopEvent;

        private AutoResetEvent _activeDocParseEvent;
        private Thread _activeDocThread;
        private IDictionary<FilePath, ParseFile> _files;
        
        private ActiveParseFile _activeFile;

        public ActiveDocParseThread(IEditorController controller)
        {
            _disposed = false;
            _lock = new object();

            _files = new Dictionary<FilePath, ParseFile>();        
            _stopEvent = new ManualResetEvent(false);                        
            _activeDocParseEvent = new AutoResetEvent(false);
            _activeDocThread = new Thread(ActiveDocThreadLoop);
            controller.ActiveDocumentChanged += OnActiveDocumentChanged;            
        }

        private void OnActiveDocumentChanged(IEditorDoc doc, IEditorDoc oldValue)
        {
            lock (_lock)
            {
                if (doc == null) //becoming null
                {
                    if (_activeFile != null)
                    {
                        _activeFile = null;
                    }
                    return;
                }

                _activeFile = null;
               
                //if (_files.ContainsKey(doc.File.Path))
              //  if(doc.File.Path.Extention != ".h")
                {
                    Project.IFileItem projectItem = doc.Project.FindFileItem(doc.File.Path);
                    if(projectItem == null) return;

                    _activeFile = new ActiveParseFile(this, doc, projectItem);
                    _activeDocParseEvent.Set();
                }                
            }
        }

        public void WakeUp()
        {
            _activeDocParseEvent.Set();
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

                ActiveParseFile pf = null;
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

        private bool IsActiveFile(ParseFile pf)
        {
            lock (_lock)
            {
                return _activeFile != null && _activeFile.Path == pf.Path;
            }
        }

        private void Parse(ActiveParseFile pf)
        {            
            pf.Parse();
        }

        public bool Run
        {
            set
            {
                if (value)// && _activeDocThread.ThreadState == System.Threading.ThreadState.Stopped)
                {
                    _stopEvent.Reset();
                    _activeDocThread.Start();
                }
                else
                {
                    _stopEvent.Set();                  
                }
            }
        }
    }
}
