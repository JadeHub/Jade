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
    }

    public class ProjectIndex : IProjectIndex
    {
        private readonly Dictionary<FilePath, IHeaderFile> _headers;
        private readonly Dictionary<FilePath, ISourceFile> _sources;

        private object _lock;
        private readonly LibClang.Index _libClangIndex;
        private Func<FilePath, bool> _fileFilter;

        private Symbols.ProjectSymbolTable _symbols;
                        
        public ProjectIndex(Func<FilePath, bool> fileFilter)
        {
            _lock = new object();
            _fileFilter = fileFilter;
            _libClangIndex = new LibClang.Index(false, true);
            _headers = new Dictionary<FilePath, IHeaderFile>();
            _sources = new Dictionary<FilePath, ISourceFile>();

            _symbols = new Symbols.ProjectSymbolTable();
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
            RaiseItemUpdatedEvent(path);
            if(tuToDispose != null)
                tuToDispose.Dispose();
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

        public Symbols.ISymbolTable Symbols { get { return _symbols; } }

        public IEnumerable<ISourceFile> SourceFiles 
        {
            get { return _sources.Values; }
        }
/*
        /// <summary>
        /// Check each TranslationUnit in tus for a Cursor at location and return the unique set of Cursors found.
        /// If location is within a source file a maximum of one Cursor will be returned.
        /// If location is within a header file a maximum of one Cursor for each source file including this header will be returned.
        /// </summary>
        /// <param name="tus"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public static IEnumerable<LibClang.Cursor> GetCursors(IEnumerable<LibClang.TranslationUnit> tus, ICodeLocation location)
        {
            HashSet<LibClang.Cursor> set = new HashSet<Cursor>();
            foreach (TranslationUnit tu in tus)
            {
                LibClang.Cursor cursor = tu.GetCursorAt(location.Path.Str, location.Offset);
                if (cursor != null && set.Add(cursor))
                    yield return cursor;
            }
        }*/

        /*
        private bool IsIndexCursorKind(CursorKind k)
        {
            return k == LibClang.CursorKind.ClassDecl ||
                    k == LibClang.CursorKind.StructDecl ||
                    k == LibClang.CursorKind.CXXMethod ||
                    k == LibClang.CursorKind.Constructor ||
                    k == LibClang.CursorKind.Destructor ||
                    k == LibClang.CursorKind.FieldDecl ||
                    k == LibClang.CursorKind.ClassTemplate ||
                    k == LibClang.CursorKind.Namespace ||
                    k == LibClang.CursorKind.FunctionDecl ||
                    k == LibClang.CursorKind.VarDecl;
        }

        private void IndexTranslationUnit(TranslationUnit tu)
        {
            foreach(Cursor c in tu.Cursor.Children)
            {
                IndexCursor(tu, c);
            }
        }

        private void IndexDefinitionCursor(Cursor c)
        {
            if(c.Kind == CursorKind.ClassDecl)
            {
               // Symbols.ISymbol symbol = new Symbols.ClassDecl(c);
            }
        }

        private void IndexReferenceCursor(Cursor c)
        {
            
        }

        private void IndexCursor(TranslationUnit tu, Cursor c)
        {
            if (IsIndexCursorKind(c.Kind) == false) return;
            if (c.Location == null || c.Location.File == null) return;
                        
            FilePath path = FilePath.Make(c.Location.File.Name);
            if (_fileFilter(path) == false) return;

            if(c.Kind == CursorKind.CXXMethod)
            {
                int i = 0;
            }

            if (c.IsDefinition)
            {
                IndexDefinitionCursor(c);
            }
            else if (c.IsReference)
            {
                IndexReferenceCursor(c);
            }

            foreach(Cursor child in c.Children)
            {
                IndexCursor(tu, child);
            }

            return;
        }*/
    }
}
