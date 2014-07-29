using System;
using System.Diagnostics;
using System.Collections.Generic;
using LibClang;

namespace CppCodeBrowser.Symbols
{
    public interface INamespaceTable
    {
        IEnumerable<NamespaceDecl> Namespaces { get; }
        IEnumerable<ClassDecl> Classes { get; }

        
    }

    public class NamespaceTable : INamespaceTable
    {
        private IDictionary<string, ClassDecl> _classes;
        private IDictionary<string, NamespaceDecl> _namespaces;

        public NamespaceTable()
        {
            _namespaces = new Dictionary<string, NamespaceDecl>();
            _classes = new Dictionary<string, ClassDecl>();
        }

        public IEnumerable<NamespaceDecl> Namespaces { get {return _namespaces.Values; } }
        public IEnumerable<ClassDecl> Classes { get {return _classes.Values; } }
        /*
        public void Load(Cursor c)
        {            
            foreach(Cursor child in c.Children)
            {
                if(child.Kind == CursorKind.Namespace)
                {
                    _namespaces.Add(child.Usr, new NamespaceDecl(child));
                }
                else if(child.Kind == CursorKind.ClassDecl)
                {
                    _classes.Add(child.Usr, new ClassDecl(child));
                }
            }
        }
         * */
    }

    public class NamespaceDecl : DeclarationBase//, INamespaceTable
    {
        private NamespaceDecl _parent;

        public NamespaceDecl(Cursor c, ISymbolTable table)
            : base(c, table)
        {
            Debug.Assert(c.Kind == CursorKind.Namespace);

            if(c.SemanticParentCurosr.Kind == CursorKind.Namespace)
            {
                _parent = table.FindNamespace(c.SemanticParentCurosr.Usr);
                Debug.Assert(_parent != null);
            }
        }

        public override string Name { get { return Cursor.Spelling; } }
        public override EntityKind Kind { get { return EntityKind.Namespace; } }

        public NamespaceDecl ParentNamespace { get { return _parent; } }

        //class. structs etc        
        //function, vars
        //namespaces
        /*
        #region INamespaceTable
        
        public IEnumerable<NamespaceDecl> Namespaces { get { return _namespaceTable.Namespaces; } }
        public IEnumerable<ClassDecl> Classes { get { return _namespaceTable.Classes; } }
        
        #endregion
         * */
    }
}
