using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JadeUtils.IO;

namespace JadeCore
{
    public class TextDocumentCache : ITextDocumentCache
    {
        private Dictionary<IFileHandle, ITextDocument> _cache;

        public TextDocumentCache()
        {
            _cache = new Dictionary<IFileHandle, ITextDocument>();
        }

        public ITextDocument FindOrAdd(IFileHandle file)
        {
            ITextDocument result;

            if(!_cache.TryGetValue(file, out result))
            {
                result = new TextDocument(file);
                _cache.Add(file, result);
            }
            return result;
        }

        public ITextDocument Find(IFileHandle file)
        {
            ITextDocument result;
            if (!_cache.TryGetValue(file, out result))
            {
                return null;
            }
            return result;
        }

        public void Reset()
        {
            _cache.Clear();
        }
    }
}
