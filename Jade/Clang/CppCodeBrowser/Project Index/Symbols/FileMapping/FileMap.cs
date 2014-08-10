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
        IEnumerable<Tuple<int, int, ISymbol>> Symbols { get; }
    }

    public class FileMap : IFileMap
    {
        private FilePath _path;
        private SortedList<Tuple<int, int>, ISymbol> _list;
        static ExtentComparer _extentComparer = new ExtentComparer();

        public FileMap(FilePath path)
        {
            _path = path;
            _list = new SortedList<Tuple<int, int>, ISymbol>(_extentComparer);
        }

        private class ExtentComparer : IComparer<Tuple<int, int>>
        {
            //Tuples are <startOffset, endOffset> if startOffsets are the same the shorter extent is considered to be less
            public int Compare(Tuple<int, int> a, Tuple<int, int> b)
            {
                if (a == b) return 0;

                if (a.Item1 < b.Item1) return -1;
                if (a.Item1 > b.Item1) return 1;
                //start offsets are the same
                return a.Item2 < b.Item2 ? -1 : 1;
            }
        }

        public void AddMapping(int startOffset, int endOffset, ISymbol referenced)
        {
            if (Get(startOffset) != referenced)
            {
                _list.Add(new Tuple<int, int>(startOffset, endOffset), referenced);
                Debug.WriteLine(string.Format("Mapping {0}:{1}:{2} to {3}", _path.FileName, startOffset, endOffset, referenced));
            }
        }

        public ISymbol Get(int offset)
        {
            foreach(var i in _list)
            {
                if (i.Key.Item1 <= offset && offset <= i.Key.Item2)
                    return i.Value;
            }
            return null;
        }

        public IEnumerable<Tuple<int, int, ISymbol>> Symbols 
        { 
            get 
            {
                for (int i = 0; i < _list.Count;i++)
                {
                    yield return new Tuple<int, int, ISymbol>(_list.Keys[i].Item1, _list.Keys[i].Item2, _list.Values[i]);
                }
            }
        }
    }
}