using JadeUtils.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadeCore
{
    public interface IContentProvider
    {
        ITextDocument OpenTextDocument(IFileHandle file);
        ITextDocument FindTextDocument(IFileHandle file);
        void Reset();
    }

    public class ContentProvider : IContentProvider
    {
        private ITextDocumentCache _textDocuments;

        public ContentProvider()
        {
            _textDocuments = new TextDocumentCache();
        }

        public ITextDocument OpenTextDocument(IFileHandle file)
        {
            return _textDocuments.FindOrAdd(file);
        }

        public ITextDocument FindTextDocument(IFileHandle file)
        {
            return _textDocuments.Find(file);
        }

        public void Reset()
        {
            _textDocuments.Reset();
        }
    }
}
