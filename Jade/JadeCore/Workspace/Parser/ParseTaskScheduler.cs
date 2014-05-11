using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace JadeCore.Workspace.Parser
{
    public class ParseTaskScheduler : IDisposable
    {
        private readonly List<Thread> _threads;
        private IParseTaskQueue _taskQueue;
        private ManualResetEvent _exitEvent;

        public ParseTaskScheduler(int numberOfThreads, IParseTaskQueue taskQueue, ThreadPriority threadPriority, ApartmentState apartmentState = ApartmentState.MTA)
        {
            _taskQueue = taskQueue;
            _exitEvent = new ManualResetEvent(false);

            if (numberOfThreads < 1) throw new ArgumentOutOfRangeException("numberOfThreads");
            
            _threads = new List<Thread>();
            for (int i = 0; i < numberOfThreads; i++)
            {
                Thread t = new Thread(() => ThreadLoop());
                t.IsBackground = true;
                t.Priority = threadPriority;
                t.SetApartmentState(apartmentState);
                _threads.Add(t);
            }
            _threads.ForEach(t => t.Start());
        }

        public void Dispose()
        {
            _exitEvent.Set();
            foreach (var thread in _threads) thread.Join();
        }

        private void ThreadLoop()
        {
          //  try
            {
                WaitHandle[] waitHandles = new WaitHandle[2] { _exitEvent, _taskQueue.WaitHandle };
                while (true)
                {
                    int wait = WaitHandle.WaitAny(waitHandles);
                    if (wait == 0)
                        return;

                    FileIndexerTask task = _taskQueue.DequeueTask();

                    if (task != null)
                    {
                        task.Parse();
                    }
                }
            }
           /* catch(Exception e)
            {
                int i = 0;
            }*/
        }

        public void Go()
        {

            while (true)
            {
                FileIndexerTask task = _taskQueue.DequeueTask();

                if (task == null)
                    break;
                task.Parse();
            }
        }
    }
}
