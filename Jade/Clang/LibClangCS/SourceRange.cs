using System.Diagnostics;

namespace LibClang
{
    /// <summary>
    /// An immutable wrapper around libclang's SourceRange type.
    /// A SourceRange represents a start and end position within a code file.
    /// </summary>
    public sealed class SourceRange
    {       
        #region Constructor

        internal delegate SourceRange CreateSourceRangeDel(Library.SourceRange handle);

        internal SourceRange(Library.SourceRange handle, ITranslationUnitItemFactory itemFactory)
        {
            Debug.Assert(!handle.IsNull);
            Handle = handle;
            Start = itemFactory.CreateSourceLocation(Library.clang_getRangeStart(Handle));
            End = itemFactory.CreateSourceLocation(Library.clang_getRangeEnd(Handle));
            Debug.Assert(Start <= End);
            Debug.Assert(Start.File == End.File);
        }
                
        #endregion

        #region Properties

        internal Library.SourceRange Handle { get; private set; }        
        public SourceLocation Start { get; private set; }
        public SourceLocation End { get; private set; }
        public bool Null { get { return Handle == Library.SourceRange.NullRange; } }
        public int Length { get { return End.Offset - Start.Offset; } }

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
