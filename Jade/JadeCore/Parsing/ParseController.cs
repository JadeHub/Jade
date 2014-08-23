using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JadeUtils.IO;

namespace JadeCore.Parsing
{
    public class ParseController : IDisposable
    {
        private Parser _parser;

        //private HashSet<FilePath> _paths;

        public ParseController()
        {
            _parser = new Parser();
            _parser.Run = true;
            Services.Provider.WorkspaceController.WorkspaceChanged += OnWorkspaceChanged;
        }

        public void Dispose()
        {
            if (_parser == null) return;
            _parser.Run = false;
            _parser = null;
        }

        private void OnWorkspaceChanged(Workspace.WorkspaceChangeOperation op)
        {
            if (op != Workspace.WorkspaceChangeOperation.Opened || Services.Provider.WorkspaceController.CurrentWorkspace == null) return;

            foreach(JadeCore.Project.IProject proj in Services.Provider.WorkspaceController.CurrentWorkspace.AllProjects)
            {
                foreach(FilePath path in proj.Files)
                {
                    if (path.Extention.ToLower() == ".cpp" || path.Extention.ToLower() == ".c" || path.Extention.ToLower() == ".cc")
                        _parser.AddJob(ParsePriority.Background, new ParseJob(path, null, proj.IndexBuilder));
                }
            }
        }

        public void AddJob(ParsePriority priority, ParseJob newJob)
        {
            _parser.AddJob(priority, newJob);
        }


    }
}
