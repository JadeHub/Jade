using System.Diagnostics;

namespace LibClang
{
    /// <summary>
    /// An immutable wrapper around libclang's SourceRange type.
    /// A SourceRange represents a start and end position within a code file.
    /// </summary>
    public sealed class SourceRange
    {
        #region Data

        private TokenSet _tokens;

        #endregion

        #region Constructor

        internal delegate SourceRange CreateSourceRangeDel(Library.CXSourceRange handle);

        internal SourceRange(Library.CXSourceRange handle, ITranslationUnitItemFactory itemFactory)
        {
            Debug.Assert(!handle.IsNull);
            Handle = handle;
            TranslationUnit = itemFactory.TranslationUnit;
            Start = itemFactory.CreateSourceLocation(Library.clang_getRangeStart(Handle));
            End = itemFactory.CreateSourceLocation(Library.clang_getRangeEnd(Handle));
            Debug.Assert(Start <= End);
            Debug.Assert(Start.File == End.File);
        }
                
        #endregion

        #region Properties

        internal TranslationUnit TranslationUnit { get; private set; }
        internal Library.CXSourceRange Handle { get; private set; }        
        public SourceLocation Start { get; private set; }
        public SourceLocation End { get; private set; }
        public bool Null { get { return Handle == Library.CXSourceRange.NullRange; } }
        public int Length { get { return End.Offset - Start.Offset; } }

        public TokenSet Tokens
        {
            get
            {
                if(_tokens == null)
                {
                    _tokens = TokenSet.Create(TranslationUnit, this);
                }
                return _tokens;
            }
        }

        #endregion

        public override string ToString()
        {
            return string.Format("{0} {1}->{2}", Start.File.Name, Start.Offset, End.Offset);
        }

        public bool Contains(int offset)
        {
            return offset >= Start.Offset && offset <= End.Offset;
        }
    }
}
