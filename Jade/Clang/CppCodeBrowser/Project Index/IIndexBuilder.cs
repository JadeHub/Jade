using JadeUtils.IO;
using System;

namespace CppCodeBrowser
{
    public class ItemIndexedEventArgs : EventArgs
    {
        public readonly IProjectFile Item;

        public ItemIndexedEventArgs(IProjectFile item)
        {
            Item = item;
        }
    }

    public class ItemIndexingFailedEventArgs : EventArgs
    {
        public readonly FilePath Path;

        public ItemIndexingFailedEventArgs(FilePath path)
        {
            Path = path;
        }
    }
    
    public interface IIndexBuilder : IDisposable
    {
        bool ParseFile(FilePath path, string[] compilerArgs);
        IProjectIndex Index { get; }
    }    
}
