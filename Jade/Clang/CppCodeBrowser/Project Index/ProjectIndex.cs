using JadeUtils.IO;
using LibClang;
using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;

namespace CppCodeBrowser
{   
    public delegate void ProjectItemEvent(FilePath path);

    public interface IProjectIndex
    {
        event ProjectItemEvent ItemUpdated;
        
        IProjectFile FindProjectItem(FilePath path);
        ISourceFile FindSourceFile(FilePath path);
        IHeaderFile FindHeaderFile(FilePath path);
        void UpdateSourceFile(FilePath path, LibClang.TranslationUnit tu);
        IEnumerable<ISourceFile> SourceFiles { get; }
        LibClang.Index LibClangIndex { get; }

        Symbols.ISymbolTable Symbols { get; }
        Symbols.FileMapping.IProjectFileMaps FileSymbolMaps { get; }
    }

    public class ProjectIndex : IProjectIndex
    {
        private readonly Dictionary<FilePath, IHeaderFile> _headers;
        private readonly Dictionary<FilePath, ISourceFile> _sources;

        private object _lock;
        private readonly LibClang.Index _libClangIndex;
        private Func<FilePath, bool> _fileFilter;

        private Symbols.ProjectSymbolTable _symbols;
        private Symbols.FileMapping.IProjectFileMaps _fileSymbolMappings;
                        
        public ProjectIndex(Func<FilePath, bool> fileFilter)
        {
            _lock = new object();
            _fileFilter = fileFilter;
            _libClangIndex = new LibClang.Index(false, true);
            _headers = new Dictionary<FilePath, IHeaderFile>();
            _sources = new Dictionary<FilePath, ISourceFile>();

            _fileSymbolMappings = new Symbols.FileMapping.ProjectFileMaps();
            _symbols = new Symbols.ProjectSymbolTable(_fileSymbolMappings);
        }

        public event ProjectItemEvent ItemUpdated;

        public void RaiseItemUpdatedEvent(FilePath path)
        {
            ProjectItemEvent handler = ItemUpdated;
            if (handler != null)
                handler(path);
        }

        public bool IsProjectFile(FilePath path)
        {
            return _sources.ContainsKey(path) || _headers.ContainsKey(path);
        }

        public ISourceFile FindSourceFile(FilePath path)
        {
            lock (_lock)
            {
                if (_sources.ContainsKey(path))
                    return _sources[path];
            }
            return null;
        }

        public IHeaderFile FindHeaderFile(FilePath path)
        {
            lock(_lock)
            {
                if (_headers.ContainsKey(path))
                    return _headers[path];
            }
            return null;
        }

        public IProjectFile FindProjectItem(FilePath path)
        {
            lock (_lock)
            {
                if (_sources.ContainsKey(path))
                    return _sources[path];

                if (_headers.ContainsKey(path))
                    return _headers[path];
            }
            return null;
        }

        public void UpdateSourceFile(FilePath path, LibClang.TranslationUnit tu)
        {
            TranslationUnit tuToDispose = null;
            lock (_lock)
            {
                if (_sources.ContainsKey(path))
                {
                    tuToDispose = _sources[path].TranslationUnit;
                    RemoveSourceFile(path);
                }
                AddSourceFile(path, tu);               
            }
            //RaiseItemUpdatedEvent(path);
            //if(tuToDispose != null)
              //  tuToDispose.Dispose();
            /*
            if(path.FileName == "blah.cc")
                IndexTranslationUnit(tu);
            */
            return;
        }

        private void RemoveSourceFile(FilePath path)
        {
            if (_sources.ContainsKey(path) == false)
                return;

            ISourceFile sf = _sources[path];

            List<FilePath> unrefedHeaders = new List<FilePath>();
            foreach(IHeaderFile header in _headers.Values)
            {
                header.RemoveReferingSource(sf);
                if (header.IsReferenced() == false)
                    unrefedHeaders.Add(header.Path);
            }
            foreach(FilePath p in unrefedHeaders)
            {
                _headers.Remove(p);
            }
            _sources.Remove(path);            
        }

        private void AddSourceFile(FilePath path, TranslationUnit tu)
        {
            SourceFile sourceFile = new SourceFile(path, tu);
            _sources.Add(path, sourceFile);

            foreach (TranslationUnit.HeaderInfo headerInfo in tu.HeaderFiles)
            {
                IHeaderFile headerFile = FindOrCreateHeaderFile(headerInfo);

                headerFile.SetReferencedBy(sourceFile);
                sourceFile.AddIncludedHeader(headerFile);
            }
        }

        private IHeaderFile FindOrCreateHeaderFile(TranslationUnit.HeaderInfo headerInfo)
        {
            IHeaderFile result;

            FilePath path = FilePath.Make(System.IO.Path.GetFullPath(headerInfo.File.Name));

            if (_headers.TryGetValue(path, out result) == false)
            {
                result = new HeaderFile(path);
                _headers.Add(path, result);
            }
            return result;
        }

        public LibClang.Index LibClangIndex { get { return _libClangIndex; } }

        public Symbols.FileMapping.IProjectFileMaps FileSymbolMaps { get { return _fileSymbolMappings; } }
        public Symbols.ISymbolTable Symbols { get { return _symbols; } }

        public IEnumerable<ISourceFile> SourceFiles 
        {
            get { return _sources.Values; }
        }
    }
}
