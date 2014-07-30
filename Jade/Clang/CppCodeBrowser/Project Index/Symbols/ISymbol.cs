using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibClang;

namespace CppCodeBrowser.Symbols
{
    public interface IFileSymbolMap
    {
        void AddMapping(int startOffset, int endOffset, ISymbol referenced);
    }

    public interface ISymbol
    {
        string Name { get; }
        ICodeLocation Location { get; }
        Cursor Cursor { get; }
    }

    public interface ISymbolSet<T> : IEnumerable<T> where T : ISymbol
    {
        void Update(T item);
        T Find(string usr);
    }

    public class SymbolSet<T> : ISymbolSet<T> where T : ISymbol
    {
        private IDictionary<string, T> _symbols;
        private Func<Cursor, T> _creator;

        public SymbolSet(Func<Cursor, T> creator)
        {
            _creator = creator;
            _symbols = new Dictionary<string, T>();
        }

        #region IEnumerable<T>
        /// <summary>
        /// Gets an enumerator for this list.
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            return _symbols.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _symbols.Values.GetEnumerator();
        }
        #endregion


        public void Update(T item)
        {
            Cursor c = item.Cursor;
            Debug.Assert(c.Usr.Length > 0);
            _symbols.Add(c.Usr, item);
        }

        public T Find(string usr)
        {
            T result = default(T);
            _symbols.TryGetValue(usr, out result);
            return result;
        }

        public T FindOrAdd(Cursor c)
        {
            T result = Find(c.Usr);
            if (result != null) return result;
            result = _creator(c);
            Update(result);
            return result;
        }
    }

    public interface ISymbolTable
    {
        ISymbolSet<NamespaceDecl> Namespaces { get; }
        ISymbolSet<ClassDecl> Classes { get; }
        ISymbolSet<MethodDecl> Methods { get; }
        ISymbolSet<EnumDecl> Enums { get; }
        ISymbolSet<ConstructorDecl> Constructors { get; }
        ISymbolSet<DestructorDecl> Destructors { get; }
        ISymbolSet<FieldDecl> Fields { get; }
        ISymbolSet<FunctionDecl> Functions { get; }
        ISymbolSet<TypedefDecl> Typedefs { get; }
        ISymbolSet<VariableDecl> Variables { get; }

        NamespaceDecl FindNamespace(string usr);
        ClassDecl FindClass(string usr);
        MethodDecl FindMethod(string usr);


        void UpdateDefinition(Cursor c);
    }

    public class ProjectSymbolTable : ISymbolTable
    {
        private SymbolSet<NamespaceDecl> _namespaces;
        private SymbolSet<ClassDecl> _classes;
        private SymbolSet<MethodDecl> _methods;
        private SymbolSet<EnumDecl> _enums;
        private SymbolSet<ConstructorDecl> _constructors;
        private SymbolSet<DestructorDecl> _destructors;
        private SymbolSet<FieldDecl> _fields;
        private SymbolSet<FunctionDecl> _functions;
        private SymbolSet<TypedefDecl> _typedefs;
        private SymbolSet<VariableDecl> _variables;

        public ProjectSymbolTable()
        {
            _namespaces = new SymbolSet<NamespaceDecl>(delegate(Cursor c) { return new NamespaceDecl(c, this); });
            _classes = new SymbolSet<ClassDecl>(delegate(Cursor c) { return new ClassDecl(c, this); });
            _methods = new SymbolSet<MethodDecl>(delegate(Cursor c) { return new MethodDecl(c, this); });
            _enums = new SymbolSet<EnumDecl>(delegate(Cursor c) { return new EnumDecl(c, this); });
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
        public ISymbolSet<EnumDecl> Enums { get { return null; } }
        public ISymbolSet<ConstructorDecl> Constructors { get { return null; } }
        public ISymbolSet<DestructorDecl> Destructors { get { return null; } }
        public ISymbolSet<FieldDecl> Fields { get { return null; } }
        public ISymbolSet<FunctionDecl> Functions { get { return null; } }
        public ISymbolSet<TypedefDecl> Typedefs { get { return null; } }
        public ISymbolSet<VariableDecl> Variables { get { return null; } }

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

        public void UpdateDefinition(Cursor c)
        {
            if (c.Kind == CursorKind.Namespace)
            {
                UpdateNamespaceDecl(c);
            }
            else if (c.Kind == CursorKind.ClassDecl)
            {
                UpdateClassDecl(c);
            }
            else if(c.Kind == CursorKind.CXXMethod)
            {
                UpdateMethodDecl(c);
            }
            else if(c.Kind == CursorKind.EnumDecl)
            {
                UpdateEnumDecl(c);
            }
            else if(c.Kind  == LibClang.CursorKind.Constructor)
            {
                UpdateConstrctorDecl(c);
            }
            else if(c.Kind  == LibClang.CursorKind.Destructor)
            {
                UpdateDestrctorDecl(c);
            }
            else if (c.Kind == LibClang.CursorKind.FieldDecl)
            {
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

        private void UpdateNamespaceDecl(Cursor c)
        {
            _namespaces.FindOrAdd(c);
        }

        private void UpdateClassDecl(Cursor c)
        {
            _classes.FindOrAdd(c);                        
        }

        private void UpdateMethodDecl(Cursor c)
        {
            _methods.FindOrAdd(c);
        }

        private void UpdateEnumDecl(Cursor c)
        {
            _enums.FindOrAdd(c);
        }

        private void UpdateConstrctorDecl(Cursor c)
        {
            _constructors.FindOrAdd(c);
        }

        private void UpdateDestrctorDecl(Cursor c)
        {
            _destructors.FindOrAdd(c);
        }

        private void UpdateFieldDecl(Cursor c)
        {
            _fields.FindOrAdd(c);
        }

        private void UpdateFunctionDecl(Cursor c)
        {
            _functions.FindOrAdd(c);
        }

        private void UpdateVariableDecl(Cursor c)
        {
            _variables.FindOrAdd(c);
        }
    }
}
