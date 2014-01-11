
namespace LibClang
{
    public enum TokenKind 
    {
        /// <summary>
        /// A token that contains some kind of punctuation.
        /// </summary>
        Punctuation,

        /// <summary>
        /// A language keyword.
        /// </summary>
        Keyword,

        /// <summary>
        /// An identifier (that is not a keyword).
        /// </summary>
        Identifier,

        /// <summary>
        /// A numeric, string, or character literal.
        /// </summary>
        Literal,

        /// <summary>
        /// A comment.
        /// </summary>
        Comment
    };

}
