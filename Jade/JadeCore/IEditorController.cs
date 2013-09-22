using System;
using JadeUtils.IO;

namespace JadeCore
{
    public interface IEditorDoc
    {
        string Name { get; }
        FilePath Path { get; }
        bool Modified { get; }
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
        event EditorDocChangeEvent DocumentClosed;
        event EditorDocChangeEvent DocumentSelected;

        IEditorDoc ActiveDocument { get; }
        bool HasOpenDocuments{ get; }
        
        void OpenSourceFile(IFileHandle file);
        void CloseAllDocuments();        
    }
}
