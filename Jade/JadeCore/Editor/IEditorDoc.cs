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
        event EventHandler OnClosing;
        event EventHandler OnSaved;

        string Name { get; }
        FilePath Path { get; }
        IFileHandle File { get; }
        bool Modified { get; set; }
        string Content { get; set; }

        void Close();
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
