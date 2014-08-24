using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JadeUtils.IO;
using LibClang;

namespace CppCodeBrowser
{
    public class ParseResult : IDisposable
    {
        private IProjectIndex _index;
        private FilePath _path;
        private LibClang.TranslationUnit _translationUnit;
        private Dictionary<FilePath, ParseFile> _files;

        public ParseResult(IProjectIndex index, FilePath path, IEnumerable<ParseFile> files, LibClang.TranslationUnit tu)
        {
            _index = index;
            _path = path;
            _translationUnit = tu;
            _files = new Dictionary<FilePath, ParseFile>();
            foreach(ParseFile pf in files)
            {
                _files.Add(pf.Path, pf);
            }
        }

        public void Dispose()
        {
            if (_translationUnit != null)
            {
                _translationUnit.Dispose();
                _translationUnit = null;
            }
        }

        public IProjectIndex Index { get { return _index; } }
        public FilePath Path { get { return _path; } }
        public LibClang.TranslationUnit TranslationUnit { get { return _translationUnit; } }
        public IEnumerable<ParseFile> Files { get { return _files.Values; } }

        public UInt64 GetFileVersion(FilePath path)
        {
            ParseFile pf = null;
            if (_files.TryGetValue(path, out pf))
                return pf.Version;
            return 0;
        }
    }
}
