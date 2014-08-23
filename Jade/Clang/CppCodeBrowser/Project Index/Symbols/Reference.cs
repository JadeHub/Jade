using System;
using System.Collections.Generic;
using System.Diagnostics;
using LibClang;
using JadeUtils.IO;

namespace CppCodeBrowser.Symbols
{
    public interface IReference : ISymbol
    {
        IDeclaration Declaration { get; }
    }

    public class Reference : IReference
    {
        private Cursor _cursor;
        private ICodeLocation _location;

        private ISymbolTable _table;
        public IDeclaration _decl;

        public Reference(Cursor c, IDeclaration decl, ISymbolTable table)
        {
            _cursor = c;
            _decl = decl;
            _table = table;
            _location = new CodeLocation(c.Extent.Start); //todo - replace for doc tracking
        }

        public string Name { get { return _cursor.Spelling; } }
        public string Spelling { get { return Cursor.Spelling; } }
        public ICodeLocation Location { get { return _location; } }
        
        public int SpellingLength 
        { 
            get 
            {
                return _cursor.Extent.Length;

                //return _cursor.Extent.Length > 5 ? 5 :_cursor.Extent.Length;

             /*   if (Cursor.Kind == CursorKind.CallExpr)// && Cursor.Spelling == "Fn2")
                {
                    return _cursor.Extent.Length;
                }
                */
                /*
                var tok = _cursor.Extent.GetTokenAtOffset(_location.Offset);

                if (tok != null)
                    return tok.Spelling.Length;

                return _cursor.DisplayName.Length;*/
            }
        }


        public Cursor Cursor { get { return _cursor; } }

        public IDeclaration Declaration { get { return _decl; } }
    }


}
