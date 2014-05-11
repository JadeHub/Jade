using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace JadeCore.Editor
{
    internal class SourceDocumentParseThread : IDisposable
    {
        private SourceDocument _doc;
        private bool _disposed;
        private bool _parsed;
        private CppCodeBrowser.IIndexBuilder _indexBuilder;

        private AutoResetEvent _parseEvent;
        private ManualResetEvent _stopEvent;
        private Thread _workerThread;
        private Action<bool> _onParseComplete;
        private TaskScheduler _guiScheduler;

        public SourceDocumentParseThread(SourceDocument doc, CppCodeBrowser.IIndexBuilder indexBuilder, Action<bool> onParseComplete)
        {
            _disposed = false;
            _doc = doc;
            _parsed = false;
            _indexBuilder = indexBuilder;
            _onParseComplete = onParseComplete;

            _guiScheduler = TaskScheduler.FromCurrentSynchronizationContext();

            _parseEvent = new AutoResetEvent(false);
            _stopEvent = new ManualResetEvent(false);
            _doc.TextDocument.TextChanged += OnTextChanged;
            _workerThread = new Thread(ThreadLoop);
        }

        private void OnTextChanged(object sender, EventArgs e)
        {
            _parseEvent.Set();
        }

        public void Dispose()
        {
            if (_disposed) return;
            Run = false;
            _doc.TextDocument.TextChanged -= OnTextChanged;
            _disposed = true;
        }

        private void ThreadLoop()
        {
            WaitHandle[] waitHandles = new WaitHandle[2] { _stopEvent, _parseEvent };
            while (true)
            {
                int wait = WaitHandle.WaitAny(waitHandles);
                if (wait == 0)
                    return;

                if (_parsed == false || _doc.Modified)
                {
                    _indexBuilder.Index.RemoveProjectItem(_doc.File.Path);
                    bool result = _indexBuilder.ParseFile(_doc.File.Path, null);
                    _parsed = true;
                    Task.Factory.StartNew(() => { _onParseComplete(result); },
                                        CancellationToken.None, TaskCreationOptions.None, _guiScheduler);
                }
            }
        }

        public bool Run
        {
            set
            {
                if (value)
                {
                    _workerThread.Start();
                }
                else
                {
                    _stopEvent.Set();
                    _workerThread.Join();
                }
            }
        }

        public bool HighPriority
        {
            set
            {
                _workerThread.Priority = value ? ThreadPriority.Normal : ThreadPriority.BelowNormal;
            }
        }
    }
}
