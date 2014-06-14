using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibClang
{
    internal class TokenSetHandle : IDisposable
    {
        private TranslationUnit _tu;
        private unsafe Library.Token* _tokens;
        private uint _count;
        /*
        internal static unsafe TokenSetHandle Create(TranslationUnit tu, SourceRange range)
        {
            Library.Token* tokens;
            uint count = 0;

            Library.clang_tokenize(tu.Handle, range.Handle, &tokens, &count);
            return count > 0 ? new TokenSetHandle(tu, tokens, count) : null;
        }*/

        internal unsafe TokenSetHandle(TranslationUnit tu, Library.Token* toks, uint count)
        {
            _tu = tu;
            _tokens = toks;
            _count = count;
        }

        public unsafe void Dispose()
        {
            if (_tokens == null)
                return;

            Library.clang_disposeTokens(_tu.Handle, _tokens, _count);

            _tokens = null;
        }

        public unsafe Library.Token GetToken(uint index)
        {
            if (index > _count)
                throw new ArgumentException("invalid index");
            return *(_tokens + index);
        }

        public uint Count { get { return _count; } }
    }

    public class TokenSet : IDisposable
    {
        internal TokenSetHandle Handle
        {
            get;
            private set;
        }

        private TranslationUnit _tu;
        private readonly IDictionary<int, Token> _tokens;

        public unsafe static TokenSet Create(TranslationUnit tu, SourceRange range)
        {
            Library.Token* tokens;
            uint count = 0;

            Library.clang_tokenize(tu.Handle, range.Handle, &tokens, &count);
            if (count == 0)
                return null;

            TokenSetHandle handle = new TokenSetHandle(tu, tokens, count);
            if (handle == null)
                return null;
            return new TokenSet(tu, handle, range);
        }
                        
        internal unsafe TokenSet(TranslationUnit tu, TokenSetHandle handle, SourceRange range)
        {
            _tu = tu;
            Handle = handle;
            _tokens = new SortedList<int, Token>();
            for (uint i = 0; i < handle.Count; i++)
            {
                Library.Token tokHandle = handle.GetToken(i);
                Token tok = new Token(tokHandle, _tu);
                if(range.Contains(tok.Location.Offset))
                    _tokens.Add(tok.Location.Offset, tok);
            }
        }

        public unsafe void Dispose()
        {
            Handle.Dispose();
        }

        public IEnumerable<Token> Tokens
        {
            get
            {
                return _tokens.Values;
            }
        }

        public Token GetTokenAtOffset(int offset)
        {
            foreach (Token t in _tokens.Values)
            {
                if (t.Extent.Contains(offset))
                    return t;
            }
            return null;
        }
    }
}
