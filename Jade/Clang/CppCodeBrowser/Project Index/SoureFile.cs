using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CppCodeBrowser
{
    public class SourceFile : IProjectItem
    {
        private LibClang.TranslationUnit _tu;

        internal SourceFile(string path, LibClang.TranslationUnit tu)
        {
            Path = path;
            _tu = tu;
        }

        #region Properties

        public ProjectItemType Type
        {
            get { return ProjectItemType.SourceFile; }
        }

        /// <summary>
        /// File path.
        /// </summary>
        public string Path
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
            get { yield return _tu; }
        }

        /// <summary>
        /// Diagnostic objects located in this file.
        /// </summary>
        public IEnumerable<LibClang.Diagnostic> Diagnostics
        {
            get
            {
                return _tu.Diagnostics.Where(d => d.Location.File.Name == Path);
            }
        }

        #endregion
    }
}
