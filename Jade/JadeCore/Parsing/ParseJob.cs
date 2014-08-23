using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JadeUtils.IO;

namespace JadeCore.Parsing
{
    public enum ParsePriority
    {
        Background,
        Editing
    }

    public class ParseJob
    {
        private FilePath _path;
        private string[] _compilerArgs;
        private CppCodeBrowser.IIndexBuilder _indexBuilder;

        public ParseJob(FilePath path, string[] compilerArgs, CppCodeBrowser.IIndexBuilder indexBuilder)
        {
            _path = path;
            _compilerArgs = compilerArgs;
            _indexBuilder = indexBuilder;
        }

        public void Parse()
        {
            CppCodeBrowser.ParseResult result = _indexBuilder.ParseFile(_path, _compilerArgs);

        }

        public FilePath Path { get { return _path; } }
    }

    public interface IParser
    {
        void AddJob(ParsePriority priority, ParseJob job);
    }
}
