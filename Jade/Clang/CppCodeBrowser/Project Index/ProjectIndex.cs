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
        
        //bool ContainsProjectItem(FilePath path);
        //void RemoveProjectItem(FilePath path);
        IProjectFile FindProjectItem(FilePath path);
        bool UpdateSourceFile(FilePath path, LibClang.TranslationUnit tu);
        IEnumerable<LibClang.TranslationUnit> TranslationUnits { get; }
        LibClang.Index LibClangIndex { get; }
    }

    public class ProjectIndex : IProjectIndex
    {
        private readonly Dictionary<FilePath, HeaderFile> _headers;
        private readonly Dictionary<FilePath, ISourceFile> _sources;

        //private readonly Dictionary<FilePath, IProjectFile> _items;
        private object _lock;
        private readonly LibClang.Index _libClangIndex;
                        
        public ProjectIndex()
        {
            _headers = new Dictionary<FilePath, HeaderFile>();
            _sources = new Dictionary<FilePath, ISourceFile>();

            //_items = new Dictionary<FilePath, IProjectFile>();
            _lock = new object();
            _libClangIndex = new LibClang.Index(false, true);
        }

        public event ProjectItemEvent ItemUpdated;

        public void RaiseItemUpdatedEvent(FilePath path)
        {
            ProjectItemEvent handler = ItemUpdated;
            if (handler != null)
                handler(path);
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

        public bool UpdateSourceFile(FilePath path, LibClang.TranslationUnit tu)
        {
            lock (_lock)
            {
                if (_sources.ContainsKey(path))
                {
                    RemoveSourceFile(path);
                }
                AddSourceFile(path, tu);
                //RaiseItemUpdatedEvent(path);
            }
            return true;
        }

        private void RemoveSourceFile(FilePath path)
        {
            _sources.Remove(path);
            //todo remove references from headers
        }

        private void AddSourceFile(FilePath path, TranslationUnit tu)
        {
            SourceFile sourceFile = new SourceFile(path, tu);
            _sources.Add(path, sourceFile);

            foreach (TranslationUnit.HeaderInfo headerInfo in tu.HeaderFiles)
            {
                HeaderFile headerFile = FindOrCreateHeaderFile(headerInfo);

                headerFile.SetReferencedBy(sourceFile);
                sourceFile.AddIncludedHeader(headerFile);
            }
        }

        private HeaderFile FindOrCreateHeaderFile(TranslationUnit.HeaderInfo headerInfo)
        {
            HeaderFile result;

            FilePath path = FilePath.Make(System.IO.Path.GetFullPath(headerInfo.File.Name));

            if (_headers.TryGetValue(path, out result) == false)
            {
                result = new HeaderFile(path);
                _headers.Add(path, result);
                //RaiseItemUpdatedEvent(path);
            }
            return result;
        }

        public LibClang.Index LibClangIndex { get { return _libClangIndex; } }

        public IEnumerable<LibClang.TranslationUnit> TranslationUnits
        {
            get
            {
                lock(_lock)
                {
                    foreach(ISourceFile sf in _sources.Values)
                    {
                        yield return sf.TranslationUnit;
                    }
                }
                /*
                //todo locking??
                foreach (IProjectFile item in _items.Values)
                {
                    if (item is SourceFile)
                        yield return (item as SourceFile).TranslationUnits.First();
                }   */              
            }
        }

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
        }
    }
}
