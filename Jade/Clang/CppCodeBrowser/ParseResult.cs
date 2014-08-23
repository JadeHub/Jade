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
        private FilePath _path;
        private LibClang.TranslationUnit _translationUnit;
        private List<ParseFile> _files;

        public ParseResult(FilePath path, IEnumerable<ParseFile> files, LibClang.TranslationUnit tu)
        {
            _path = path;
            _translationUnit = tu;
            _files = new List<ParseFile>(files);
        }

        public void Dispose()
        {
            if (_translationUnit != null)
            {
                _translationUnit.Dispose();
                _translationUnit = null;
            }
        }

        public FilePath Path { get { return _path; } }
        public LibClang.TranslationUnit TranslationUnit { get { return _translationUnit; } }
        public IEnumerable<ParseFile> Files { get { return _files; } }
    }
}
