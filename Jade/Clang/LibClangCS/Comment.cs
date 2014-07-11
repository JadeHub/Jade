using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibClang
{
    public class Comment
    {
        private Library.CXComment _handle;

        internal Comment(Library.CXComment handle)
        {
            _handle = handle;
        }

        #region Properties

        public CommentKind Kind
        {
            get { return Library.clang_Comment_getKind(_handle); }
        }

        public IEnumerable<Comment> Children
        {
            get
            {
                List<Comment> children = new List<Comment>();
                for (uint i = 0; i < Library.clang_Comment_getNumChildren(_handle);i++)
                {
                    Comment child = new Comment(Library.clang_Comment_getChild(_handle, i));
                    children.Add(child);
                }
                return children;
            }
        }
        
        public bool IsWhiteSpace
        {
            get { return Library.clang_Comment_isWhitespace(_handle) != 0; }
        }

        public bool InlineConentHasTrailingNewline
        {
            get { return Library.clang_InlineContentComment_hasTrailingNewline(_handle) != 0; }
        }

        public string TextCommentText
        {
            get { return Library.clang_TextComment_getText(_handle).ManagedString; }
        }

        public string InlineCommandName
        {
            get { return Library.clang_InlineCommandComment_getCommandName(_handle).ManagedString; }
        }

        public CommentInlineCommandRenderKind InlineCommandRenderKind
        {
            get { return Library.clang_InlineCommandComment_getRenderKind(_handle); }
        }

        public IEnumerable<string> InlineCommandArgs
        {
            get
            {
                List<string> args = new List<string>();

                for(uint i=0;i<Library.clang_InlineCommandComment_getNumArgs(_handle);i++)
                {
                    args.Add(Library.clang_InlineCommandComment_getArgText(_handle, i).ManagedString);
                }
                return args;
            }
        }
        
        public string HTMLTagName
        {
            get { return Library.clang_HTMLTagComment_getTagName(_handle).ManagedString; }
        }

        public bool HTMLStartTagIsSelfClosing
        {
            get { return Library.clang_HTMLStartTagComment_isSelfClosing(_handle) != 0; }
        }

        public IEnumerable<Tuple<string, string>> HTMLStartTagAttributes
        {
            get
            {
                List<Tuple<string, string>> attribs = new List<Tuple<string, string>>();

                for (uint i = 0; i < Library.clang_HTMLStartTag_getNumAttrs(_handle);i++ )
                {
                    string name = Library.clang_HTMLStartTag_getAttrName(_handle, i).ManagedString;
                    string value = Library.clang_HTMLStartTag_getAttrValue(_handle, i).ManagedString;
                    attribs.Add(new Tuple<string, string>(name, value));
                }
                return attribs;
            }
        }

        public string BlockCommandName
        {
            get { return Library.clang_BlockCommandComment_getCommandName(_handle).ManagedString; }
        }

        public IEnumerable<string> BlockCommandArguments
        {
            get
            {
                List<string> args = new List<string>();

                for (uint i = 0; i < Library.clang_BlockCommandComment_getNumArgs(_handle);i++)
                {
                    args.Add(Library.clang_BlockCommandComment_getArgText(_handle, i).ManagedString);
                }
                return args;
            }
        }

        public Comment BlockCommandParagraph
        {
            get 
            { 
                Library.CXComment c = Library.clang_BlockCommandComment_getParagraph(_handle);
                return new Comment(c);
            }
        }

        public string ParamCommandName
        {
            get { return Library.clang_ParamCommandComment_getParamName(_handle).ManagedString; }
        }

        public bool ParamCommandIsIndexValid
        {
            get { return Library.clang_ParamCommandComment_isParamIndexValid(_handle) != 0; }
        }

        public uint ParamCommandIndex
        {
            get { return Library.clang_ParamCommandComment_getParamIndex(_handle); }
        }

        public bool ParamCommandIsDirectionExplicit
        {
            get { return Library.clang_ParamCommandComment_isDirectionExplicit(_handle) != 0; }
        }

        public CommentParamPassDirection  ParamCommandDirection
        {
            get { return Library.clang_ParamCommandComment_getDirection(_handle); }
        }

        public string TParamCommandName
        {
            get { return Library.clang_TParamCommandComment_getParamName(_handle).ManagedString; }
        }

        public bool TParamCommandIsPositionValid
        {
            get { return Library.clang_TParamCommandComment_isParamPositionValid(_handle) != 0; }
        }
        
        public uint TParamCommandDepth
        {
            get { return Library.clang_TParamCommandComment_getDepth(_handle); }
        }
        
        public uint GetTParamCommandIndex(uint depth)
        {
            return Library.clang_TParamCommandComment_getIndex(_handle, depth);
        }
        
        public string VerbatimBlockLineText
        {
            get { return Library.clang_VerbatimBlockLineComment_getText(_handle).ManagedString; }
        }
        
        public string VerbatimLineText
        {
            get { return Library.clang_VerbatimLineComment_getText(_handle).ManagedString;}
        }

        public string HTMLTagAsString
        {
            get {return Library.clang_HTMLTagComment_getAsString(_handle).ManagedString;}
        }
                
        public string FullCommendAsHTML
        {
            get {return Library.clang_FullComment_getAsHTML(_handle).ManagedString;}
        }

        public string FullCommendAsXML
        {
            get {return Library.clang_FullComment_getAsXML(_handle).ManagedString;}
        }

        #endregion
    }
}
