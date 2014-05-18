
using System.Collections.Generic;
using System.Linq;
using JadeUtils.IO;

namespace CppCodeBrowser
{
    public interface IHeaderFile : IProjectFile
    {
        IEnumerable<ISourceFile> SourceFiles { get; }
    }

    public class HeaderFile : IHeaderFile
    {
        private HashSet<ISourceFile> _sourceFiles;

        public HeaderFile(FilePath path)
        {
            Path = path;
            _sourceFiles = new HashSet<ISourceFile>();
        }

        #region Properties

        public ProjectItemType Type
        {
            get { return ProjectItemType.HeaderFile; }
        }

        /// <summary>
        /// File path.
        /// </summary>
        public FilePath Path
        {
            get;
            private set;
        }

        public IEnumerable<ISourceFile> SourceFiles { get { return _sourceFiles; } }
        
        /// <summary>
        /// Diagnostic objects located in this file.
        /// </summary>
        public IEnumerable<LibClang.Diagnostic> Diagnostics
        {
            get
            {
                foreach(ISourceFile sourceFile in _sourceFiles)
                {
                    foreach (LibClang.Diagnostic diag in sourceFile.TranslationUnit.Diagnostics.Where(d => System.IO.Path.GetFullPath(d.Location.File.Name) == Path.Str))
                    {
                        yield return diag;
                    }
                }
            }
        }

        #endregion

        #region Methods

        public void SetReferencedBy(ISourceFile sourceFile)
        {
            _sourceFiles.Add(sourceFile);
        }

        #endregion
    }
}
