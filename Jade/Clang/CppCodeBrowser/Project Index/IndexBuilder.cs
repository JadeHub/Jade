    using JadeUtils.IO;
using LibClang;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CppCodeBrowser
{       
    /// <summary>
    /// This is a simple synchronous implementation.
    /// </summary>
    public class IndexBuilder : IIndexBuilder
    {
        private bool _disposed = false;
        private readonly ProjectIndex _index;

        //Set of all parsed Tus for Disposal
        private readonly HashSet<TranslationUnit> _allTus;

        private TaskScheduler _callbackSCheduler;

        private object _lock = new object();

        public IndexBuilder(TaskScheduler callbackSCheduler)
        {
            _callbackSCheduler = callbackSCheduler;
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

                if (tu.Parse(compilerArgs) == false)
                {
                    tu.Dispose();
                    return false;
                }
                _allTus.Add(tu);
                if(_index.UpdateSourceFile(path, tu))
                {
                    Task.Factory.StartNew(() => { _index.RaiseItemUpdatedEvent(path); },
                                        CancellationToken.None, TaskCreationOptions.None, _callbackSCheduler);
                }
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
