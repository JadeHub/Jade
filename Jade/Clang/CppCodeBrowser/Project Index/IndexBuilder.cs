using JadeUtils.IO;
using LibClang;
using System;
using System.Collections.Generic;

namespace CppCodeBrowser
{       
    /// <summary>
    /// This is a simple synchronous implementation.
    /// </summary>
    public class IndexBuilder : IIndexBuilder
    {
        private bool _disposed = false;
        private readonly ProjectIndex _index;
        private readonly LibClang.Index _libClangIndex;

        //Set of all parsed Tus for Disposal
        private readonly HashSet<TranslationUnit> _allTus;

        private object _lock = new object();

        public event ItemIndexedEvent ItemIndexed;
        public event ItemIndexingFailedEvent ItemIndexingFailed;

        public IndexBuilder()
        {
            _index = new ProjectIndex();
            _libClangIndex = new LibClang.Index(false, true);
            _allTus = new HashSet<TranslationUnit>();
        }

        public void Dispose()
        {
            if(_disposed) return;

            foreach (TranslationUnit tu in _allTus)
            {
                tu.Dispose();
            }
            _allTus.Clear();
            _disposed = true;
        }

        public bool ParseFile(FilePath path, string[] compilerArgs)
        {
            if (_disposed) return false;

            lock (_lock)
            {
                LibClang.TranslationUnit tu = new LibClang.TranslationUnit(_libClangIndex, path.Str);

                if (tu.Parse(compilerArgs) == false)
                {
                    tu.Dispose();
                    return false;
                }
                _allTus.Add(tu);
                _index.AddSourceFile(path, tu);
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

        private void RaiseItemIndexedEvent(IProjectItem item)
        {
            if (_disposed) return;
            ItemIndexedEvent handler = ItemIndexed;
            if (handler != null)
                handler(new ItemIndexedEventArgs(item));
        }

        private void RaiseItemIndexingFailedEvent(FilePath path)
        {
            if (_disposed) return;
            ItemIndexingFailedEvent handler = ItemIndexingFailed;
            if (handler != null)
                handler(new ItemIndexingFailedEventArgs(path));
        }
    }
}
