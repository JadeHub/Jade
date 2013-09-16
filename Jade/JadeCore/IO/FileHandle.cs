using System;
using System.IO;

namespace JadeCore.IO
{
    public interface IFileHandle : IDisposable
    {
        /// <summary>
        /// Name including extention.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Path, may be ralative or absolute.
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Path without filename
        /// </summary>
        string Directory { get; }

        bool Exists { get; }
    }

    public class FileHandle : IFileHandle
    {
        private string _path;

        public FileHandle(string path)
        {
            _path = path;
        }

        public void Dispose()
        {
        }

        public string Name 
        {
            get
            {
                return System.IO.Path.GetFileName(_path);
            }
        }

        public string Path 
        {
            get { return _path; }
        }

        public string Directory 
        {
            get
            {
                return System.IO.Path.GetDirectoryName(_path);
            }
        }

        public bool Exists 
        {
            get
            {
                return System.IO.File.Exists(_path);
            }
        }
    }

    public static class FileHandleFactory
    {
        public static IFileHandle Create(string path, bool checkExists = false)
        {
            using (IFileHandle h = new FileHandle(path))
            {
                if (checkExists && h.Exists == false)
                    throw new Exception("File " + path + " doesn't exist.");
                return h;
            }
        }
    }

}
