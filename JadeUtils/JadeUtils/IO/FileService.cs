using System.Collections.Generic;

namespace JadeUtils.IO
{
    public class FileHandleStore
    {
        private Dictionary<FilePath, IFileHandle> _handles;

        public FileHandleStore()
        {
            _handles = new Dictionary<FilePath, IFileHandle>();
        }

        public bool Contains(FilePath path)
        {
            return _handles.ContainsKey(path);
        }

        public bool Contains(IFileHandle handle)
        {
            return Contains(handle.Path);
        }

        public IFileHandle Get(FilePath path)
        {
            IFileHandle handle;
            return _handles.TryGetValue(path, out handle) ? handle : null;
        }

        public void Add(IFileHandle handle)
        {
            System.Diagnostics.Debug.Assert(Contains(handle) == false);
            _handles.Add(handle.Path, handle);
        }

        public bool Remove(FilePath path)
        {
            return _handles.Remove(path);
        }

        public bool Remove(IFileHandle handle)
        {
            return Remove(handle.Path);
        }
    }

    public class FileService : IFileService
    {
        #region FileCreated Event

        public event FileChangeEvent FileCreated;

        private void OnFileCreated(IFileHandle file)
        {
            FileChangeEvent handler = FileCreated;
            if (handler != null)
            {
                handler(new FileChangeEventArgs(file, FileChangeType.Created));
            }
        }

        #endregion

        #region Data

        private FileHandleStore _handles;

        #endregion

        #region Constructor

        public FileService()
        {
            _handles = new FileHandleStore();
        }

        #endregion

        #region FileHandle Creation

        public IFileHandle MakeFileHandle(string path)
        {
            return MakeFileHandle(FilePath.Make(path));
        }

        public IFileHandle MakeFileHandle(FilePath path)
        {
            IFileHandle file = _handles.Get(path);
            if (file == null)
            {
                file = new FileHandle(this, path);
                _handles.Add(file);
                OnFileCreated(file);
            }
            return file;
        }
        
        #endregion
    }
}
