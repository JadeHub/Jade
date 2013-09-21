using System;
using System.Text;

namespace JadeCore.IO
{
    public struct FilePath
    {
        #region Factory

        /// <summary>
        /// Create a FilePath struct
        /// </summary>
        /// <param name="path">File path, does not need to exist.</param>
        /// <returns>FilePath object</returns>
        public static FilePath Make(string path)
        {
            return new FilePath(path);
        }

        /// <summary>
        /// Create a FilePath struct pointing to a temporary file path which is guaranteed not to exist.
        /// </summary>
        /// /// <returns>FilePath object</returns>
        public static FilePath MakeTemporaryFilePath()
        {
            return new FilePath(MakeTempFilename());
        }

        private static string MakeTempFilename()
        {
            return System.IO.Path.GetTempFileName();
        }

        #endregion

        #region Data

        private string _path;

        #endregion

        #region Constructor

        private FilePath(string path)
        {
            System.Diagnostics.Debug.Assert(path.Length > 0);
            _path = path;
        }

        #endregion

        #region Public Interface

        public override int GetHashCode()
        {
            return _path.GetHashCode();
        }

        public string Str { get { return _path; } }
        
        public bool IsAbsolute()
        {
            return System.IO.Path.IsPathRooted(_path);
        }

        /// <summary>
        /// Returns the file name portion of the path including the extention.
        /// Returns null if the path does not contain a file name.
        /// </summary>
        public string FileName 
        {
            get
            {
                return System.IO.Path.GetFileName(_path);
            }
        }

        public string Directory
        {
            get
            {
                return System.IO.Path.GetDirectoryName(_path);
            }
        }

        #endregion
    }
}
