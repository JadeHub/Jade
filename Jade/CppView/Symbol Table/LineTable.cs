using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CppView
{
    using JadeUtils.IO;

    /*
    public interface ILineSymbolTable : ISymbolTable
    {
       // ICodeElement GetElementAt(int col);
    }

    public class LineSymbolTable : ILineSymbolTable
    {
        #region Data

        private SymbolTableImpl _indexImpl;
        //        private 

        #endregion

        public LineSymbolTable()
        {
            _indexImpl = new SymbolTableImpl();
        }

        public bool Add(IDeclaration decl)
        {
            if (_indexImpl.Add(decl))
            {
                return true;
            }
            return false;
        }

        public bool Add(IReference refer)
        {
            if (_indexImpl.Add(refer))
            {
                return true;
            }
            return false;
        }

        public bool HasDeclaration(string usr)
        {
            return _indexImpl.HasDeclaration(usr);
        }

        public bool HasDeclaration(string usr, FilePath path, int offset)
        {
            return _indexImpl.HasDeclaration(usr, path, offset);
        }

        public bool HasReference(string refedUSR, FilePath path, int offset)
        {
            return _indexImpl.HasReference(refedUSR, path, offset);
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

        public ICodeElement GetElementAt(ISourceFile file, int offset)
        {
            return _indexImpl.GetElementAt(file, offset);
        }

        public void Dump()
        {
            _indexImpl.Dump();
        }

        #region Private Methods


        #endregion
    }
     */ 
}
