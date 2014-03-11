using JadeUtils.IO;
using System;
using System.Collections.Generic;

namespace JadeCore
{
    public delegate void EditorDocChangeEvent(EditorDocChangeEventArgs args);

    public interface IEditorController
    {
        event EditorDocChangeEvent DocumentOpened;
        event EditorDocChangeEvent DocumentSelected;

        IEditorDoc ActiveDocument { get; set; }
        bool HasOpenDocuments{ get; }
        IEnumerable<IEditorDoc> ModifiedDocuments { get; }

        void OpenDocument(IFileHandle file);
        void SaveActiveDocument();
        void CloseAllDocuments();
        void CloseActiveDocument();
        
        /// <summary>
        /// Close all documents, clear cache etc
        /// </summary>
        void Reset();
    }
}
