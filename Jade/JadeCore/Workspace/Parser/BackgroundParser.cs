using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadeCore.Workspace
{
    public class WorkspaceIndexer
    {
        private IWorkspaceController _controller;
        private IWorkspace _workspace;

        public WorkspaceIndexer()
        {
            _controller = Services.Provider.WorkspaceController;
            _controller.WorkspaceChanged += OnControllerWorkspaceChanged;
        }

        private void OnControllerWorkspaceChanged(WorkspaceChangeOperation op)
        {
            if(op == WorkspaceChangeOperation.Created || op == WorkspaceChangeOperation.Opened)
            {
                Debug.Assert(_controller.CurrentWorkspace != null);

                //create list of projects
                _workspace = _controller.CurrentWorkspace;
            }
            else if(op == WorkspaceChangeOperation.Closed)
            {
                Debug.Assert(_controller.CurrentWorkspace == null);

                _workspace = _controller.CurrentWorkspace;
            }
        }

        private IWorkspace Workspace
        {
            get { return _controller.CurrentWorkspace; }
        }

        private void Reset()
        {
            
        }
    }
}
