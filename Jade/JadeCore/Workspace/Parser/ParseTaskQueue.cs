using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadeCore.Workspace.Parser
{
    public class ParseTaskQueue : IParseTaskQueue
    {
        #region Data;

        private ProjectTaskQueue _highPriorityProject;
        private FileIndexerTask _highPriorityItem;
        private LinkedList<ProjectTaskQueue> _lowPriorityQueue;
        private object _lock;
        private System.Threading.AutoResetEvent _waitEvent;

        #endregion

        #region Constructor

        public ParseTaskQueue()
        {
            _lowPriorityQueue = new LinkedList<ProjectTaskQueue>();
            _lock = new object();
            _waitEvent = new System.Threading.AutoResetEvent(false);
        }

        #endregion

        #region IParseTaskQueue

        public void QueueProject(ProjectTaskQueue proj)
        {
            lock (_lock)
            {
                if (_lowPriorityQueue.Contains(proj) == false)
                    _lowPriorityQueue.AddLast(proj);
                _waitEvent.Set();
            }
        }

        public void Prioritise(ProjectTaskQueue proj)//, FileIndexerTask file)
        {
            /*lock (_lock)
            {
                _highPriorityProject = proj;
                _highPriorityItem = file;
                _waitEvent.Set();
            }*/
        }

        public FileIndexerTask DequeueNextTask()
        {
            return DequeueTask();
        }

        public void RemoveProject(ProjectTaskQueue proj)
        {
            lock (_lock)
            {
                if (proj == _highPriorityProject)
                {
                    _highPriorityProject = null;
                }
                _lowPriorityQueue.Remove(proj);
            }
        }

        public void Wait()
        {
            _waitEvent.WaitOne();
        }

        #endregion

        #region Private Methods

        private FileIndexerTask DequeueTaskFromProject(ProjectTaskQueue proj)
        {
            return proj.DequeueWorkItem();
        }

        private FileIndexerTask DequeueTask()
        {
            FileIndexerTask result = null;

            lock (_lock)
            {
                // There are three cases:
                // _highPriorityItem is set - we need immediate processing of this file
                // _highPriorityProject is set but _highPriorityItem is null - user is viewing an indexed file from this project - keep working on other files in the same project
                //neither _highPriorityItem or _highPriorityProject are set - continue background procesing of remaining projects

                if (_highPriorityItem != null)
                {
                    result = _highPriorityItem;
                    _highPriorityItem = null;
                }
                else if (_highPriorityProject != null)
                {
                    //This can return null in which case we fall through to processing the lowPriorityQueue
                    result = DequeueTaskFromProject(_highPriorityProject);

                    if (!_highPriorityProject.HasWork)
                        _highPriorityProject = null;
                }

                while (result == null && _lowPriorityQueue.Count > 0)
                {
                    ProjectTaskQueue p = _lowPriorityQueue.First();
                    //This can return null in which case we remove the project and keep going
                    result = DequeueTaskFromProject(p);

                    if (!p.HasWork)
                    {
                        _lowPriorityQueue.RemoveFirst();
                    }
                }
            }
            return result;
        }

        #endregion
    }
}
