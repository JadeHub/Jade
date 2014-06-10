using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibClang
{
    public sealed class Token
    {
        #region Data

        internal Library.Token Handle
        {
            get;
            private set;
        }

        private readonly TranslationUnit _tu;
        private string _spelling;
        private SourceLocation _location;
        private SourceRange _extent;

        #endregion

        internal Token(Library.Token handle, TranslationUnit tu)
        {
            _tu = tu;
            Handle = handle;
        }

        #region Properties

        public TokenKind Kind
        {
            get { return Library.clang_getTokenKind(Handle); }
        }

        public string Spelling
        {
            get { return _spelling ?? (_spelling = Library.clang_getTokenSpelling(_tu.Handle, Handle).ManagedString); }        
        }

        private SourceLocation CreateLocation()
        {
            Debug.Assert(_tu.Handle != IntPtr.Zero);
            return _tu.ItemFactory.CreateSourceLocation(Library.clang_getTokenLocation(_tu.Handle, Handle));
        }

        public SourceLocation Location
        {
            get { return _location ??(_location = CreateLocation()); }
        }

        private SourceRange CreateExtent()
        {
            Debug.Assert(_tu.Handle != IntPtr.Zero);
            return _tu.ItemFactory.CreateSourceRange(Library.clang_getTokenExtent(_tu.Handle, Handle));
        }

        public SourceRange Extent
        {
            get { return _extent ?? (_extent = CreateExtent()); }
        }

        #endregion
    }
}
