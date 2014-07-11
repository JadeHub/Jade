using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibClang
{
    public enum CommentKind
    {
        Null = 0,

        /// <summary>
        /// Plain text.  Inline content.
        /// </summary>
        Text = 1,
        
        /// <summary>
        /// A command with word-like arguments that is considered inline content.
        /// For example: \\c command.
        /// </summary>
        InlineCommand = 2,

        /// <summary>
        /// HTML start tag with attributes (name-value pairs).  Considered inline content.
        /// For example: <br> <br /> <a href="http://example.org/">
        /// </summary>
        HTMLStartTag = 3,

        /// <summary>
        /// HTML end tag.  Considered inline content.
        /// For example: </a>
        /// </summary>
         HTMLEndTag = 4,

        /// <summary>
        /// A paragraph, contains inline comment.  The paragraph itself is block content.
        /// </summary>
        Paragraph = 5,

        /// <summary>
        /// A command that has zero or more word-like arguments (number of
        /// word-like arguments depends on command name) and a paragraph as an
        /// argument.  Block command is block content.
        /// 
        /// Paragraph argument is also a child of the block command.
        /// For example: \has 0 word-like arguments and a paragraph argument.
        /// AST nodes of special kinds that parser knows about (e. g., \\param command) have their own node kinds.
        /// </summary>
         BlockCommand = 6,

        /// <summary>
        /// A \\param or \\arg command that describes the function parameter
        /// (name, passing direction, description).
        /// 
        /// For example: \\param [in] ParamName description.
        /// </summary>
        ParamCommand = 7,

        /// <summary>
        /// \\tparam command that describes a template parameter (name and description).
        /// For example: \\tparam T description.
        /// </summary>
        TParamCommand = 8,

        /// <summary>
        /// A verbatim block command (e. g., preformatted code).  Verbatim
        /// block has an opening and a closing command and contains multiple lines of
        /// text (\c VerbatimBlockLine child nodes).
        /// For example: aaa
        /// </summary>
        VerbatimBlockCommand = 9,

        /// <summary>
        /// A line of text that is contained within a VerbatimBlockCommand node.
        /// </summary>
        VerbatimBlockLine = 10,

        /// <summary>
        /// A verbatim line command.  Verbatim line has an opening command,
        /// a single line of text (up to the newline after the opening command) and
        /// has no closing command.
        /// </summary>
        VerbatimLine = 11,

        /// <summary>
        /// A full comment attached to a declaration, contains block content.
        /// </summary>
        FullComment = 12
    };

    /// <summary>
    /// The most appropriate rendering mode for an inline command, chosen on command semantics in Doxygen.
    /// </summary>
    public enum CommentInlineCommandRenderKind
    {
        /// <summary>
        /// Command argument should be rendered in a normal font.
        /// </summary>
        Normal,

        /// <summary>
        /// Command argument should be rendered in a bold font.
        /// </summary>
        Bold,

        /// <summary>
        /// Command argument should be rendered in a monospaced font.
        /// </summary>
        Monospaced,

        /// <summary>
        /// Command argument should be rendered emphasized (typically italic font).
        /// </summary>
        Emphasized
    };

    /// <summary>
    /// Describes parameter passing direction for \\param or \\arg command.
    /// </summary>
    public enum CommentParamPassDirection
    {
        /// <summary>
        /// The parameter is an input parameter.
        /// </summary>
        In,

        /// <summary>
        /// The parameter is an output parameter.
        /// </summary>
        Out,

        /// <summary>
        /// The parameter is an input and output parameter.
        /// </summary>
        InOut
    };

}
