using System;
using JadeUtils.IO;

namespace JadeCore
{
    public interface IEditorDoc
    {
        event EventHandler OnClosing;

        string Name { get; }
        FilePath Path { get; }
        bool Modified { get; set; }
        string Content { get; set; }

        void Close();
    }

    public class EditorDocChangeEventArgs : EventArgs
    {
        public IEditorDoc Document;

        public EditorDocChangeEventArgs(IEditorDoc doc)
        {
            Document = doc;
        }
    }

    public delegate void EditorDocChangeEvent(EditorDocChangeEventArgs args);

    public interface IEditorController
    {
        event EditorDocChangeEvent DocumentOpened;
        event EditorDocChangeEvent DocumentSelected;

        IEditorDoc ActiveDocument { get; set; }
        bool HasOpenDocuments{ get; }

        void OpenDocument(IFileHandle file);
        void SaveActiveDocument();
        void CloseAllDocuments();
        void CloseActiveDocument();

    }
}
