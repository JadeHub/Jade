using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibClang;

namespace CppCodeBrowser.Symbols
{
    public enum TemplateKind
    {
        NonTemplate = 0,
        Template = 1,
        TemplatePartialSpecialization = 2,
        TemplateSpecialization = 3
    }

    public class ClassDecl : DeclarationBase
    {
        public IDeclaration _parent;
        private TemplateKind _templateKind;

        public ClassDecl(Cursor c, ISymbolTable table)
            : base(c, table)
        {
            Debug.Assert(CursorKinds.IsClassStructEtc(c.Kind));
    
            _templateKind = Symbols.TemplateKind.NonTemplate;
            if (c.Kind == CursorKind.ClassTemplate)
                _templateKind = Symbols.TemplateKind.Template;
            else if (c.Kind == CursorKind.ClassTemplatePartialSpecialization)
                _templateKind = Symbols.TemplateKind.TemplatePartialSpecialization;
            else if (c.TemplateSpecialisedCursorTemplate != null)
                _templateKind = Symbols.TemplateKind.TemplateSpecialization;

            if(c.SemanticParentCurosr.Kind == CursorKind.Namespace)
            {
                _parent = table.FindNamespaceDeclaration(c.SemanticParentCurosr.Usr);
                Debug.Assert(_parent != null);
            }
            else if (c.SemanticParentCurosr.Kind == CursorKind.TranslationUnit ||
                c.SemanticParentCurosr.Kind == CursorKind.UnexposedDecl)
            {
                _parent = null;
            }
            else if(CursorKinds.IsClassStructEtc(c.SemanticParentCurosr.Kind))
            {
                _parent = table.FindClassDeclaration(c.SemanticParentCurosr.Usr);
                Debug.Assert(_parent != null);
            }
            else
            {
                Debug.Assert(false);
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

        public TemplateKind TemplateKind { get { return _templateKind; } }

        public IDeclaration Parent { get { return _parent; } }
        public bool IsStruct { get { return Cursor.Kind == CursorKind.StructDecl; } }
        
        public IEnumerable<MethodDecl> Methods
        {
            get 
            {
                return from m in Table.Methods where m.Class == this select m;
            }
        }

        public IEnumerable<ConstructorDecl> Constructors
        {
            get
            {
                return from c in Table.Constructors where c.Class == this select c;
            }
        }

        public IEnumerable<ClassDecl> BaseClasses
        {
            get 
            {
                return new List<ClassDecl>();
            }
        }

        public IEnumerable<VariableDecl> DataMembers
        {
            get
            {
                return from v in Table.Variables where v.Parent != null && v.Parent == this select v;
            }
        }
    }
}
