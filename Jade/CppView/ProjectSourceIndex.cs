using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CppView
{
    using JadeUtils.IO;

    public interface ISourceFileStore
    {
        ISourceFile Find(JadeUtils.IO.FilePath path);
        ISourceFile FindOrAdd(JadeUtils.IO.FilePath path);
        bool Remove(JadeUtils.IO.FilePath path);
    }

    public interface IProjectSourceIndex : ISourceFileStore
    {
        IProjectSymbolTable SymbolTable { get; }
    }

    public class ProjectSourceIndex : IProjectSourceIndex
    {
        #region Data

        private IProjectSymbolTable _symbolTable;
        private IDictionary<FilePath, ISourceFile> _sourceFiles;
        private object _lock;

        #endregion

        #region Constructor

        public ProjectSourceIndex(IProjectSymbolTable symbolTable)
        {
            _symbolTable = symbolTable;
            _sourceFiles = new Dictionary<FilePath, ISourceFile>();
            _lock = new object();
        }

        #endregion

        #region ISourceFileStore implementation

        public ISourceFile Find(FilePath path)
        {
            lock (_lock)
            {
                ISourceFile file;
                if (_sourceFiles.TryGetValue(path, out file))
                {
                    return file;
                }
            }
            return null;
        }

        public ISourceFile FindOrAdd(JadeUtils.IO.FilePath path)
        {
            lock (_lock)
            {
                ISourceFile file;
                if (!_sourceFiles.TryGetValue(path, out file))
                {
                    file = new SourceFile(path);
                    _sourceFiles.Add(path, file);
                }
                return file;
            }
        }

        public bool Remove(JadeUtils.IO.FilePath path)
        {
            lock (_lock)
            {
                return _sourceFiles.Remove(path);
            }
        }

        #endregion

        public IProjectSymbolTable SymbolTable
        {
            get { return _symbolTable; }
        }
        
    }
}
