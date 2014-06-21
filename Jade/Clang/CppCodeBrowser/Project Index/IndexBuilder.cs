    using JadeUtils.IO;
using LibClang;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CppCodeBrowser
{   
    public interface IUnsavedFileProvider
    {
       /// <summary>
       /// Return a list of Tuples containing path, content
       /// </summary>
       /// <returns></returns>
        IList<Tuple<string, string>> GetUnsavedFiles();
    }

    public class ProjectIndexBuilder : IIndexBuilder
    {
        private bool _disposed = false;
        private readonly ProjectIndex _index;

        //Set of all parsed Tus for Disposal
        private readonly HashSet<TranslationUnit> _allTus;

        private TaskScheduler _callbackScheduler;
        private IUnsavedFileProvider _unsavedFilesProvider;
        private object _lock = new object();

        public ProjectIndexBuilder(TaskScheduler callbackSCheduler, IUnsavedFileProvider unsavedFiles)
        {
            _callbackScheduler = callbackSCheduler;
            _unsavedFilesProvider = unsavedFiles;
            _index = new ProjectIndex();
            _allTus = new HashSet<TranslationUnit>();
        }

        public void Dispose()
        {
            if(_disposed) return;

            lock (_lock)
            {
                foreach (TranslationUnit tu in _allTus)
                {
                    tu.Dispose();
                }
                _allTus.Clear();
                _disposed = true;
            }
        }

        public bool ParseFile(FilePath path, string[] compilerArgs)
        {
            if (_disposed) return false;

            lock (_lock)
            {
                
                LibClang.TranslationUnit tu = new LibClang.TranslationUnit(_index.LibClangIndex, path.Str);

                //pass in unsaved files
                if (tu.Parse(compilerArgs, _unsavedFilesProvider.GetUnsavedFiles()) == false)
                {
                    tu.Dispose();
                    return false;
                }
                  
                //_allTus.Add(tu);
                Task.Factory.StartNew(() => 
                    {
                        lock (_lock)
                        {
                            if (_index.UpdateSourceFile(path, tu))
                            {
                                _index.RaiseItemUpdatedEvent(path);
                            }
                        }
                    }, CancellationToken.None, TaskCreationOptions.None, _callbackScheduler);
                
            }
            return true;
        }

        public IProjectIndex Index 
        { 
            get
            {
                if (_disposed) return null;
                return _index; 
            }
        }
    }
}
