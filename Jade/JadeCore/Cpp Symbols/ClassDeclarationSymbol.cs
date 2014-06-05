using System;
using System.Collections.Generic;
using LibClang;

namespace JadeCore.CppSymbols
{
    public class ClassDeclarationSymbol : SymbolCursorBase
    {
        private List<MethodDeclarationSymbol> _methods;
        private List<ConstructorDeclarationSymbol> _constructors;

        public ClassDeclarationSymbol(Cursor cur)
            : base(cur)
        { }

        public string Name
        {
            get { return Cursor.Spelling; }
        }

        public IEnumerable<MethodDeclarationSymbol> Methods
        {
            get 
            {
                if (_methods == null)
                    _methods = new List<MethodDeclarationSymbol>(GetType<MethodDeclarationSymbol>(LibClang.CursorKind.CXXMethod));
                return _methods; 
            }
        }

        public IEnumerable<ConstructorDeclarationSymbol> Constructors
        {
            get
            {
                if (_constructors == null)
                    _constructors = new List<ConstructorDeclarationSymbol>(GetType<ConstructorDeclarationSymbol>(LibClang.CursorKind.Constructor));
                return _constructors;
            }
        }
    }
}
