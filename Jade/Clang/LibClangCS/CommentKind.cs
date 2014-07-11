using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibClang
{
    public enum CommentKind
    {
        /**
         * Null comment.  No AST node is constructed at the requested location
         * because there is no text or a syntax error.
         */
        Null = 0,

        /**
         * Plain text.  Inline content.
         */
        Text = 1,

        /**
         * A command with word-like arguments that is considered inline content.
         *
         * For example: \\c command.
         */
        InlineCommand = 2,

        /**
         * HTML start tag with attributes (name-value pairs).  Considered
         * inline content.
         *
         * For example:
         * \verbatim
         * <br> <br /> <a href="http://example.org/">
         * \endverbatim
         */
        HTMLStartTag = 3,

        /**
         * HTML end tag.  Considered inline content.
         *
         * For example:
         * \verbatim
         * </a>
         * \endverbatim
         */
        HTMLEndTag = 4,

        /**
         * A paragraph, contains inline comment.  The paragraph itself is
         * block content.
         */
        Paragraph = 5,

        /**
         * A command that has zero or more word-like arguments (number of
         * word-like arguments depends on command name) and a paragraph as an
         * argument.  Block command is block content.
         *
         * Paragraph argument is also a child of the block command.
         *
         * For example: \has 0 word-like arguments and a paragraph argument.
         *
         * AST nodes of special kinds that parser knows about (e. g., \\param
         * command) have their own node kinds.
         */
        BlockCommand = 6,

        /**
         * A \\param or \\arg command that describes the function parameter
         * (name, passing direction, description).
         *
         * For example: \\param [in] ParamName description.
         */
        ParamCommand = 7,

        /**
         * A \\tparam command that describes a template parameter (name and
         * description).
         *
         * For example: \\tparam T description.
         */
        TParamCommand = 8,

        /**
         * A verbatim block command (e. g., preformatted code).  Verbatim
         * block has an opening and a closing command and contains multiple lines of
         * text (\c VerbatimBlockLine child nodes).
         *
         * For example:
         * \\verbatim
         * aaa
         * \\endverbatim
         */
        VerbatimBlockCommand = 9,

        /**
         * A line of text that is contained within a
         * VerbatimBlockCommand node.
         */
        VerbatimBlockLine = 10,

        /**
         * A verbatim line command.  Verbatim line has an opening command,
         * a single line of text (up to the newline after the opening command) and
         * has no closing command.
         */
        VerbatimLine = 11,

        /**
         * A full comment attached to a declaration, contains block content.
         */
        FullComment = 12
    };
}
