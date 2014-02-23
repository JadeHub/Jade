using System.Collections.Generic;
using System.Linq;
using JadeUtils.IO;

namespace CppCodeBrowser
{
    public class HeaderFile : IProjectItem
    {
        private HashSet<LibClang.TranslationUnit> _tus;

        public HeaderFile(FilePath path)
        {
            Path = path;
            _tus = new HashSet<LibClang.TranslationUnit>();
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

        /// <summary>
        /// TranslationUnit objects which referenced this file. 
        /// 
        /// For source files this will only contain the source file's TranslationUnit object. 
        /// For header files it will contain all TranslationUnit objects which included the header.
        /// </summary>
        public IEnumerable<LibClang.TranslationUnit> TranslationUnits
        {
            get { return _tus; }
        }

        /// <summary>
        /// Diagnostic objects located in this file.
        /// </summary>
        public IEnumerable<LibClang.Diagnostic> Diagnostics
        {
            get
            {
                foreach (LibClang.TranslationUnit tu in _tus)
                {
                    foreach (LibClang.Diagnostic diag in tu.Diagnostics.Where(d => System.IO.Path.GetFullPath(d.Location.File.Name) == Path.Str))
                    {
                        yield return diag;
                    }
                }
            }
        }

        #endregion

        #region Methods

        public void AddTranslationUnit(LibClang.TranslationUnit tu)
        {
            _tus.Add(tu);
        }

        #endregion
    }
}
