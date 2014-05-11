using JadeUtils.IO;
using System;
using System.Collections.Generic;

namespace JadeCore
{
    public delegate void EditorDocChangeEvent(EditorDocChangeEventArgs args);

    public interface IEditorController : IDisposable
    {
        event EditorDocChangeEvent ActiveDocumentChanged;
        event EditorDocChangeEvent DocumentOpened;
        event EditorDocChangeEvent DocumentClosed;

        IEditorDoc ActiveDocument { get; set; }
        bool HasOpenDocuments{ get; }
        IEnumerable<IEditorDoc> ModifiedDocuments { get; }

        void OpenDocument(IFileHandle file);
        void SaveActiveDocument();
        void CloseAllDocuments();
        void CloseActiveDocument();
        void DiscardChangesToActiveDocument();
        
        /// <summary>
        /// Close all documents, clear cache etc
        /// </summary>
        void Reset();
    }
}
