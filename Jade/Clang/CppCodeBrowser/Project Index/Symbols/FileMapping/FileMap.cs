using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JadeUtils.IO;

namespace CppCodeBrowser.Symbols.FileMapping
{
    public interface IFileMap
    {
        void AddMapping(int startOffset, int endOffset, ISymbol symbol);
        ISymbol Get(int offset);
    }

    public class FileMap : IFileMap
    {
        private FilePath _path;

        public FileMap(FilePath path)
        {
            _path = path;
        }

        public void AddMapping(int startOffset, int endOffset, ISymbol symbol)
        {
            Debug.WriteLine(string.Format("Mapping {0}:{1}:{2} to {3}", _path.FileName, startOffset, endOffset, symbol));
        }

        public ISymbol Get(int offset)
        {
            return null;
        }
    }
}