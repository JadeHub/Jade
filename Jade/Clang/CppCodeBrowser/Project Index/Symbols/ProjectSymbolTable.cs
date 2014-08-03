using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        private FileMapping.IProjectFileMaps _fileSymbolMaps;

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

        public NamespaceDecl FindNamespace(string usr)
        {
            return _namespaces.Find(usr);
        }

        public ClassDecl FindClass(string usr)
        {
            return _classes.Find(usr);
        }

        public MethodDecl FindMethod(string usr)
        {
            return _methods.Find(usr);
        }

        public EnumDecl FindEnum(string usr)
        {
            return _enums.Find(usr);
        }

        public void UpdateDefinition(Cursor c)
        {
            if (c.Kind == CursorKind.Namespace)
            {
                UpdateNamespaceDecl(c);
            }
            else if (CursorKinds.IsClassStructEtc(c.Kind))
            {
                UpdateClassDecl(c);
            }
            else if (c.Kind == CursorKind.CXXMethod)
            {
                if(CursorKinds.IsClassStructEtc(c.SemanticParentCurosr.Kind))
                    UpdateMethodDecl(c);
            }
            else if (c.Kind == CursorKind.EnumConstantDecl)
            {
                UpdateEnumConstantDecl(c);
            }
            else if (c.Kind == CursorKind.EnumDecl)
            {
                UpdateEnumDecl(c);
            }
            else if (c.Kind == CursorKind.EnumConstantDecl)
            {
                UpdateEnumConstantDecl(c);
            }
            else if (c.Kind == LibClang.CursorKind.Constructor)
            {
                UpdateConstrctorDecl(c);
            }
            else if (c.Kind == LibClang.CursorKind.Destructor)
            {
                UpdateDestrctorDecl(c);
            }
            else if (c.Kind == LibClang.CursorKind.FieldDecl)
            {
                if(CursorKinds.IsClassStructEtc(c.SemanticParentCurosr.Kind))
                    UpdateFieldDecl(c);
            }
            else if (c.Kind == LibClang.CursorKind.FunctionDecl)
            {
                UpdateFunctionDecl(c);
            }
            else if (c.Kind == LibClang.CursorKind.VarDecl)
            {
                UpdateVariableDecl(c);
            }
        }

        private void UpdateDeclarationSymbolMapping(IDeclaration decl)
        {
            _fileSymbolMaps.UpdateDeclarationMapping(decl.Location.Path,  decl.Location.Offset, decl.Location.Offset + decl.SpellingLength, decl);
        }

        private void UpdateNamespaceDecl(Cursor c)
        {
            var result = _namespaces.FindOrAdd(c);
            if (result.Item1)
            {
                UpdateDeclarationSymbolMapping(result.Item2);
            }
        }

        private void UpdateClassDecl(Cursor c)
        {
            var result = _classes.FindOrAdd(c);
            if (result.Item1)
            {
                UpdateDeclarationSymbolMapping(result.Item2);
            }
        }

        private void UpdateMethodDecl(Cursor c)
        {
            var result = _methods.FindOrAdd(c);
            if (result.Item1)
            {
                UpdateDeclarationSymbolMapping(result.Item2);
            }
            if(c.IsDefinition)
            {
                result.Item2.UpdateDefinition(c);
                _fileSymbolMaps.UpdateDeclarationMapping(FilePath.Make(c.Location.File.Name),
                                                     c.Location.Offset,
                                                     c.Location.Offset + c.Spelling.Length,
                                                     result.Item2);
            }
        }

        private void UpdateEnumDecl(Cursor c)
        {
            var result = _enums.FindOrAdd(c);
            if (result.Item1)
            {
                UpdateDeclarationSymbolMapping(result.Item2);
            }
        }

        private void UpdateEnumConstantDecl(Cursor c)
        {
            var result = _enumConstants.FindOrAdd(c);
            if (result.Item1)
            {
                UpdateDeclarationSymbolMapping(result.Item2);
            }
        }

        private void UpdateConstrctorDecl(Cursor c)
        {
            var result = _constructors.FindOrAdd(c);
            if (result.Item1)
            {
                UpdateDeclarationSymbolMapping(result.Item2);
            }
            if (c.IsDefinition)
            {
                result.Item2.UpdateDefinition(c);
                _fileSymbolMaps.UpdateDeclarationMapping(FilePath.Make(c.Location.File.Name),
                                                     c.Location.Offset,
                                                     c.Location.Offset + c.Spelling.Length,
                                                     result.Item2);
            }
        }

        private void UpdateDestrctorDecl(Cursor c)
        {
            var result = _destructors.FindOrAdd(c);
            if (result.Item1)
            {
                UpdateDeclarationSymbolMapping(result.Item2);
            }
            if (c.IsDefinition)
            {
                result.Item2.UpdateDefinition(c);
                _fileSymbolMaps.UpdateDeclarationMapping(FilePath.Make(c.Location.File.Name),
                                                     c.Location.Offset,
                                                     c.Location.Offset + c.Spelling.Length,
                                                     result.Item2);
            }
        }

        private void UpdateFieldDecl(Cursor c)
        {
            var result = _fields.FindOrAdd(c);
            if (result.Item1)
            {
                UpdateDeclarationSymbolMapping(result.Item2);
            }
        }

        private void UpdateFunctionDecl(Cursor c)
        {
            var result = _functions.FindOrAdd(c);
            if (result.Item1)
            {
                UpdateDeclarationSymbolMapping(result.Item2);
            }
            if (c.IsDefinition)
            {
                result.Item2.UpdateDefinition(c);
                _fileSymbolMaps.UpdateDeclarationMapping(FilePath.Make(c.Location.File.Name),
                                                     c.Location.Offset,
                                                     c.Location.Offset + c.Spelling.Length,
                                                     result.Item2);
            }
        }

        private void UpdateVariableDecl(Cursor c)
        {
            var result = _variables.FindOrAdd(c);
            if (result.Item1)
            {
                UpdateDeclarationSymbolMapping(result.Item2);
            }
            if(c.IsDefinition)
            {
                result.Item2.UpdateDefinition(c);
                _fileSymbolMaps.UpdateDeclarationMapping(FilePath.Make(c.Location.File.Name),
                                                     c.Location.Offset,
                                                     c.Location.Offset + c.Spelling.Length,
                                                     result.Item2);
            }
        }
    }
}
