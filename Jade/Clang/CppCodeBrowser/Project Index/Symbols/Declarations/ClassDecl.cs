using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibClang;

namespace CppCodeBrowser.Symbols
{
    public class ClassDecl : DeclarationBase
    {
        public IDeclaration _parent;

        public ClassDecl(Cursor c, ISymbolTable table)
            : base(c, table)
        {
            Debug.Assert(CursorKinds.IsClassStructEtc(c.Kind));
            if(c.SemanticParentCurosr.Kind == CursorKind.Namespace)
            {
                _parent = table.FindNamespace(c.SemanticParentCurosr.Usr);
                Debug.Assert(_parent != null);
            }
            else if(c.SemanticParentCurosr.Kind == CursorKind.TranslationUnit)
            {
                _parent = null;
            }
            else
            {
                int p = 0;
            }
        }
        
        public override string Name { get { return Cursor.Spelling; } }
        public override EntityKind Kind 
        { 
            get 
            {
                if (Cursor.Kind == CursorKind.ClassDecl)
                    return EntityKind.Class; 
                
                return EntityKind.Struct; 
            }
        }

        public IDeclaration Parent { get { return _parent; } }
        public bool IsStruct { get { return Cursor.Kind == CursorKind.StructDecl; } }
    }
}
