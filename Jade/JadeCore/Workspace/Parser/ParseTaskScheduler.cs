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
    public class ParseTaskScheduler : TaskScheduler, IDisposable
    {
        private ApartmentState apartmentState;
        private ThreadPriority threadPriority;

        private readonly List<Thread> threads;

        //private BlockingCollection<Task> tasks;
        private IParseTaskQueue _taskQueue;

        /// <summary>
        /// An MTA, BelowNormal TaskScheduler with the appropriate number of threads
        /// </summary>
        public ParseTaskScheduler(int numberOfThreads, IParseTaskQueue taskQueue)
            : this(numberOfThreads, taskQueue, ApartmentState.MTA, ThreadPriority.BelowNormal)
        {
        }

        public ParseTaskScheduler(int numberOfThreads, IParseTaskQueue taskQueue, ApartmentState apartmentState, ThreadPriority threadPriority)
        {
            this._taskQueue = taskQueue;
            this.apartmentState = apartmentState;
            this.threadPriority = threadPriority;

            if (numberOfThreads < 1) throw new ArgumentOutOfRangeException("numberOfThreads");

            threads = Enumerable.Range(0, numberOfThreads).Select(i =>
            {
                var thread = new Thread(() =>
                {
                    FileIndexerTask task;

                    do
                    {
                        task = _taskQueue.DequeueNextTask();
                        if(task != null)
                        {
                            TryExecuteTask(task.Task);
                        }
                        else
                        {
                            _taskQueue.Wait();
                        }
                    } while (true);
                });
                thread.IsBackground = true;
                thread.Priority = this.threadPriority;
                thread.SetApartmentState(this.apartmentState);
                return thread;
            }).ToList();

            threads.ForEach(t => t.Start());
        }

        protected override void QueueTask(Task task)
        {
            Debug.Assert(false);
        }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return null;            
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            // this is used to execute the Task on the thread that is waiting for it - i.e. INLINE
            // it needs to check the Apartment state and any other requirements
            if (Thread.CurrentThread.GetApartmentState() != this.apartmentState) return false;          // can't execute on wrong Appt state
            if (Thread.CurrentThread.Priority != this.threadPriority) return false;                     // can't execute on wrong priority of thread either
            return TryExecuteTask(task);
        }
                
        protected override bool TryDequeue(Task task)
        {
            return false;
        }


        public override int MaximumConcurrencyLevel
        {
            get { return threads.Count; }
        }

        public void Dispose()
        {
            foreach (var thread in threads) thread.Join();
        }        
    }
}
