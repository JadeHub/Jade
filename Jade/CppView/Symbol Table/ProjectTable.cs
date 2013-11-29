using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace CppView
{
    using JadeUtils.IO;

    public interface IProjectSymbolTable : ISymbolTable
    {
    //    IFileSymbolTable GetFileSymbolTable(FilePath path);
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
     //   private Dictionary<FilePath, IFileSymbolTable> _files;
        
        #endregion

        public ProjectSymbolTable()
        {
            _indexImpl = new SymbolTableImpl();
            //_files = new Dictionary<JadeUtils.IO.FilePath, IFileSymbolTable>();            
        }

        #region ISymbolTable Implementation

        public bool Add(IDeclaration decl)
        {
            if (_indexImpl.Add(decl))
            {
                //GetFileIndex(decl.Location.Path).Add(decl);
                return true;
            }
            return false;
        }

        public bool Add(IReference refer)
        {
            if (_indexImpl.Add(refer))
            {
                //GetFileIndex(refer.Location.Path).Add(refer);
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

        public IEnumerable<IReference> References 
        {
            get { return _indexImpl.References; } 
        }
        /*
        public IFileSymbolTable GetFileSymbolTable(FilePath path)
        {
            IFileSymbolTable result;
            _files.TryGetValue(path, out result);
            return result;
        }*/

        public ICodeElement GetElementAt(FilePath path, int offset)
        {
            //ISourceFile sourceFile = 


            return _indexImpl.GetElementAt(path, offset);

            /*IFileSymbolTable fileTable = GetFileSymbolTable(file.Path);
            if (fileTable == null) return null;

            return fileTable.GetElementAt(offset);*/
            /*
            if (file.TranslationUnit == null)
            {
            //    file.TranslationUnit = _files.Values.Last().SourceFile.TranslationUnit;
            }
            
            LibClang.Cursor c = file.TranslationUnit.GetCursorAt(file.Path.Str, (uint)offset);
            
            if (c == null)
            {
                return null;
            }

            Debug.WriteLine("Cursor = " + c);

            IFileSymbolTable fileTable = GetFileSymbolTable(file.Path);
            if(fileTable == null)
            {
                return null;
            }

            return fileTable.GetElementAt(c.Location.Offset);
             */
            /*
            ILineSymbolTable lineTable = fileTable.GetLine(c.Location.Line);
            if (lineTable == null)
            {
                return null;
            }

            ICodeElement elem = lineTable.GetElementAt(file, c.Location.Offset);
            
            return elem;*/
        //    return null;
        }

        public void Dump()
        {
            _indexImpl.Dump();
           /* Debug.WriteLine("**********************");
            foreach (IFileSymbolTable fi in _files.Values)
            {
                Debug.WriteLine("File: " + fi.SourceFile.Path);
                fi.Dump();
                Debug.WriteLine("**********************");
            }*/
        }
        
        #endregion

        #region Private Methods
        /*
        private IFileSymbolTable GetFileIndex(FilePath path)
        {
            IFileSymbolTable fi;
            if (_files.TryGetValue(path, out fi))
            {
                return fi;
            }
            fi = new FileSymbolTable(path);
            _files.Add(path, fi);
            return fi;
        }
        */
        #endregion
    }    
}
