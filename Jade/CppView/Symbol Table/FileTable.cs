using System.Collections.Generic;
using System.Diagnostics;

namespace CppView
{
    using JadeUtils.IO;

    /*
    public interface IFileSymbolTable //: ISymbolTable
    {
       // ISourceFile SourceFile { get; }
        bool Add(IDeclaration decl);
        bool Add(IReference reference);
        ICodeElement GetElementAt(int offset);
        void Dump();
       // ILineSymbolTable GetLine(int line);
    }
    
    public class FileSymbolTable : IFileSymbolTable
    {
        private class Line
        {
            private readonly int _line;
            private IList<ICodeElement> _elems;

            public Line(int line) 
            {
                _line = line;
                _elems = new List<ICodeElement>();
            }

            public bool AddItem(ICodeElement elem)
            {
                int col = elem.LocalCursor.Location.Column;
                
                for (int i = 0; i < _elems.Count; i++)
                {
                   // Debug.Assert(col != _elems[i].LocalCursor.Location.Column);
                    if (col < _elems[i].LocalCursor.Location.Column)
                    {
                        _elems.Insert(i, elem);
                        return true;
                    }
                }
                _elems.Add(elem);
                return true;
            }
            
            public ICodeElement GetElementAt(int offset)
            {
                for (int i = 0; i < _elems.Count; i++)
                {
                    //if(_elems[i].LocalCursor.Extent.ContainsOffset(offset))
                    if (_elems[i].LocalCursor.Location.Offset >= offset && offset <= _elems[i].LocalCursor.Location.Offset + _elems[i].Name.Length)
                    {
                        return _elems[i];
                    }
                }
                return null;
            }

            public int StartOffset
            {
                get
                {
                    Debug.Assert(_elems.Count > 0);
                    return _elems[0].LocalCursor.Extent.Start.Offset;
                }
            }

            public int EndOffset
            {
                get
                {
                    Debug.Assert(_elems.Count > 0);
                    return _elems[_elems.Count - 1].LocalCursor.Location.Offset + _elems[_elems.Count - 1].Name.Length;
                }
            }
        }

        #region Data
             
        private Dictionary<int, Line> _lines;

        #endregion

        //public ISourceFile SourceFile { get { return _sourceFile; } }


        public FileSymbolTable(FilePath path)
        {
            _lines = new Dictionary<int, Line>();
        } 

        public bool Add(IDeclaration decl)
        {
            Line line = GetOrAddLine(decl.Location.Line);
            return line.AddItem(decl);
        }

        public bool Add(IReference refer)
        {
            Line line = GetOrAddLine(refer.Location.Line);
            return line.AddItem(refer);
        }
        
        public ICodeElement GetElementAt(int offset)
        {
            foreach (Line line in _lines.Values)
            {
                if(offset >= line.StartOffset && offset <= line.EndOffset)
                {
                    return line.GetElementAt(offset);
                }
            }
            return null;
        }

        public void Dump()
        {
            foreach (int lineNum in _lines.Keys)
            {
                Line line = _lines[lineNum];
                Debug.WriteLine("Line " + lineNum);
                //line.Dump();
            }
            //_indexImpl.Dump();
        }
        
        #region Private Methods

        private Line GetOrAddLine(int l)
        {
            Line result;
            if (_lines.TryGetValue(l, out result))
                return result;
            result = new Line(l);
            _lines.Add(l, result);
            return result;
        }

        private Line FindLine(int l)
        {
            Line result;
            if (_lines.TryGetValue(l, out result))
                return result;
            return null;
        }

        #endregion
    }
     */ 
}
