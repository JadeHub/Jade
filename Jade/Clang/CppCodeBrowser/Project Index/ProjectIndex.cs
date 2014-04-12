using JadeUtils.IO;
using LibClang;
using System;
using System.Linq;
using System.Collections.Generic;

namespace CppCodeBrowser
{   
    public interface IProjectIndex
    {
        IProjectItem FindProjectItem(FilePath path);
        IEnumerable<LibClang.TranslationUnit> TranslationUnits { get; }
    }

    public class ProjectIndex : IProjectIndex
    {        
        private readonly Dictionary<FilePath, IProjectItem> _items;
        private readonly string _projectName;
                
        public ProjectIndex(string projectName)
        {
            _projectName = projectName;
            _items = new Dictionary<FilePath, IProjectItem>();
        }

        public IProjectItem FindProjectItem(FilePath path)
        {
            IProjectItem item;
            return _items.TryGetValue(path, out item) ? item : null;
        }

        public IProjectItem AddSourceFile(FilePath path, LibClang.TranslationUnit tu)
        {
            IProjectItem item = new SourceFile(path, tu);
            _items.Add(path, item);
            foreach (TranslationUnit.HeaderInfo header in tu.HeaderFiles)
            {
                RecordHeader(header, tu);
            }
            return item;
        }

        private void RecordHeader(TranslationUnit.HeaderInfo headerInfo, TranslationUnit tu)
        {
            FilePath path = FilePath.Make(System.IO.Path.GetFullPath(headerInfo.File.Name));

            IProjectItem item;
            if (_items.TryGetValue(path, out item) == false)
            {
                item = new HeaderFile(path);
                _items.Add(path, item);
            }

            if (item.Type != ProjectItemType.HeaderFile || !(item is HeaderFile))
                throw new ApplicationException(headerInfo + " previously seen as a source file.");

            HeaderFile header = item as HeaderFile;
            header.AddTranslationUnit(tu);
        }

        public IEnumerable<LibClang.TranslationUnit> TranslationUnits
        {
            get
            {
                foreach (IProjectItem item in _items.Values)
                {
                    if (item is SourceFile)
                        yield return (item as SourceFile).TranslationUnits.First();
                }                 
            }
        }

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
