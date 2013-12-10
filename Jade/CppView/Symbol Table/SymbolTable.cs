 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CppView
{
    using JadeUtils.IO;

    public interface ISymbolTable
    {
        bool Add(IDeclaration decl);
        bool Add(IReference refer);

        bool HasDeclaration(string usr);
        bool HasDeclaration(string usr, FilePath path, int offset);
        bool HasReference(string refedUSR, FilePath path, int offset);

        IDeclaration GetCanonicalDefinition(string usr);

        IEnumerable<IDeclaration> GetDeclarations(string usr);
        IEnumerable<string> DeclarationUSRs { get; }
        IEnumerable<IReference> References { get; }

        void Dump();
    }

    internal class SymbolTableImpl : ISymbolTable
    {
        #region Data

        private object _lock;

        //Usr to definitions
        private Dictionary<string, IList<IDeclaration>> _declarations;
        private HashSet<IReference> _references;

        #endregion

        internal SymbolTableImpl()
        {
            _lock = new object();
            _declarations = new Dictionary<string, IList<IDeclaration>>();
            _references = new HashSet<IReference>();
        }

        public bool Add(IDeclaration decl)
        {
            lock (_lock)
            {
                IList<IDeclaration> decls;

                if (_declarations.TryGetValue(decl.Usr, out decls))
                {
                    decls.Add(decl);
                    return true;
                }
                else
                {
                    decls = new List<IDeclaration>();
                    decls.Add(decl);
                    _declarations[decl.Usr] = decls;
                }
            }
            return true;
        }

        public bool Add(IReference refer)
        {
            lock (_lock)
            {
                return _references.Add(refer);
            }
        }

        public bool HasDeclaration(string usr)
        {
            lock (_lock)
            {
                return _declarations.ContainsKey(usr);
            }
        }

        public bool HasDeclaration(string usr, FilePath path, int offset)
        {
            lock (_lock)
            {
                foreach (IDeclaration decl in GetDeclarations(usr))
                {
                    if (decl.Location.Path == path && decl.Location.Offset == offset)
                        return true;
                }
            }
            return false;
        }

        public bool HasReference(string refedUSR, FilePath path, int offset)
        {
            lock (_lock)
            {
                foreach (IReference r in References)
                {
                    if (r.ReferencedUSR == refedUSR && r.Location.Offset == offset && r.Location.Path == path)
                        return true;
                }
            }
            return false;
        }

        public IDeclaration GetCanonicalDefinition(string usr)
        {
            lock (_lock)
            {
                foreach (IDeclaration d in GetDeclarations(usr))
                {
                    if(d.Cursor == d.Cursor.CanonicalCursor)
                        return d;
                }
            }
            return null;
        }

        public IEnumerable<IDeclaration> GetDeclarations(string usr)
        {
            lock (_lock)
            {
                IList<IDeclaration> result;
                if (_declarations.TryGetValue(usr, out result))
                {
                    return result;
                }
                return new List<IDeclaration>();
            }
        }
        public IEnumerable<string> DeclarationUSRs
        {
            get
            {
                lock (_lock)
                {
                    List<string> decls = new List<string>(_declarations.Keys);
                    return decls;
                }
            }
        }

        public IEnumerable<IReference> References
        {
            get
            {
                lock (_lock)
                {
                    List<IReference> refs = new List<IReference>(_references);
                    return refs;
                }
            }
        }

        public void Dump()
        {
            lock (_lock)
            {
                foreach (IList<IDeclaration> decls in _declarations.Values)
                {
                    foreach (IDeclaration decl in decls)
                    {
                        Debug.WriteLine(string.Format("Decl {0}", decl));
                    }
                }
                foreach (IReference r in _references)
                {
                    Debug.WriteLine(string.Format("Refr {0}", r));
                }
            }
        }
    }
}
