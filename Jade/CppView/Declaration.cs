using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace CppView
{
    public interface ICodeElement
    {
        ICodeLocation Location { get; }
        ICodeRange Range { get; }
        string Name { get; }
    }

    public interface IDeclaration : ICodeElement
    {
        LibClang.Type Type { get; }
        LibClang.Cursor Cursor { get; }
        LibClang.Indexer.EntityKind Kind { get; }
        string Usr { get; }
    }    

    public class Declaration : IDeclaration
    {
        #region Data

        private LibClang.Indexer.DeclInfo _decl;
        private ICodeLocation _location;
        private ICodeRange _range;
        private ISourceFile _file;
        
        #endregion

        public Declaration(LibClang.Indexer.DeclInfo decl, ISourceFile file)
        {
            _decl = decl;
            _file = file;
            //Debug.Assert(decl.Location.File.Name.ToLowerInvariant() == file.Path.To)
            Debug.Assert(JadeUtils.IO.Path.AreSamePath(decl.Location.File.Name, file.Path.ToString()));
        }

        public ICodeLocation Location
        {
            get { return _location ?? (_location = new CodeLocation(_decl.Location, _file)); }
        }

        public ICodeRange Range
        {
            get { return _range ?? (_range = new CodeRange(_decl.Cursor, _file)); }
        }

        public string Name
        {
            get { return _decl.EntityInfo.Name; }
        }

        public string Usr
        {
            //get { return _decl.EntityInfo.Usr; }
            get { return _decl.Cursor.Usr; }
        }

        public LibClang.Type Type { get { return _decl.Cursor.Type; } }

        public LibClang.Cursor Cursor { get { return _decl.Cursor; } }

        public LibClang.Indexer.EntityKind Kind
        {
            get { return _decl.EntityInfo.Kind; }
        }

        public override int GetHashCode()
        {
            return _decl.EntityInfo.Usr.GetHashCode() + Location.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0} is a {1} at {2} in {3} Range({4}) Usr \"{5}\" Type {6} Definition {7} Redef {8}",
                                 Name, Kind, Location, Location.File, Range, Usr, _decl.Cursor.Type, _decl.IsDefinition, _decl.IsRedefinition);
        }
    }
}
