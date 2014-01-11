using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CppCodeBrowser
{
    public enum ProjectItemType
    {
        SourceFile,
        HeaderFile
    }

    /// <summary>
    /// Represents either a source or header file.
    /// </summary>
    public interface IProjectItem
    {
        /// <summary>
        /// Type of this item.
        /// </summary>
        ProjectItemType Type { get; }

        /// <summary>
        /// File path.
        /// </summary>
        string Path { get; }

        /// <summary>
        /// TranslationUnit objects which referenced this file. 
        /// 
        /// For source files this will only contain the source file's TranslationUnit object. 
        /// For header files it will contain all TranslationUnit objects which included the header.
        /// </summary>
        IEnumerable<LibClang.TranslationUnit> TranslationUnits { get; }

        /// <summary>
        /// Diagnostic objects located in this file.
        /// </summary>
        IEnumerable<LibClang.Diagnostic> Diagnostics { get; }
    }
}
