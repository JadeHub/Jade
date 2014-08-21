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
            _location = new CodeLocation(c.Location); //todo - replace for doc tracking
        }

        public string Name { get { return _cursor.Spelling; } }
        public string Spelling { get { return Cursor.Spelling; } }
        public ICodeLocation Location { get { return _location; } }
        public int SpellingLength { get { return _cursor.Extent.Length; } }
        public Cursor Cursor { get { return _cursor; } }

        public IDeclaration Declaration { get { return _decl; } }
    }

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
            else if (c.Kind == LibClang.CursorKind.VarDecl)
            {
                return FindVariableDeclaration(c.Usr);
            }
            return null;
        }

        private Dictionary<ICodeLocation, IReference> _refs = new Dictionary<ICodeLocation, IReference>();

        public void UpdateReference(Cursor c)
        {
            Debug.Assert(c.CursorReferenced != null);
            IDeclaration decl = FindDeclaration(c.CursorReferenced);

            if(decl == null)
            {
                UpdateDefinition(c.CursorReferenced);
                decl = FindDeclaration(c.CursorReferenced);
            }
            if(decl != null)
                UpdateReference(c, decl);
        }

        private void UpdateReference(Cursor c, IDeclaration decl)
        {
            IReference existing = null;
            ICodeLocation loc = new CodeLocation(c.Location);

            if(_refs.TryGetValue(loc, out existing))
            {
                //Debug.Assert(existing.Declaration == decl);
                return;
            }
            Reference refer = new Reference(c, decl, this);
            if (refer.Location.Offset == 1257)
            {
                int i = 0;
            }
            _refs.Add(refer.Location, refer);
            UpdateSymbolMapping(refer);
        }

        HashSet<string> _defs = new HashSet<string>();

        public void UpdateDefinition(Cursor c)
        {
         /*   if (_defs.Contains(c.Usr))
                return;
            _defs.Add(c.Usr);*/

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

        private void UpdateSymbolMapping(ISymbol decl)
        {
            _fileSymbolMaps.UpdateDeclarationMapping(decl.Location.Path,  decl.Location.Offset, decl.Location.Offset + decl.SpellingLength, decl);
        }

        private void UpdateSymbolMapping(ICodeLocation startLoc, int length,  ISymbol decl)
        {
            _fileSymbolMaps.UpdateDeclarationMapping(startLoc.Path, startLoc.Offset, startLoc.Offset + length, decl);
        }

        private void UpdateNamespaceDecl(Cursor c)
        {
            var result = _namespaces.FindOrAdd(c);

            UpdateSymbolMapping(new CodeLocation(c.Location), c.Spelling.Length, result.Item2);

           // if (result.Item1)
            {
                //UpdateSymbolMapping(result.Item2);
            }
        }

        private void UpdateClassDecl(Cursor c)
        {
            if (c.Spelling == "Template2")
            {
                int i = 0;
            }
            var result = _classes.FindOrAdd(c);
            if (result.Item1)
            {
                UpdateSymbolMapping(result.Item2);
            }
        }

        private void UpdateMethodDecl(Cursor c)
        {
            var result = _methods.FindOrAdd(c);
            if (result.Item1)
            {
                UpdateSymbolMapping(result.Item2);
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
                UpdateSymbolMapping(result.Item2);
            }
        }

        private void UpdateEnumConstantDecl(Cursor c)
        {
            var result = _enumConstants.FindOrAdd(c);
            if (result.Item1)
            {
                UpdateSymbolMapping(result.Item2);
            }
        }

        private void UpdateConstrctorDecl(Cursor c)
        {
            if(c.Spelling == "Template2<T>")
            {
                int i = 0;
            }

            var result = _constructors.FindOrAdd(c);
            if (result.Item1)
            {
                UpdateSymbolMapping(result.Item2);
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
                UpdateSymbolMapping(result.Item2);
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
                UpdateSymbolMapping(result.Item2);
            }
        }

        private void UpdateFunctionDecl(Cursor c)
        {
            var result = _functions.FindOrAdd(c);
            if (result.Item1)
            {
                UpdateSymbolMapping(result.Item2);
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
                UpdateSymbolMapping(result.Item2);
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
