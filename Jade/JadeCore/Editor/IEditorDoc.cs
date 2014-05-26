using JadeUtils.IO;
using System;

namespace JadeCore
{
    /// <summary>
    /// A file that can be opened in the editor. 
    /// The file representation used by the EditorController
    /// EditorSourceDocument is implementation for source files
    /// </summary>
    public interface IEditorDoc
    {
        string Name { get; }
        bool Modified { get; }
        IFileHandle File { get; }

        ITextDocument TextDocument { get; }

        /// <summary>
        /// Project to which the document belongs. null if document not owned by project.
        /// </summary>
        Project.IProject Project { get; }
             
        void Save();
    }

    public class EditorDocChangeEventArgs : EventArgs
    {
        public IEditorDoc Document;

        public EditorDocChangeEventArgs(IEditorDoc doc)
        {
            Document = doc;
        }
    }
}
