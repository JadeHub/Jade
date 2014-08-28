using System;
using System.Collections.Generic;
using System.Diagnostics;
using LibClang;
using JadeUtils.IO;

namespace CppCodeBrowser.Symbols
{
    public class ProjectSymbolTable : ISymbolTable
    {
        private SymbolSet<NamespaceDecl> _namespaces;
        private SymbolSet<ClassDecl> _classes;
        private SymbolSet<MethodDecl> _methods;
        private SymbolSet<EnumDecl> _enums;
        private SymbolSet<EnumConstantDecl> _enumConstants;
        private SymbolSet<ConstructorDecl> _constructors;
        private SymbolSet<DestructorDecl> _destructors;
        private SymbolSet<FieldDecl> _fields;
        private SymbolSet<FunctionDecl> _functions;
        private SymbolSet<TypedefDecl> _typedefs;
        private SymbolSet<VariableDecl> _variables;
        private SymbolSet<IncludeDecl> _includes;

        private FileMapping.IProjectFileMaps _fileSymbolMaps;

        private Dictionary<Tuple<ICodeLocation, int>, IReference> _refs = new Dictionary<Tuple<ICodeLocation, int>, IReference>();

        public ProjectSymbolTable(FileMapping.IProjectFileMaps fileSymbolMaps)
        {
            _fileSymbolMaps = fileSymbolMaps;

            _namespaces = new SymbolSet<NamespaceDecl>(delegate(Cursor c) { return new NamespaceDecl(c, this); });
            _classes = new SymbolSet<ClassDecl>(delegate(Cursor c) { return new ClassDecl(c, this); });
            _methods = new SymbolSet<MethodDecl>(delegate(Cursor c) { return new MethodDecl(c, this); });
            _enums = new SymbolSet<EnumDecl>(delegate(Cursor c) { return new EnumDecl(c, this); });
            _enumConstants = new SymbolSet<EnumConstantDecl>(delegate(Cursor c) { return new EnumConstantDecl(c, this); });
            _constructors = new SymbolSet<ConstructorDecl>(delegate(Cursor c) { return new ConstructorDecl(c, this); });
            _destructors = new SymbolSet<DestructorDecl>(delegate(Cursor c) { return new DestructorDecl(c, this); });
            _fields = new SymbolSet<FieldDecl>(delegate(Cursor c) { return new FieldDecl(c, this); });
            _functions = new SymbolSet<FunctionDecl>(delegate(Cursor c) { return new FunctionDecl(c, this); });
            _typedefs = new SymbolSet<TypedefDecl>(delegate(Cursor c) { return new TypedefDecl(c, this); });
            _variables = new SymbolSet<VariableDecl>(delegate(Cursor c) { return new VariableDecl(c, this); });
            _includes = new SymbolSet<IncludeDecl>(delegate(Cursor c) { return new IncludeDecl(c, this); });
        }

        public ISymbolSet<ClassDecl> Classes { get { return _classes; } }
        public ISymbolSet<NamespaceDecl> Namespaces { get { return _namespaces; } }
        public ISymbolSet<MethodDecl> Methods { get { return _methods; } }
        public ISymbolSet<EnumDecl> Enums { get { return _enums; } }
        public ISymbolSet<EnumConstantDecl> EnumConstants { get { return _enumConstants; } }
        public ISymbolSet<ConstructorDecl> Constructors { get { return _constructors; } }
        public ISymbolSet<DestructorDecl> Destructors { get { return _destructors; } }
        public ISymbolSet<FieldDecl> Fields { get { return _fields; } }
        public ISymbolSet<FunctionDecl> Functions { get { return _functions; } }
        public ISymbolSet<TypedefDecl> Typedefs { get { return _typedefs; } }
        public ISymbolSet<VariableDecl> Variables { get { return _variables; } }
        public ISymbolSet<IncludeDecl> Includes { get { return _includes; } }

        public NamespaceDecl FindNamespaceDeclaration(string usr)
        {
            return _namespaces.Find(usr);
        }
                
        public ClassDecl FindClassDeclaration(string usr)
        {
            return _classes.Find(usr);
        }

        public MethodDecl FindMethodDeclaration(string usr)
        {
            return _methods.Find(usr);
        }

        public EnumDecl FindEnumDeclaration(string usr)
        {
            return _enums.Find(usr);
        }

        public EnumConstantDecl FindEnumConstantDeclaration(string usr)
        {
            return _enumConstants.Find(usr);
        }

        public ConstructorDecl FindConstructorDeclaration(string usr)
        {
            return _constructors.Find(usr);
        }

        public DestructorDecl FindDestructorDeclaration(string usr)
        {
            return _destructors.Find(usr);
        }

        public FieldDecl FindFieldDeclaration(string usr)
        {
            return _fields.Find(usr);
        }

        public FunctionDecl FindFunctionDeclaration(string usr)
        {
            return _functions.Find(usr);
        }

        public TypedefDecl FindTypedefDeclaration(string usr)
        {
            return _typedefs.Find(usr);
        }

        public VariableDecl FindVariableDeclaration(string usr)
        {
            return _variables.Find(usr);
        }

        public IncludeDecl FindIncludeDeclaration(string usr)
        {
            return _includes.Find(usr);
        }

        private IDeclaration FindDeclaration(Cursor c)
        {
            if (c.Kind == CursorKind.Namespace)
            {
                return FindNamespaceDeclaration(c.Usr);
            }
            else if (CursorKinds.IsClassStructEtc(c.Kind))
            {
                return FindClassDeclaration(c.Usr);
            }
            else if (c.Kind == CursorKind.CXXMethod)
            {
                return FindMethodDeclaration(c.Usr);
            }
            else if (c.Kind == CursorKind.EnumConstantDecl)
            {
                return FindEnumConstantDeclaration(c.Usr);
            }
            else if (c.Kind == CursorKind.EnumDecl)
            {
                return FindEnumDeclaration(c.Usr);
            }
            else if (c.Kind == LibClang.CursorKind.Constructor)
            {
                return FindConstructorDeclaration(c.Usr);
            }
            else if (c.Kind == LibClang.CursorKind.Destructor)
            {
                return FindDestructorDeclaration(c.Usr);
            }
            else if (c.Kind == LibClang.CursorKind.FieldDecl)
            {
                return FindFieldDeclaration(c.Usr);
            }
            else if (c.Kind == LibClang.CursorKind.FunctionDecl)
            {
                return FindFunctionDeclaration(c.Usr);
            }
            else if (c.Kind == LibClang.CursorKind.VarDecl || c.Kind == LibClang.CursorKind.ParamDecl)
            {
                return FindVariableDeclaration(c.Usr);
            }
            else if(c.Kind == CursorKind.InclusionDirective)
            {
                return FindIncludeDeclaration(c.Usr);
            }
            return null;
        }
                
        public void UpdateReference(Cursor c)
        {
            if (c.Kind == CursorKind.CXXBaseSpecifier) return;

            Debug.Assert(c.CursorReferenced != null);
            IDeclaration decl = FindDeclaration(c.CursorReferenced);

            if(decl == null)
            {
                UpdateDefinition(c.CursorReferenced);
                decl = FindDeclaration(c.CursorReferenced);
            }
            if (decl != null)
            {                
                UpdateReference(c, decl);
            }
        }

        private void UpdateReference(Cursor c, IDeclaration decl)
        {
            Reference refer = new Reference(c, decl, this);
            UpdateSymbolMapping(refer);
        }

        public void UpdateDefinition(Cursor c)
        {        
            if (c.SemanticParentCurosr != null)
            {
                if (CursorKinds.IsClassStructEtc(c.SemanticParentCurosr.Kind))
                {
                    UpdateDefinition(c.SemanticParentCurosr);
                }
                else if (c.SemanticParentCurosr.Kind == CursorKind.EnumDecl)
                {
                    UpdateDefinition(c.SemanticParentCurosr);
                }
            }

            if (c.Kind == CursorKind.Namespace)
            {
                UpdateDeclaration(_namespaces, c);
            }
            else if (CursorKinds.IsClassStructEtc(c.Kind))
            {
                UpdateDeclaration(_classes, c);
            }
            else if (c.Kind == CursorKind.CXXMethod)
            {
                if(CursorKinds.IsClassStructEtc(c.SemanticParentCurosr.Kind))
                    UpdateDeclaration(_methods, c);
            }
            else if (c.Kind == CursorKind.EnumConstantDecl)
            {
                UpdateDeclaration(_enumConstants, c);
            }
            else if (c.Kind == CursorKind.EnumDecl)
            {
                UpdateDeclaration(_enums, c);
            }
            else if (c.Kind == LibClang.CursorKind.Constructor)
            {
                UpdateDeclaration(_constructors, c);
            }
            else if (c.Kind == LibClang.CursorKind.Destructor)
            {
                UpdateDeclaration(_destructors, c);
            }
            else if (c.Kind == LibClang.CursorKind.FieldDecl)
            {
                if(CursorKinds.IsClassStructEtc(c.SemanticParentCurosr.Kind))
                    UpdateDeclaration(_fields, c);
            }
            else if (c.Kind == LibClang.CursorKind.FunctionDecl)
            {
                UpdateDeclaration(_functions, c);
            }
            else if (c.Kind == LibClang.CursorKind.VarDecl)
            {
                UpdateDeclaration(_variables, c);
            }
            else if (c.Kind == LibClang.CursorKind.ParamDecl)
            {
                if(c.Usr.Length > 0)
                    UpdateDeclaration(_variables, c);
            }
            else if(c.Kind == CursorKind.InclusionDirective)
            {
                UpdateIncludeDecl(c);
            }
        }

        private void UpdateSymbolMapping(ISymbol symbol)
        {
            _fileSymbolMaps.UpdateDeclarationMapping(symbol.Location.Path,  symbol.Location.Offset, symbol.Location.Offset + symbol.SpellingLength, symbol);
        }

        private void UpdateSymbolMapping(ICodeLocation startLoc, int length, ISymbol decl)
        {
            _fileSymbolMaps.UpdateDeclarationMapping(startLoc.Path, startLoc.Offset, startLoc.Offset + length, decl);
        }

        private void UpdateDeclaration<T>(SymbolSet<T> set, Cursor c) where T : ISymbol
        {
            var result = set.FindOrAdd(c);
            if (result.Item1)
            {
                UpdateSymbolMapping(result.Item2);
            }
            if (result.Item2 is IHasDefinition && c.IsDefinition)
            {
                (result.Item2 as IHasDefinition).UpdateDefinition(c);
                UpdateSymbolMapping(new CodeLocation(c.Location), c.Spelling.Length, result.Item2);
            }
        }

        private void UpdateIncludeDecl(Cursor c)
        {
            FilePath includedPath = FilePath.Make(c.IncludedFile.Name);
            string usr = includedPath.Str;
            IncludeDecl include = FindIncludeDeclaration(usr);
            if(include == null)
            {
                include = new IncludeDecl(c, this);
                _includes.Add(usr, include);
            }
            UpdateReference(c, include);
        }
    }
}
