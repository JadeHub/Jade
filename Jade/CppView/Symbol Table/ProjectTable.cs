using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace CppView
{
    public interface IProjectSymbolTable : ISymbolTable
    {

    }

    public class ProjectSymbolTable : IProjectSymbolTable
    {

        //get item at location

        //get definitions

        //get definition of reference

        //get references to definition

        //code completion?

        //state - complete, refreshing, loading

        //translation units

        #region Data

        private SymbolTableImpl _indexImpl;
        private Dictionary<JadeUtils.IO.FilePath, IFileSymbolTable> _files;

        #endregion

        public ProjectSymbolTable()
        {
            _indexImpl = new SymbolTableImpl();
            _files = new Dictionary<JadeUtils.IO.FilePath, IFileSymbolTable>();
        }

        public bool Add(IDeclaration decl)
        {
            if (_indexImpl.Add(decl))
            {
                GetFileIndex(decl.Location.File).Add(decl);
                return true;
            }
            return false;
        }

        public bool Add(IReference refer)
        {
            if (_indexImpl.Add(refer))
            {
                GetFileIndex(refer.Location.File).Add(refer);
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
            _indexImpl.Dump();
            Debug.WriteLine("**********************");
            foreach (IFileSymbolTable fi in _files.Values)
            {
                Debug.WriteLine("File: " + fi.SourceFile.Path);
                fi.Dump();
                Debug.WriteLine("**********************");
            }
        }

        #region Private Methods

        private IFileSymbolTable GetFileIndex(ISourceFile file)
        {
            IFileSymbolTable fi;
            if (_files.TryGetValue(file.Path, out fi))
            {
                return fi;
            }
            fi = new FileSymbolTable(file);
            _files.Add(file.Path, fi);
            return fi;
        }

        #endregion
    }    
}
