using System;
using System.Diagnostics;
using System.Collections.Generic;
using LibClang;

namespace CppCodeBrowser.Symbols
{
    public class VariableDecl : DeclarationBase, IHasDefinition
    {
        public IDeclaration _parent;
        private Cursor _definition;

        public VariableDecl(Cursor c, ISymbolTable table)
            : base(c, table)
        {
           // Debug.Assert(c.Kind == CursorKind.VarDecl);
            if (c.SemanticParentCurosr.Kind == CursorKind.Namespace)
            {
                _parent = table.FindNamespaceDeclaration(c.SemanticParentCurosr.Usr);
                Debug.Assert(_parent != null);
            }
            else if (CursorKinds.IsClassStructEtc(c.SemanticParentCurosr.Kind))
            {
                _parent = table.FindClassDeclaration(c.SemanticParentCurosr.Usr);
                Debug.Assert(_parent != null);
            }
            else if(CursorKinds.IsFunctionEtc(c.SemanticParentCurosr.Kind))
            {
                _parent = table.FindFunctionDeclaration(c.SemanticParentCurosr.Usr);
            }
            else if (c.SemanticParentCurosr.Kind == CursorKind.TranslationUnit ||
                c.SemanticParentCurosr.Kind == CursorKind.UnexposedDecl)
            {
                _parent = null;
            }
            else
            {
                Debug.Assert(false);
            }
        }

        public override string Name { get { return Cursor.Spelling; } }
        public override EntityKind Kind { get { return EntityKind.Variable; } }

        public IDeclaration Parent { get { return _parent; } }

        #region IHasDefinition

        public void UpdateDefinition(Cursor c)
        {
         //   Debug.Assert(c.Kind == CursorKind.VarDecl);
            Debug.Assert(c.IsDefinition);
            if (_definition == null)
                _definition = c;
        }

        public Cursor DefinitionCursor
        {
            get { return _definition; }
        }

        public bool HasDefinitionCursor { get { return _definition != null; } }

        #endregion
    }
}
