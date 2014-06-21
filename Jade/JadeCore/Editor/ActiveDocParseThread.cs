using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JadeUtils.IO;

namespace JadeCore.Editor
{
    internal class ActiveParseFile : ParseFile
    {
        public ActiveParseFile(IEditorDoc doc)
            : base(doc.File.Path, doc.Project.IndexBuilder)
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

    public class ActiveDocParseThread
    {
        private bool _disposed;
        private object _lock;
        
        private ManualResetEvent _stopEvent;

        private AutoResetEvent _activeDocParseEvent;
        private Thread _activeDocThread;

        private TaskScheduler _guiScheduler;

        private IDictionary<FilePath, ParseFile> _files;
        
        private ActiveParseFile _activeFile;

        public ActiveDocParseThread(IEditorController controller)
        {
            _disposed = false;
            _lock = new object();

            _files = new Dictionary<FilePath, ParseFile>();        
            _guiScheduler = JadeCore.Services.Provider.GuiScheduler;
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
                //if (_files.ContainsKey(doc.File.Path))
              //  if(doc.File.Path.Extention != ".h")
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
                if (_activeFile != null)
                {
                    _activeFile.RequiresParse = true;
                    _activeDocParseEvent.Set();
                }
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

        private bool IsActiveFile(ParseFile pf)
        {
            lock (_lock)
            {
                return _activeFile != null && _activeFile.Path == pf.Path;
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
