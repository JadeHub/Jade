using LibClang;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CppCodeBrowser
{    
    public interface IProjectIndex
    {
        IProjectItem FindProjectItem(string path);
    }

    public class ProjectIndex : IProjectIndex
    {
        private readonly Dictionary<string, IProjectItem> _items;
                
        public ProjectIndex()
        {
            _items = new Dictionary<string, IProjectItem>();
        }

        public IProjectItem FindProjectItem(string path)
        {
            IProjectItem item;
            return _items.TryGetValue(path, out item) ? item : null;
        }
        
        public void AddSourceFile(string path, LibClang.TranslationUnit tu)
        {
            IProjectItem item = new SourceFile(path, tu);

            _items.Add(path, item);

            foreach (TranslationUnit.HeaderInfo header in tu.HeaderFiles)
            {
                RecordHeader(header, tu);
            }
        }

        private void RecordHeader(TranslationUnit.HeaderInfo headerInfo, TranslationUnit tu)
        {
            string path = System.IO.Path.GetFullPath(headerInfo.Path);

            IProjectItem item;
            if (_items.TryGetValue(path, out item) == false)
            {
                item = new HeaderFile(path);
                _items.Add(path, item);
            }

            if (item.Type != ProjectItemType.HeaderFile || !(item is HeaderFile))
                throw new ApplicationException(headerInfo.Path + " previously seen as a source file.");

            HeaderFile header = item as HeaderFile;
            header.AddTranslationUnit(tu);
            IEnumerable<Diagnostic> ds = header.Diagnostics;
        }
    }
}
