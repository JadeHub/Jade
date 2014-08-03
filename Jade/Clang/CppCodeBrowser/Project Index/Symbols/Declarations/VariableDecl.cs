﻿using System;
using System.Diagnostics;
using System.Collections.Generic;
using LibClang;

namespace CppCodeBrowser.Symbols
{
    public class VariableDecl : DeclarationBase
    {
        public IDeclaration _parent;
        private Cursor _definition;

        public VariableDecl(Cursor c, ISymbolTable table)
            : base(c, table)
        {
            Debug.Assert(c.Kind == CursorKind.VarDecl);
            if (c.SemanticParentCurosr.Kind == CursorKind.Namespace)
            {
                _parent = table.FindNamespace(c.SemanticParentCurosr.Usr);
                Debug.Assert(_parent != null);
            }
            else if (CursorKinds.IsClassStructEtc(c.SemanticParentCurosr.Kind))
            {
                _parent = table.FindClass(c.SemanticParentCurosr.Usr);
                Debug.Assert(_parent != null);
            }
            else if (c.SemanticParentCurosr.Kind == CursorKind.TranslationUnit)
            {
                _parent = null;
            }
            else
            {
                int p = 0;
            }
        }

        public override string Name { get { return Cursor.Spelling; } }
        public override EntityKind Kind { get { return EntityKind.Variable; } }

        public IDeclaration Parent { get { return _parent; } }

        public void UpdateDefinition(Cursor c)
        {
            Debug.Assert(c.Kind == CursorKind.VarDecl);
            Debug.Assert(c.IsDefinition);
            if (_definition == null)
                _definition = c;
        }
    }
}
