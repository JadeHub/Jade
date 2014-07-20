using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LibClang
{
    /// <summary>
    /// A set of Token objects sorted by offset
    /// </summary>
    public class TokenSet : IEnumerable<Token>
    {
        private readonly List<Token> _tokens;      
        
        internal unsafe static TokenSet Create(TranslationUnit tu, SourceRange range)
        {
            Library.CXToken* tokenHandles;
            uint count = 0;

            Library.clang_tokenize(tu.Handle, range.Handle, &tokenHandles, &count);
            if (count == 0)
                return null;

            SortedList<int, Token> tokens = new SortedList<int, Token>();
            for(uint i = 0; i < count; i++)
            {                
                Token token = new Token(*(tokenHandles + i), tu);
                if(range.Contains(token.Location.Offset))
                    tokens.Add(token.Location.Offset, token);
            }
            Library.clang_disposeTokens(tu.Handle, tokenHandles, count);
            
            return new TokenSet(tokens.Values);
        }
        
        internal TokenSet(IEnumerable<Token> tokens)
        {
            _tokens = new List<Token>(tokens);
        }

        public Token GetTokenAtOffset(int offset)
        {
            foreach (Token t in _tokens)
            {
                if (t.Extent.Contains(offset))
                    return t;
            }
            return null;
        }

        public IEnumerator<Token> GetEnumerator()
        {
            return _tokens.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _tokens.GetEnumerator();
        }
    }
}
