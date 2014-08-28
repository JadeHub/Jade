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
        private object _lock;
        private ParseThreads _parseThreads;
        private IUnsavedFileProvider _unsavedFileProvider;

        public event ParseEventHandler TranslationUnitParsed;
        public event ParseEventHandler TranslationUnitIndexed;

        public Dictionary<FilePath, UInt64> _parsedVersions;
        
        public ParseService(IUnsavedFileProvider unsavedProvider)
        {
            _lock = new object();
            _unsavedFileProvider = unsavedProvider;
            _parseThreads = new ParseThreads();
            _parsedVersions = new Dictionary<FilePath, ulong>();
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
                        _parseThreads.AddJob(ParsePriority.Background, new ParseJob(path, 0, null, proj.Index));
                }
            }
        }

        public void AddJob(ParsePriority priority, ParseJob newJob)
        {
            //check if we have this version
            lock (_lock)
            {
                if (_parsedVersions.ContainsKey(newJob.Path))
                {
                    if (newJob.DocumentVersion <= _parsedVersions[newJob.Path])
                        return;
                }
            }

            _parseThreads.AddJob(priority, newJob);
        }

        public void OnParseComplete(ParseResult result)
        {
            lock(_lock)
            {
                if(_parsedVersions.ContainsKey(result.Path))
                {
                    _parsedVersions[result.Path] = result.GetFileVersion(result.Path);
                }
                else
                {
                    _parsedVersions.Add(result.Path, result.GetFileVersion(result.Path));
                }
            }

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
