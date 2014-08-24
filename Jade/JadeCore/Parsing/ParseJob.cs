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
        private CppCodeBrowser.IProjectIndex _index;

        public ParseJob(FilePath path, string[] compilerArgs, CppCodeBrowser.IProjectIndex index)
        {
            _path = path;
            _compilerArgs = compilerArgs;
            _index = index;
        }

        public void Parse()
        {
            CppCodeBrowser.ParseResult result = CppCodeBrowser.Parser.Parse(_index, _path,
                                                                            _compilerArgs,
                                                                            JadeCore.Services.Provider.EditorController);
            if(result != null)
            {
                JadeCore.Services.Provider.CppParser.OnParseComplete(result);
            }
                //_indexBuilder.IndexTranslationUnit(result);
        }

        public FilePath Path { get { return _path; } }
    }

    public interface IParser
    {
        void AddJob(ParsePriority priority, ParseJob job);
    }
}
