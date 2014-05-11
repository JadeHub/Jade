using JadeUtils.IO;
using System;

namespace CppCodeBrowser
{
    public class ItemIndexedEventArgs : EventArgs
    {
        public readonly IProjectItem Item;

        public ItemIndexedEventArgs(IProjectItem item)
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

    public delegate void ItemIndexedEvent(ItemIndexedEventArgs args);
    public delegate void ItemIndexingFailedEvent(ItemIndexingFailedEventArgs args);

    public interface IIndexBuilder : IDisposable
    {
        event ItemIndexedEvent ItemIndexed;
        event ItemIndexingFailedEvent ItemIndexingFailed;

        bool ParseFile(FilePath path, string[] compilerArgs);
        IProjectIndex Index { get; }
    }    
}
