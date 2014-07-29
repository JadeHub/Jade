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

    public interface ISymbolSet<T> where T : ISymbol
    {
        IEnumerable<T> Symbols { get; }
        void Update(T item);
        T Find(string usr);
    }

    public class SymbolSet<T> : ISymbolSet<T> where T : ISymbol
    {
        private IDictionary<string, T> _symbols;

        public SymbolSet()
        {
            _symbols = new Dictionary<string, T>();
        }

        public IEnumerable<T> Symbols { get { return _symbols.Values; } }

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
    }

    public interface ISymbolTable
    {
        ISymbolSet<NamespaceDecl> Namespaces { get; }
        ISymbolSet<ClassDecl> Classes { get; }
        ISymbolSet<MethodDecl> Methods { get; }

        NamespaceDecl FindNamespace(string usr);
        ClassDecl FindClass(string usr);
        MethodDecl FindMethod(string usr);

        //vars
        //functions

        void UpdateDefinition(Cursor c);
    }

    public class ProjectSymbolTable : ISymbolTable
    {
        private SymbolSet<NamespaceDecl> _namespaces;
        private SymbolSet<ClassDecl> _classes;
        private SymbolSet<MethodDecl> _methods;

        public ProjectSymbolTable()
        {
            _namespaces = new SymbolSet<NamespaceDecl>();
            _classes = new SymbolSet<ClassDecl>();
            _methods = new SymbolSet<MethodDecl>();
        }

        public ISymbolSet<ClassDecl> Classes { get { return _classes; } }
        public ISymbolSet<NamespaceDecl> Namespaces { get { return _namespaces; } }
        public ISymbolSet<MethodDecl> Methods { get { return _methods; } }

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
            else if(c.Kind  == LibClang.CursorKind.Constructor)
            {

            }
            else if(c.Kind  == LibClang.CursorKind.Destructor)
            {
            }
            else if (c.Kind == LibClang.CursorKind.FieldDecl)
            {
            }
            else if (c.Kind == LibClang.CursorKind.FunctionDecl)
            {
            }
            else if (c.Kind == LibClang.CursorKind.VarDecl)
            {
            }
        }

        private void UpdateNamespaceDecl(Cursor c)
        {
            NamespaceDecl decl = FindOrAddNamespaceDecl(c);
                        
        }

        private void UpdateClassDecl(Cursor c)
        {
            ClassDecl classDecl = FindOrAddClassDecl(c);
                        
        }

        private void UpdateMethodDecl(Cursor c)
        {
            MethodDecl method = FindOrAddMethodDecl(c);
            if (c.IsDefinition)
                method.SetDefinition(c);
            
        }

        private NamespaceDecl FindOrAddNamespaceDecl(Cursor c)
        {
            NamespaceDecl result = _namespaces.Find(c.Usr);
            if (result != null) return result;
            result = new NamespaceDecl(c, this);
            _namespaces.Update(result);
            return result;
        }

        private ClassDecl FindOrAddClassDecl(Cursor c)
        {
            ClassDecl result = _classes.Find(c.Usr);
            if (result != null) return result;
            result = new ClassDecl(c, this);
            _classes.Update(result);
            return result;
        }

        private MethodDecl FindOrAddMethodDecl(Cursor c)
        {
            MethodDecl result = _methods.Find(c.Usr);
            if (result != null) return result;
            result = new MethodDecl(c, this);
            _methods.Update(result);
            return result;
        }
    }
}
