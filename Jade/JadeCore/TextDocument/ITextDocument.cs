using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit.Document;
using JadeUtils.IO;

namespace JadeCore
{
    public delegate void TextChangedEvent(UInt64 version);

    public interface ITextDocument
    {
        #region Events

        event TextChangedEvent TextChanged; // pass version number
        event EventHandler ModifiedChanged;
        event EventHandler<DocumentChangeEventArgs> Changed;

        #endregion

        #region Properties

        string Name { get; }
        IFileHandle File { get; }
        bool Modified { get; }
        int TextLength { get; }
        string Text { get; }
        UInt64 Version { get; }
        ICSharpCode.AvalonEdit.Document.TextDocument AvDoc { get; }

        #endregion

        #region Methods

        bool GetLineAndColumnForOffset(int offset, out int line, out int col);
        int GetLineNumForOffset(int offset);
        ISegment GetLineForOffset(int offset);
        string GetText(ISegment segment);        
        bool Save(IFileHandle file);
        void DiscardChanges();
        TextDocumentSnapshot CreateSnapshot();

        #endregion
    }
}
