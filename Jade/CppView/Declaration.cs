using System.Diagnostics;

namespace CppView
{
    using JadeUtils.IO;

    public interface ICodeElement
    {
        ICodeLocation Location { get; }
        ICodeRange Range { get; }
        string Name { get; }
        LibClang.Cursor LocalCursor { get; }
    }

    public interface IDeclaration : ICodeElement
    {
        LibClang.Type Type { get; }
        LibClang.Cursor Cursor { get; }
        LibClang.Indexer.EntityKind Kind { get; }
        string Usr { get; }
        FilePath Path { get; }
    }    

    public class Declaration : IDeclaration
    {
        #region Data

        private LibClang.Indexer.DeclInfo _decl;
        private ICodeLocation _location;
        private ICodeRange _range;
        private FilePath _path;
        private LibClang.Cursor _localCursor;
        
        #endregion

        public Declaration(LibClang.Indexer.DeclInfo decl, FilePath path, LibClang.Cursor localCursor)
        {
            _decl = decl;
            _path = path;
            _localCursor = localCursor;
            Debug.Assert(JadeUtils.IO.Path.AreSamePath(decl.Location.File.Name, _path.ToString()));
            //Debug.WriteLine("Decl " + decl.Cursor.Spelling + " at " + decl.Cursor.Location);
        }

        public ICodeLocation Location
        {
            get { return _location ?? (_location = new CodeLocation(_decl.Location, _path)); }
        }

        public ICodeRange Range
        {
            get { return _range ?? (_range = new CodeRange(_decl.Cursor, _path)); }
        }

        public FilePath Path
        {
            get { return _path; }
        }

        public string Name
        {
            get { return _decl.EntityInfo.Name; }
        }

        public string Usr
        {
            get { return _decl.Cursor.Usr; }
        }

        public LibClang.Type Type { get { return _decl.Cursor.Type; } }

        public LibClang.Cursor Cursor { get { return _decl.Cursor; } }

        public LibClang.Indexer.EntityKind Kind
        {
            get { return _decl.EntityInfo.Kind; }
        }

        public LibClang.Cursor LocalCursor
        {
            get { return _localCursor; }
        }

        public override int GetHashCode()
        {
            return _decl.EntityInfo.Usr.GetHashCode() + Location.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0} is a {1} at {2} in {3} Range({4}) Usr \"{5}\" Type {6} Definition {7} Redef {8}",
                                 Name, Kind, Location, Location.Path, Range, Usr, _decl.Cursor.Type, _decl.IsDefinition, _decl.IsRedefinition);
        }
    }
}
