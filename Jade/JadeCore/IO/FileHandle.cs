using System;
using System.IO;

namespace JadeCore.IO
{
    public class FileHandle : IFileHandle
    {
        private IFileService _service;
        private FilePath _path;

        public FileHandle(IFileService service, string path)
            : this(service, FilePath.Make(path))
        {   
        }

        public FileHandle(IFileService service, FilePath path)
        {
            _service = service;
            _path = path;
        }

        public void Dispose()
        {
        }

        public string Name 
        {
            get{return _path.FileName;}
        }

        public FilePath Path 
        {
            get { return _path; }
        }

        public bool Exists 
        {
            get
            {
                return System.IO.File.Exists(_path.Str);
            }
        }
    }
}
