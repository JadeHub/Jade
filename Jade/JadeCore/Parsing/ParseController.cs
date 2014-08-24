using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CppCodeBrowser;
using JadeUtils.IO;

namespace JadeCore.Parsing
{
    public delegate void ParseEventHandler(ParseResult result);

    public interface IParseService
    {
        event ParseEventHandler TranslationUnitParsed;
        event ParseEventHandler TranslationUnitIndexed;
    }

    public class ParseService : IDisposable, IParseService
    {
        private ParseThreads _parseThreads;
        private IUnsavedFileProvider _unsavedFileProvider;

        public event ParseEventHandler TranslationUnitParsed;
        public event ParseEventHandler TranslationUnitIndexed;
        
        public ParseService(IUnsavedFileProvider unsavedProvider)
        {
            _unsavedFileProvider = unsavedProvider;
            _parseThreads = new ParseThreads();
            _parseThreads.Run = true;
            Services.Provider.WorkspaceController.WorkspaceChanged += OnWorkspaceChanged;
        }

        public void Dispose()
        {
            if (_parseThreads == null) return;
            _parseThreads.Run = false;
            _parseThreads = null;
        }

        private void OnWorkspaceChanged(Workspace.WorkspaceChangeOperation op)
        {
            if (op != Workspace.WorkspaceChangeOperation.Opened || Services.Provider.WorkspaceController.CurrentWorkspace == null) return;
                        
            foreach(JadeCore.Project.IProject proj in Services.Provider.WorkspaceController.CurrentWorkspace.AllProjects)
            {
                foreach(FilePath path in proj.Files)
                {
                    if (path.Extention.ToLower() == ".cpp" || path.Extention.ToLower() == ".c" || path.Extention.ToLower() == ".cc")
                        _parseThreads.AddJob(ParsePriority.Background, new ParseJob(path, null, proj.Index));
                }
            }
        }

        public void AddJob(ParsePriority priority, ParseJob newJob)
        {
            _parseThreads.AddJob(priority, newJob);
        }

        public void OnParseComplete(ParseResult result)
        {
            Task.Factory.StartNew(() =>
            {
                //    lock (_lock)
                {
                    ParseEventHandler handler = TranslationUnitParsed;
                    if (handler != null)
                    {
                        handler(result);
                    }

                    //begin indexing
                    ProjectIndexBuilder.IndexTranslationUnit(result);
                    OnIndexingComplete(result);

                }
            }, CancellationToken.None, TaskCreationOptions.None, JadeCore.Services.Provider.GuiScheduler);

            
        }

        public void OnIndexingComplete(ParseResult result)
        {
            ParseEventHandler handler = TranslationUnitIndexed;
            if (handler != null)
            {
                handler(result);
            }
        }
    }
}
