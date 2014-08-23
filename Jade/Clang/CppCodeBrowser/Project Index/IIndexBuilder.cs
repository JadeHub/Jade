using JadeUtils.IO;
using System;

namespace CppCodeBrowser
{
    public interface IIndexBuilder : IDisposable
    {
        ParseResult ParseFile(FilePath path, string[] compilerArgs);        
        IProjectIndex Index { get; }
    }    
}
