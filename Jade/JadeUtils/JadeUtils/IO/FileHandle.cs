using System.Collections.Generic;

namespace JadeUtils.IO
{
    public class FileHandle : IFileHandle
    {
        private IFileService _service;
        private FilePath _path;
        private List<IFileObserver> _observers;

        public FileHandle(IFileService service, string path)
            : this(service, FilePath.Make(path))
        {   
        }

        public FileHandle(string path)
            : this(null, FilePath.Make(path))
        {

        }

        public FileHandle(IFileService service, FilePath path)
        {
            _service = service;
            _path = path;
            _observers = new List<IFileObserver>();
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

        public void AddFileObserver(IFileObserver observer)
        {
            _observers.Add(observer);
        }

        public void RemoveFileObserver(IFileObserver observer)
        {
            _observers.Remove(observer);
        }

        public override string ToString()
        {
            return _path.ToString();
        }
    }
}
