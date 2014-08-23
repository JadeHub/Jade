using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JadeUtils.IO;

namespace CppCodeBrowser
{
    public interface ISourceFile : IProjectFile
    {
        LibClang.TranslationUnit TranslationUnit { get; }

       // IEnumerable<IHeaderFile> Headers { get; }

        LibClang.Cursor GetCursorAt(FilePath path, int offset);
    }

    public class SourceFile : ISourceFile
    {
        private LibClang.TranslationUnit _tu;
    //    private HashSet<IHeaderFile> _headerFiles;
        private Symbols.FileMapping.IFileMap _fileSymbolMap;

        internal SourceFile(FilePath path, LibClang.TranslationUnit tu)
        {
            Path = path;
            _tu = tu;
            _fileSymbolMap = new Symbols.FileMapping.FileMap(path);
          //  _headerFiles = new HashSet<IHeaderFile>();
        }

        public override string ToString()
        {
            return Path.FileName;
        }

        #region Properties

       /* public ProjectItemType Type
        {
            get { return ProjectItemType.SourceFile; }
        }*/

        /// <summary>
        /// File path.
        /// </summary>
        public FilePath Path
        {
            get;
            private set;
        }

        public LibClang.Cursor GetCursorAt(FilePath path, int offset)
        {
            return _tu.GetCursorAt(path.Str, offset);
        }
                
        public LibClang.TranslationUnit TranslationUnit { get {return _tu;} }
      //  public IEnumerable<IHeaderFile> Headers { get { return _headerFiles; } }
                
        /// <summary>
        /// Diagnostic objects located in this file.
        /// </summary>
        public IEnumerable<LibClang.Diagnostic> Diagnostics
        {
            get
            {
                foreach(LibClang.Diagnostic d in _tu.Diagnostics)
                {
                    if (d.Location != null && d.Location.File.Name == Path.Str)
                        yield return d;
                }
            }
        }

        #endregion
        /*
        public void AddIncludedHeader(IHeaderFile headerFile)
        {
            _headerFiles.Add(headerFile);
        }*/

        public Symbols.FileMapping.IFileMap FileSymbolMap { get { return null; } }
    }
}
