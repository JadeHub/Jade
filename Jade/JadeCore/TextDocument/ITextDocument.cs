using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit.Document;
using JadeUtils.IO;

namespace JadeCore
{
    public interface ITextDocument
    {
        #region Events

        event EventHandler TextChanged;
        event EventHandler ModifiedChanged;

        #endregion

        #region Properties

        string Name { get; }
        IFileHandle File { get; }
        bool Modified { get; }
        int TextLength { get; }

        UInt64 Version { get; }
        
        ICSharpCode.AvalonEdit.Document.TextDocument AvDoc { get; }

        #endregion

        int GetLineNumForOffset(int offset);
        ISegment GetLineForOffset(int offset);
        string GetText(ISegment segment);
        
        bool Save(IFileHandle file);
        void DiscardChanges();
    }
}
