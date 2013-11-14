using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace CppView
{
    public interface IFileSymbolTable : ISymbolTable
    {
        ISourceFile SourceFile { get; }
        IEnumerable<ILineSymbolTable> Lines { get; }
    }

    public class FileSymbolTable : IFileSymbolTable
    {
        #region Data

        private ISourceFile _sourceFile;
        private SymbolTableImpl _indexImpl;
        private Dictionary<int, ILineSymbolTable> _lines;

        #endregion

        public ISourceFile SourceFile { get { return _sourceFile; } }

        public IEnumerable<ILineSymbolTable> Lines
        {
            get { return _lines.Values; }
        }

        public FileSymbolTable(ISourceFile sf)
        {
            _sourceFile = sf;
            _indexImpl = new SymbolTableImpl();
            _lines = new Dictionary<int, ILineSymbolTable>();
        }

        public bool Add(IDeclaration decl)
        {
            if (_indexImpl.Add(decl))
            {
                GetLineIndex(decl.Location.Line).Add(decl);
                return true;
            }
            return false;
        }

        public bool Add(IReference refer)
        {
            if (_indexImpl.Add(refer))
            {
                GetLineIndex(refer.Location.Line).Add(refer);
                return true;
            }
            return false;
        }

        public bool HasDeclaration(string usr)
        {
            return _indexImpl.HasDeclaration(usr);
        }

        public bool HasDeclaration(string usr, ISourceFile file, int offset)
        {
            return _indexImpl.HasDeclaration(usr, file, offset);
        }

        public bool HasReference(string refedUSR, ISourceFile file, int offset)
        {
            return _indexImpl.HasReference(refedUSR, file, offset);
        }

        public IDeclaration GetCanonicalDefinition(string usr)
        {
            return _indexImpl.GetCanonicalDefinition(usr);
        }

        public IEnumerable<IDeclaration> GetDeclarations(string usr)
        {
            return _indexImpl.GetDeclarations(usr);
        }
        public IEnumerable<string> DeclarationUSRs
        {
            get
            {
                return _indexImpl.DeclarationUSRs;
            }
        }

        public IEnumerable<IReference> References { get { return _indexImpl.References; } }

        public void Dump()
        {
            foreach (int lineNum in _lines.Keys)
            {
                ILineSymbolTable line = _lines[lineNum];
                Debug.WriteLine("Line " + lineNum);
                line.Dump();
            }
            //_indexImpl.Dump();
        }

        #region Private Methods

        private ILineSymbolTable GetLineIndex(int line)
        {
            ILineSymbolTable li;
            if (_lines.TryGetValue(line, out li))
            {
                return li;
            }
            li = new LineSymbolTable();
            _lines.Add(line, li);
            return li;
        }

        #endregion
    }
}
