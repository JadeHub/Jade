using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadeCore
{
    //An immutable snapshot of the content of a modified ITextDocument
    public class TextDocumentSnapshot
    {
        public TextDocumentSnapshot (ITextDocument doc, UInt64 version, string text)
        {
            Document = doc;
            Version = version;
            Text = text;
        }

        public ITextDocument Document
        {
            get;
            private set;
        }

        public UInt64 Version
        {
            get;
            private set;
        }

        public string Text
        {
            get;
            private set;
        }
    }
}
