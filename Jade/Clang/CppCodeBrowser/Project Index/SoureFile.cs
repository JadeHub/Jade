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
        IEnumerable<IHeaderFile> Headers { get; }

        LibClang.Cursor GetCursorAt(FilePath path, int offset);
    }

    public class SourceFile : ISourceFile
    {
        private LibClang.TranslationUnit _tu;
        private HashSet<IHeaderFile> _headerFiles;

        internal SourceFile(FilePath path, LibClang.TranslationUnit tu)
        {
            Path = path;
            _tu = tu;
            _headerFiles = new HashSet<IHeaderFile>();
        }

        #region Properties

        public ProjectItemType Type
        {
            get { return ProjectItemType.SourceFile; }
        }

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
            return TranslationUnit.GetCursorAt(path.Str, offset);
        }

        public LibClang.TranslationUnit TranslationUnit { get {return _tu;} }
        public IEnumerable<IHeaderFile> Headers { get { return _headerFiles; } }
                
        /// <summary>
        /// Diagnostic objects located in this file.
        /// </summary>
        public IEnumerable<LibClang.Diagnostic> Diagnostics
        {
            get
            {
                return _tu.Diagnostics.Where(d => d.Location.File.Name == Path.Str);
            }
        }

        #endregion

        public void AddIncludedHeader(IHeaderFile headerFile)
        {
            _headerFiles.Add(headerFile);
        }
    }
}
