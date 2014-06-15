using System;
using System.Linq;
using System.Collections.Generic;
using LibClang;

namespace JadeCore.CppSymbols
{
    public class ClassDeclarationSymbol : SymbolCursorBase
    {
        private List<MethodDeclarationSymbol> _methods;
        private List<ConstructorDeclarationSymbol> _constructors;
        private List<ClassDeclarationSymbol> _baseClasses;
        private List<DataMemberDeclarationSymbol> _dataMembers;

        public ClassDeclarationSymbol(Cursor cur)
            : base(cur)
        {
        }

        public string Name
        {
            get { return Cursor.Spelling; }
        }

        public bool IsStruct
        {
            get { return Cursor.Kind == CursorKind.StructDecl; }
        }

        public bool IsClass
        {
            get { return !IsStruct; }
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

        public IEnumerable<ClassDeclarationSymbol> BaseClasses
        {
            get 
            { 
                if(_baseClasses == null)
                {
                    _baseClasses = new List<ClassDeclarationSymbol>(
                        from c in Cursor.Children
                               where c.Kind == CursorKind.CXXBaseSpecifier
                               select
                                   (ClassDeclarationSymbol)JadeCore.Services.Provider.SymbolCursorFactory.Create(c.CursorReferenced)      
                                   //make new type with symbol & access / virtual
                        );
                }

                return _baseClasses; 
            }
        }

        public IEnumerable<DataMemberDeclarationSymbol> DataMembers
        {
            get
            {
                if (_dataMembers == null)
                    _dataMembers = new List<DataMemberDeclarationSymbol>(GetType<DataMemberDeclarationSymbol>(LibClang.CursorKind.FieldDecl));
                return _dataMembers;
            }
        }
    }
}
