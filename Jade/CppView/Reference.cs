
namespace CppView
{
    using JadeUtils.IO;

    public interface IReference : ICodeElement
    {
        /// <summary>
        /// Find the current cononical definition
        /// </summary>
        /// <param name="symbols"></param>
        /// <returns></returns>
        IDeclaration ReferencedDecl { get; }
        string ReferencedUSR { get; }
    }

    public class Reference : IReference
    {
        #region Data

        private LibClang.Indexer.EntityReference _reference;
        private string _referencedUSR;
        private ICodeLocation _location;
        private FilePath _path;
        private ISymbolTable _symbolTable;
        private ICodeRange _range;
        private LibClang.Cursor _localCursor;
                        
        #endregion

        public Reference(string referencedUSR, LibClang.Indexer.EntityReference reference, 
                        FilePath path, ISymbolTable symbolTable, LibClang.Cursor localCursor)
        {
            _referencedUSR = referencedUSR;
            _reference = reference;
            _path = path;
            _symbolTable = symbolTable;
            _localCursor = localCursor;
        }

        public string ReferencedUSR
        {
            get { return _referencedUSR; }
        }

        public IDeclaration ReferencedDecl
        {
            get 
            { 
                IDeclaration d = _symbolTable.GetCanonicalDefinition(ReferencedUSR);
                return d;
            }
        }

        public ICodeLocation Location
        {
            get { return _location ?? (_location = new CodeLocation(_reference.Location, Path)); }
        }

        public FilePath Path
        {
            get { return _path; }
        }

        public ICodeRange Range 
        {
            get { return _range ?? (_range = new CodeRange(_reference.Cursor, Path)); }
        }

        public string Name 
        {
            get { return _reference.Cursor.Spelling; }
        }

        public LibClang.Cursor LocalCursor
        {
            get { return _localCursor; }
        }

        public override string ToString()
        {
            return string.Format("Refr to {0} at {1} in {2}", ReferencedDecl.Name, Location, Location.Path);
        }        
    }
}
