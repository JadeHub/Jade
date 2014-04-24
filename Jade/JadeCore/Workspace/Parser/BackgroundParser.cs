using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JadeUtils.IO;

namespace JadeCore.Workspace.Parser
{   
    public class FileIndexerTask 
    {
        private Project.IFileItem _file;
        private CppCodeBrowser.IIndexBuilder _indexBuilder;
        private Task _task;
        
        public FileIndexerTask(Project.IFileItem file, CppCodeBrowser.IIndexBuilder indexBuilder, TaskFactory taskFactory)
        {
            _file = file;
            _indexBuilder = indexBuilder;
            CreateTask(taskFactory);
        }

        #region IIndexer

        public Task Task
        {
            get
            {
                return _task;
            }
        }

        public FilePath Path { get { return _file.Path; } }
        public Project.IFileItem File { get { return _file; } }

        public void Start()
        {
            Task t = Task.Factory.StartNew(() => Parse());
            t.ContinueWith(task => OnParseComplete(task), TaskScheduler.FromCurrentSynchronizationContext());
        }

        public bool NeedsParse
        {
            get
            {
                //lock
                return false;
            }
        }

        private void CreateTask(TaskFactory taskFactory)
        {
            Debug.Assert(_task == null);
            
            _task = taskFactory.StartNew(() => Parse());
            _task.ContinueWith(task => OnParseComplete(task), TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void OnParseComplete(Task t)
        { 
        }

        private void Parse()
        {
            _indexBuilder.Index.RemoveProjectItem(Path);
            _indexBuilder.AddFile(Path, null);
        }



        #endregion
    }

    public class ProjectTaskQueue
    {
        private Project.IProject _project;
        private IDictionary<Project.IFileItem, FileIndexerTask> _files;

        //accesed from multiple threads
        private LinkedList<FileIndexerTask> _workList;
 
        private TaskFactory _taskFactory;

        private CppCodeBrowser.IIndexBuilder _indexBuilder;

        public ProjectTaskQueue(Project.IProject project, TaskFactory taskFactory)
        {
            _project = project;
            _taskFactory = taskFactory;

            _files = new Dictionary<Project.IFileItem, FileIndexerTask>();
            _workList = new LinkedList<FileIndexerTask>();

            _indexBuilder = new CppCodeBrowser.IndexBuilder();
            _project.SourceFiles.Observe(OnFileAdded, OnFileRemoved);
        }

        public void Prioritise(Project.IFileItem file)
        {
           lock(_workList)
           {
               FileIndexerTask task;
               if(_files.TryGetValue(file, out task) == false)
                   return;

               _workList.Remove(task);
               _workList.AddFirst(task);
           }
        }

        public bool HasWork
        {
            get { return _workList.Count > 0; }
        }

        private void OnFileAdded(Project.IFileItem file)
        {
            ITextDocument document = JadeCore.Services.Provider.WorkspaceController.DocumentCache.FindOrAdd(file.Handle);
            if (document == null)
                throw new Exception("Doc not in cache");
            FileIndexerTask indexer = new FileIndexerTask(file, _indexBuilder, _taskFactory);
            document.ModifiedChanged += delegate { OnDocumentModified(indexer, document); };

            _files.Add(file, indexer);
            lock(_workList)
            {
                _workList.AddLast(indexer);
            }            
        }

        public FileIndexerTask DequeueWorkItem()
        {
            lock(_workList)
            {
                FileIndexerTask result = null;

                if(_workList.Count > 0)
                {
                    result = _workList.First();
                    _workList.RemoveFirst();
                }
                return result;
            }
        }

        private void OnDocumentModified(FileIndexerTask indexer, ITextDocument doc)
        {
            Debug.Assert(_files.ContainsKey(indexer.File));

            lock (_workList)
            {
                if (_workList.Contains(indexer) == false)
                {
                    _workList.AddLast(indexer);
                }
                else
                {
                    //if(indexer.IsRunning)...restart
                }
            }
        }

        private void OnFileRemoved(Project.IFileItem file)
        { 
        }
    }

    public interface IWorkspaceIndexer 
    {
        void SetActiveSource(Project.IProject project, Project.IFileItem file);
    }

    public class WorkspaceIndexer : IWorkspaceIndexer
    {
        private IWorkspaceController _controller;
        private IWorkspace _workspace;
        private IDictionary<Project.IProject, ProjectTaskQueue> _projects;

        private Parser.IParseTaskQueue _parserTaskQueue;
        private TaskFactory _taskFactory;
        
        public WorkspaceIndexer(IWorkspaceController controller)
        {
            _controller = controller;
            _controller.WorkspaceChanged += OnControllerWorkspaceChanged;
            _projects = new Dictionary<Project.IProject, ProjectTaskQueue>();

            _parserTaskQueue = new Parser.ParseTaskQueue();
            _taskFactory = new TaskFactory(new Parser.ParseTaskScheduler(4, _parserTaskQueue));
        }

        public void SetActiveSource(Project.IProject project, Project.IFileItem file)
        {
            ProjectTaskQueue projQueue = FindProjectTaskQueue(project);
            Debug.Assert(projQueue != null);

            projQueue.Prioritise(file);
        }
        
        #region Workspace and Project tracking

        private void OnControllerWorkspaceChanged(WorkspaceChangeOperation op)
        {
            if(op == WorkspaceChangeOperation.Created || op == WorkspaceChangeOperation.Opened)
            {
                Debug.Assert(_controller.CurrentWorkspace != null);

                //create list of projects
                Workspace = _controller.CurrentWorkspace;
            }
            else if(op == WorkspaceChangeOperation.Closed)
            {
                Debug.Assert(_controller.CurrentWorkspace == null);
                Workspace = _controller.CurrentWorkspace;
            }
        }

        private IWorkspace Workspace
        {
            get
            {
                return _workspace; 
            }
            set
            {
                if(_workspace != null)
                {
                    _workspace.AllProjects.ItemAdded -= OnProjectAddedToWorkspace;
                    _workspace.AllProjects.ItemRemoved -= OnProjectRemovedFromWorkspace;
                    _projects.Clear();
                }
                _workspace = value;

                if (_workspace != null)
                {
                    _workspace.AllProjects.ItemAdded += OnProjectAddedToWorkspace;
                    _workspace.AllProjects.ItemRemoved += OnProjectRemovedFromWorkspace;
                    
                    foreach(Project.IProject p in _workspace.AllProjects)
                    {
                        OnProjectAddedToWorkspace(p);
                    }
                }
            }
        }

        private void OnProjectAddedToWorkspace(Project.IProject p)
        {
            ProjectTaskQueue indexer = new ProjectTaskQueue(p, _taskFactory);
            _projects.Add(p, indexer);
            _parserTaskQueue.QueueProject(indexer);
        }

        private void OnProjectRemovedFromWorkspace(Project.IProject p)
        {
            _projects.Remove(p);
            //remove from task queue
        }

        private ProjectTaskQueue FindProjectTaskQueue(Project.IProject p)
        {
            ProjectTaskQueue result;
            _projects.TryGetValue(p, out result);
            return result;
        }

        #endregion
    }
}
