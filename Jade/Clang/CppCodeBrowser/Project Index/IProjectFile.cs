using System.Collections.Generic;
using JadeUtils.IO;

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
    public interface IProjectFile
    {
        /// <summary>
        /// Type of this item.
        /// </summary>
      //  ProjectItemType Type { get; }

        /// <summary>
        /// File path.
        /// </summary>
        FilePath Path { get; }

        /// <summary>
        /// Diagnostic objects located in this file.
        /// </summary>
        IEnumerable<LibClang.Diagnostic> Diagnostics { get; }

        
    }
}
