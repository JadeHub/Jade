using JadeUtils.IO;
using System;

namespace CppCodeBrowser
{
    public interface IIndexBuilder : IDisposable
    {
        bool ParseFile(FilePath path, string[] compilerArgs);
        IProjectIndex Index { get; }
    }    
}
