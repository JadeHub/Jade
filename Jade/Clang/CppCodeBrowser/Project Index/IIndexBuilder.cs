using JadeUtils.IO;
using System;
using LibClang;

namespace CppCodeBrowser
{
    public interface IIndexBuilder : IDisposable
    {
        ParseResult ParseFile(FilePath path, string[] compilerArgs);
        void IndexTranslationUnit(ParseResult parseResult);
        IProjectIndex Index { get; }
    }    
}
