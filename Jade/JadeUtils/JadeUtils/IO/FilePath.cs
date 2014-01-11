using System;
using System.Text;
using System.Diagnostics;

namespace JadeUtils.IO
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

        public override bool Equals(object obj)
        {
            if (obj is FilePath)
            {
                FilePath rhs = (FilePath)obj;

                if (this.IsNull && rhs.IsNull)
                    return true;
                                
                if(rhs.IsNull == false && this.IsNull == false)
                    return rhs.Str.ToLowerInvariant() == this.Str.ToLowerInvariant();

            }
            return false;
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
            Debug.Assert(path.Length > 0);            
            _path = path;
            if (System.IO.File.Exists(_path))
            {
                Normalise();
            }
        }

        #endregion

        public static bool operator ==(FilePath left, FilePath right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Inequality test.
        /// </summary>
        public static bool operator !=(FilePath left, FilePath right)
        {
            return !left.Equals(right);
        }

        #region Public Interface

        public override int GetHashCode()
        {
            return _path.GetHashCode();
        }

        public override string ToString()
        {
            return Str;
        }

        public string Str { get { return _path; } }
        
        public bool IsAbsolute()
        {
            return System.IO.Path.IsPathRooted(_path);
        }

        public void Normalise()
        {
            try
            {
                Uri uri = new Uri(_path);
                _path = uri.LocalPath;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }

        public bool IsNull
        {
            get { return _path == null; }
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

        public string Extention
        {
            get
            {
                return System.IO.Path.GetExtension(_path);
            }
        }

        #endregion
    }
}
