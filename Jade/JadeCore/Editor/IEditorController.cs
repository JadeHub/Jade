using JadeUtils.IO;
using System;
using System.Collections.Generic;

namespace JadeCore
{
    public delegate void EditorDocChangeEvent(EditorDocChangeEventArgs args);
    public delegate void ActiveDocumentChangeEvent(IEditorDoc newValue, IEditorDoc oldValue);

    public interface IEditorController : CppCodeBrowser.IUnsavedFileProvider, IDisposable
    {
        event ActiveDocumentChangeEvent ActiveDocumentChanged;
        event EditorDocChangeEvent DocumentOpened;
        event EditorDocChangeEvent DocumentClosed;

        IEditorDoc ActiveDocument { get; set; }
        bool HasOpenDocuments{ get; }
        bool HasModifiedDocuments { get; }
        IEnumerable<IEditorDoc> ModifiedDocuments { get; }

        IList<TextDocumentSnapshot> GetSnapshots();

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
