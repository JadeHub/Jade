using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CppCodeBrowser.Symbols
{/*
    public class FileSymbolMap : IFileSymbolMap
    {
        private SortedList<Tuple<int, int>, ISymbol> _list;

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

        static ExtentComparer _extentComparer = new ExtentComparer();

        public FileSymbolMap()
        {
            _list = new SortedList<Tuple<int, int>, ISymbol>(_extentComparer);
        }

        public void AddMapping(int startOffset, int endOffset, ISymbol referenced)
        {

        }
    }*/
}
