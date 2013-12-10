using System.Collections.Generic;

namespace CppView
{
    using JadeUtils.IO;

    public interface ISourceFileStore
    {
        ISourceFile FindSourceFile(FilePath path);
        ISourceFile AddSourceFile(FilePath path);
        bool RemoveSourceFile(FilePath path);
        IEnumerable<ISourceFile> SourceFiles { get; }

        IHeaderFile FindHeaderFile(FilePath path);
        IHeaderFile FindOrAddHeaderFile(FilePath path);
        IEnumerable<IHeaderFile> HeaderFiles { get; }

        IEnumerable<FilePath> AllFiles { get; }
        bool ContainsFile(FilePath path);
        ICodeFile FindFile(FilePath path);
    }

    public interface IProjectSourceIndex
    {
        IProjectSymbolTable SymbolTable { get; }
        ISourceFileStore FileStore { get; }

        LibClang.Cursor GetCursorAt(FilePath path, int offset);
    }

    public class ProjectSourceIndex : IProjectSourceIndex, ISourceFileStore
    {
        #region Data

        private IProjectSymbolTable _symbolTable;
        private IDictionary<FilePath, ISourceFile> _sourceFiles;
        private Dictionary<FilePath, IHeaderFile> _headerFiles;
        private object _lock;

        #endregion

        #region Constructor

        public ProjectSourceIndex(IProjectSymbolTable symbolTable)
        {
            _symbolTable = symbolTable;
            _sourceFiles = new Dictionary<FilePath, ISourceFile>();
            _headerFiles = new Dictionary<FilePath, IHeaderFile>();
            _lock = new object();
        }

        #endregion

        #region ISourceFileStore implementation

        public ISourceFile FindSourceFile(FilePath path)
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

        public ISourceFile AddSourceFile(JadeUtils.IO.FilePath path)
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

        public bool RemoveSourceFile(JadeUtils.IO.FilePath path)
        {
            lock (_lock)
            {
                return _sourceFiles.Remove(path);
            }
        }

        public IEnumerable<ISourceFile> SourceFiles
        {
            get { return _sourceFiles.Values; }
        }

        public IHeaderFile FindHeaderFile(FilePath path)
        {
            lock (_lock)
            {
                IHeaderFile file;
                if (_headerFiles.TryGetValue(path, out file))
                {
                    return file;
                }
            }
            return null;
        }

        public IHeaderFile FindOrAddHeaderFile(FilePath path)
        {
            lock (_lock)
            {
                IHeaderFile file;
                if (!_headerFiles.TryGetValue(path, out file))
                {
                    file = new HeaderFile(path);
                    _headerFiles.Add(path, file);
                }
                return file;
            }
        }
        
        public IEnumerable<IHeaderFile> HeaderFiles 
        {
            get { return _headerFiles.Values; }
        }

        public IEnumerable<FilePath> AllFiles 
        { 
            get
            {
                foreach(ISourceFile sf in _sourceFiles.Values)
                {
                    yield return sf.Path;
                }
            }
        }

        public bool ContainsFile(FilePath path)
        {
            return _sourceFiles.ContainsKey(path) || _headerFiles.ContainsKey(path);
        }

        public ICodeFile FindFile(FilePath path)
        {
            ICodeFile file = FindSourceFile(path);
            if (file != null) 
                return file;
            return FindHeaderFile(path);
        }

        public LibClang.Cursor GetCursorAt(FilePath path, int offset)
        {
            ICodeFile f = FindFile(path);
            if (f == null)
                return null;
            LibClang.TranslationUnit tu = GetDefaultTuForFile(f);
            if (tu == null)
                return null;

            LibClang.Cursor c = tu.GetCursorAt(path.Str, (uint)offset);
            return c;
        }

        #endregion

        private LibClang.TranslationUnit GetDefaultTuForFile(ICodeFile file)
        {
            if (file is ISourceFile)
            {
                return (file as ISourceFile).TranslationUnit;
            }
            else if (file is IHeaderFile)
            {
                return (file as IHeaderFile).DefaultTranslationUnit;
            }
            return null;
        }

        public IProjectSymbolTable SymbolTable
        {
            get { return _symbolTable; }
        }

        public ISourceFileStore FileStore 
        {
            get { return this; }
        }
        
    }
}
