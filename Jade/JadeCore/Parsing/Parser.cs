using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace JadeCore.Parsing
{
    public class Parser : IParser
    {
        private object _lock;
        private ManualResetEvent _stopEvent;
        private AutoResetEvent _workerParseEvent;
        private Thread _workerThread;
        private List<ParseJob> _work;
                        
        public Parser()
        {
            _lock = new object();
            _stopEvent = new ManualResetEvent(false);

            _work = new List<ParseJob>();
            _workerParseEvent = new AutoResetEvent(false);
            _workerThread = new Thread(WorkerThreadLoop);
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

        public void AddJob(ParsePriority priority, ParseJob newJob)
        {
            lock (_lock)
            {
                foreach(var job in _work)
                {
                    if(job.Path == newJob.Path)
                    {
                        if (priority == ParsePriority.Editing) //we want to move it to the front
                        {
                            _work.Remove(job);
                            break;
                        }
                        else
                            return;
                    }                    
                }
                if (priority == ParsePriority.Editing)
                    _work.Insert(0, newJob);
                else
                    _work.Insert(_work.Count, newJob);
                _workerParseEvent.Set();
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

                //Drain the work queue
                while (true)
                {
                    ParseJob job = null;
                    lock (_lock)
                    {
                        if (_work.Count > 0)
                        {
                            job = _work.First();
                            _work.RemoveAt(0);
                        }
                        else
                        {
                            break;
                        }
                    }
                    Parse(job);
                }
            }
        }

        private void Parse(ParseJob job)
        {
            job.Parse();
        }
    }
}
