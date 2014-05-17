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

        bool ContainsProjectItem(FilePath path);
        void RemoveProjectItem(FilePath path);
        IProjectItem FindProjectItem(FilePath path);
        bool UpdateSourceFile(FilePath path, LibClang.TranslationUnit tu);
        IEnumerable<LibClang.TranslationUnit> TranslationUnits { get; }
        LibClang.Index LibClangIndex { get; }
    }

    public class ProjectIndex : IProjectIndex
    {        
        private readonly Dictionary<FilePath, IProjectItem> _items;
        private object _lock;
        private readonly LibClang.Index _libClangIndex;
                        
        public ProjectIndex()
        {
            _items = new Dictionary<FilePath, IProjectItem>();
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

        public IProjectItem FindProjectItem(FilePath path)
        {
            IProjectItem item;
            lock (_lock)
            {
                return _items.TryGetValue(path, out item) ? item : null;
            }
        }

        public bool UpdateSourceFile(FilePath path, LibClang.TranslationUnit tu)
        {
            IProjectItem item = null;
            lock (_lock)
            {
                if (_items.ContainsKey(path))
                {

                }
                else
                {
                    item = new SourceFile(path, tu);
                    _items.Add(path, item);
                  //  RaiseItemUpdatedEvent(path);
                    foreach (TranslationUnit.HeaderInfo header in tu.HeaderFiles)
                    {
                        RecordHeader(header, tu);
                    }
                }
            }
            return true;
        }

        public bool ContainsProjectItem(FilePath path)
        {
            lock (_lock)
            {
                return _items.ContainsKey(path);
            }
        }

        public void RemoveProjectItem(FilePath path)
        {
            lock (_lock)
            {
                _items.Remove(path);
            }
        }

        private void RecordHeader(TranslationUnit.HeaderInfo headerInfo, TranslationUnit tu)
        {
            lock (_lock)
            {
                FilePath path = FilePath.Make(System.IO.Path.GetFullPath(headerInfo.File.Name));

                IProjectItem item;
                if (_items.TryGetValue(path, out item) == false)
                {
                    item = new HeaderFile(path);
                    RaiseItemUpdatedEvent(path);
                    _items.Add(path, item);
                }

                if (item.Type != ProjectItemType.HeaderFile || !(item is HeaderFile))
                    throw new ApplicationException(headerInfo + " previously seen as a source file.");

                HeaderFile header = item as HeaderFile;
                header.AddTranslationUnit(tu);
            }
        }

        public LibClang.Index LibClangIndex { get { return _libClangIndex; } }

        public IEnumerable<LibClang.TranslationUnit> TranslationUnits
        {
            get
            {
                //todo locking??
                foreach (IProjectItem item in _items.Values)
                {
                    if (item is SourceFile)
                        yield return (item as SourceFile).TranslationUnits.First();
                }                 
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
