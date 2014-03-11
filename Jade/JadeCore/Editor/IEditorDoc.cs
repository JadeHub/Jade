using JadeUtils.IO;
using System;

namespace JadeCore
{
    /// <summary>
    /// A file that can be opened in the editor. 
    /// The file representation used by the EditorController
    /// EditorSourceDocument is implementation for source files
    /// </summary>
    public interface IEditorDoc : ITextDocument
    {
        event EventHandler OnClosing;
        event EventHandler OnSaved;

        bool Modified { get; set; }
        new string Content { get; set; }

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
