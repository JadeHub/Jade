using System.Linq;
using System.Collections.Generic;
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

        /// <summary>
        /// Returns a SourceLocation representing the begining of the SourceRange
        /// </summary>
        public SourceLocation Start { get; private set; }

        /// <summary>
        /// Returns a SourceLocation representing the end of the SourceRange
        /// </summary>
        public SourceLocation End { get; private set; }

        /// <summary>
        /// Returns the length of the SourceRange
        /// </summary>
        public int Length { get { return End.Offset - Start.Offset; } }

        /// <summary>
        /// Returns the set of Tokens within this SourceRange
        /// </summary>
        public TokenSet Tokens
        {
            get
            {
                if (_tokens == null)
                {
                    _tokens = TokenSet.Create(TranslationUnit, this);
                }
                return _tokens;
            }
        }

        /// <summary>
        /// Return the Token located at offset 
        /// </summary>
        /// <param name="offset"></param>
        /// <returns>The Token located at offset or null if no Token exists</returns>
        public Token GetTokenAtOffset(int offset)
        {
            if (offset < Start.Offset || offset > End.Offset) return null;

            foreach (Token t in Tokens)
            {
                if (t.Extent.Contains(offset))
                    return t;
            }
            return null;
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
