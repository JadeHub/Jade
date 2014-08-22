using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadeCore.Editor
{
    public interface ISourceDocChangeTracker
    {
        bool RequiresParse { get; }
        UInt64 Revision { get; }
    }

    public class SourceDocChangeTracker : ISourceDocChangeTracker
    {
        private bool _requiresParse;
        private ISourceDocument _doc;

        public SourceDocChangeTracker(ISourceDocument document)
        {
            _requiresParse = false;
            _doc = document;
            _doc.TextDocument.Changed += TextDocument_Changed;
        }

        private void TextDocument_Changed(object sender, ICSharpCode.AvalonEdit.Document.DocumentChangeEventArgs e)
        {
            //if change warrants reparsing
            //initiate parse
        }

        public bool RequiresParse 
        {
            get { return _requiresParse; }
        }

        public UInt64 Revision 
        {
            get { return 0; }
        }
    }
}
