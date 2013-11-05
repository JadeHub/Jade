using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CppView
{
    public interface ICodeElement
    {
        ICodeLocation Location { get; }
        ICodeRange Range { get; }
        ISourceFile File { get; }
        string Name { get; }
        string Usr { get; }
        LibClang.Indexer.EntityKind Kind { get; }
    }

    public interface IDeclaration : ICodeElement
    {
        
    }    

    public class Declaration : IDeclaration
    {
        #region Data

        private LibClang.Indexer.DeclInfo _decl;
        private ICodeLocation _location;
        private ICodeRange _range;
        private ISourceFile _file;

        #endregion

        public Declaration(LibClang.Indexer.DeclInfo decl)
        {
            _decl = decl;
        }

        public ICodeLocation Location
        {
            get { return _location ?? (_location = new CodeLocation(_decl.Location)); }
        }

        public ICodeRange Range
        {
            get{return _range ?? (_range = new CodeRange(_decl.EntityInfo.Cursor.Extent));}
        }

        public ISourceFile File
        {
            get { return _file ?? (_file = new SourceFile(null)); }
        }

        public string Name
        {
            get { return _decl.EntityInfo.Name; }
        }

        public string Usr
        {
            get { return _decl.EntityInfo.Usr; }
        }

        public LibClang.Indexer.EntityKind Kind
        {
            get { return _decl.EntityInfo.Kind; }
        }
    }
}
