using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace JadeCore.Workspace.Parser
{
    public class ParseTaskQueue : IParseTaskQueue
    {
        #region Data;

        private LinkedList<ProjectTaskQueue> _projectQueue;
        private object _lock;
        private ManualResetEvent _waitObject;

        #endregion

        #region Constructor

        public ParseTaskQueue()
        {
            _projectQueue = new LinkedList<ProjectTaskQueue>();
            _lock = new object();
            _waitObject = new ManualResetEvent(false);
        }

        #endregion

        #region IParseTaskQueue

        public WaitHandle WaitHandle { get { return _waitObject; } }

        public void QueueProject(ProjectTaskQueue proj)
        {
            lock (_lock)
            {
                if (_projectQueue.Contains(proj) == false && proj.HasWork)
                {
                    _projectQueue.AddLast(proj);
                    _waitObject.Set();
                }                
            }
        }

        public void Prioritise(ProjectTaskQueue proj)
        {
            lock (_lock)
            {
                _projectQueue.Remove(proj);
                _projectQueue.AddFirst(proj);
            }
        }

        public void RemoveProject(ProjectTaskQueue proj)
        {
            lock (_lock)
            {
                _projectQueue.Remove(proj);
            }
        }

        public FileIndexerTask DequeueTask()
        {
            FileIndexerTask result = null;
            lock(_lock)
            {
                if (_projectQueue.Count > 0)
                {
                    result = _projectQueue.First().DequeueWorkItem();
                    if (!_projectQueue.First().HasWork)
                        _projectQueue.RemoveFirst();
                    if (_projectQueue.Count == 0)
                        _waitObject.Reset();
                }
            }
            return result;
        }

        #endregion
        
    }
}
