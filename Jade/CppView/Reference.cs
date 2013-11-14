
namespace CppView
{
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
        private ISourceFile _file;
        private ISymbolTable _symbolTable;
        private ICodeRange _range;
                
        #endregion

        public Reference(string referencedUSR, LibClang.Indexer.EntityReference reference, ISourceFile file, ISymbolTable symbolTable)
        {
            _referencedUSR = referencedUSR;
            _reference = reference;
            _file = file;
            _symbolTable = symbolTable;
        }

        public string ReferencedUSR
        {
            get { return _referencedUSR; }
        }

        public IDeclaration ReferencedDecl
        {
            get { return _symbolTable.GetCanonicalDefinition(ReferencedUSR); }
        }

        public ICodeLocation Location
        {
            get { return _location ?? (_location = new CodeLocation(_reference.Location, File)); }
        }

        public ISourceFile File
        {
            get { return _file; }
        }

        public ICodeRange Range 
        {
            get { return _range ?? (_range = new CodeRange(_reference.Cursor, _file)); }
        }

        public string Name 
        {
            get { return _reference.Cursor.Spelling; }
        }

        public override string ToString()
        {
            return string.Format("Refr to {0} at {1} in {2}", ReferencedDecl.Name, Location, Location.File);
        }        
    }
}
