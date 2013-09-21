using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
