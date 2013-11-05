using System;

namespace JadeUtils.IO
{
    public enum FileChangeType
    {
        Created,
        Opened,
        Read,
        Written,
        ChangedExternally,
        Deleted
    }

    public class FileChangeEventArgs : EventArgs
    {
        IFileHandle Handle;
        FileChangeType ChangeType;

        public FileChangeEventArgs(IFileHandle handle, FileChangeType changeType)
        {
            Handle = handle;
            ChangeType = changeType;
        }
    }

    public delegate void FileChangeEvent(FileChangeEventArgs args);

    public interface IFileObserver
    {
        void Created(IFileHandle h);
        void Opened(IFileHandle h);
        void Read(IFileHandle h);
        void Written(IFileHandle h);
        void ChangedExternally(IFileHandle h);
        void Deleted(IFileHandle h);
    }

    public interface IFileHandle : IDisposable
    {
        /// <summary>
        /// Name including extention.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Path, may be ralative or absolute.
        /// </summary>
        FilePath Path { get; }
                
        bool Exists { get; }

        void AddFileObserver(IFileObserver observer);
        void RemoveFileObserver(IFileObserver observer);
    }
}
