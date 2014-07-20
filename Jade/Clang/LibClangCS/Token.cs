using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibClang
{
    /// <summary>
    /// Wrapper around libclang's Token type.
    /// 
    /// Describes a single preprocessing token.
    /// </summary>
    public sealed class Token
    {        
        internal Token(Library.CXToken handle, TranslationUnit tu)
        {
            //we dont save the handle here as it will be destroyed when the CXTokenSet id disposed
            Kind = Library.clang_getTokenKind(handle);
            Spelling = Library.clang_getTokenSpelling(tu.Handle, handle).ManagedString;
            Location = tu.ItemFactory.CreateSourceLocation(Library.clang_getTokenLocation(tu.Handle, handle));
            Extent = tu.ItemFactory.CreateSourceRange(Library.clang_getTokenExtent(tu.Handle, handle));
        }

        #region Properties

        /// <summary>
        /// Returns the Token's kind
        /// </summary>
        public TokenKind Kind
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns the Token's spelling
        /// </summary>
        public string Spelling
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns the Token's location
        /// </summary>
        public SourceLocation Location
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns the Token's extent.
        /// </summary>
        public SourceRange Extent
        {
            get;
            private set;
        }

        #endregion
    }
}
