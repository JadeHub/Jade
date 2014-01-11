using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LibClang
{
    /// <summary>
    /// An immutable wrapper around libclang's Cursor type.
    /// A Cursor represents an element in the AST of a translation unit.
    /// 
    /// Property comments taken from libclang.h.
    /// see http://clang.llvm.org/doxygen/Index_8h.html for more details.
    /// </summary>
    public sealed class Cursor
    {
        internal delegate Cursor CreateCursorDel(Library.Cursor handle);

        #region Data
        
        private readonly ITranslationUnitItemFactory _itemFactory;
        private SourceLocation _location;
        private SourceRange _extent;
        private string _spelling;
        private readonly Type _type;
        private string _usr;
        
        #endregion

        #region Construction

        /// <summary>
        /// Create a new Cursor object.
        /// </summary>
        /// <param name="handle">Handle to a non null cursor obect.</param>
        /// <param name="itemFactory">TranslationUnit's item factory / item cache.</param>
        internal Cursor(Library.Cursor handle, ITranslationUnitItemFactory itemFactory)
        {
            Debug.Assert(!handle.IsNull);
            Handle = handle;
            _itemFactory = itemFactory;
            Kind = Library.clang_getCursorKind(Handle);
            Library.CXType typeHandle = Library.clang_getCursorType(Handle);
            if (typeHandle.IsValid)
                _type = _itemFactory.CreateType(typeHandle);
        }
                
        #endregion

        #region Properties

        /// <summary>
        /// SourceRange object representing the extent of this cursor.
        /// 
        /// Retrieve the physical extent of the source construct referenced by
        /// the given cursor.
        ///
        /// The extent of a cursor starts with the file/line/column pointing at the
        /// first character within the source construct that the cursor refers to and
        /// ends with the last character withinin that source construct. For a
        /// declaration, the extent covers the declaration itself. For a reference,
        /// the extent covers the location of the reference (e.g., where the referenced
        /// entity was actually used).
        /// </summary>
        public SourceRange Extent
        {
            get { return _extent ?? (_extent = MakeExtent()); }
        }

        private SourceRange MakeExtent()
        {
            Library.SourceRange range = Library.clang_getCursorExtent(Handle);
            if (range.IsNull)
                return null;
            return _itemFactory.CreateSourceRange(Library.clang_getCursorExtent(Handle));
        }

        internal Library.Cursor Handle
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Return the Kind of this Cursor.
        /// </summary>
        public CursorKind Kind
        {
            get;
            private set;
        }
                
        public string Spelling
        {
            get { return _spelling ?? (_spelling = Library.clang_getCursorSpelling(Handle).ManagedString); }
        }        

        /// <summary>
        /// A SourceLocation object representing the cursor's location.
        /// 
        /// Retrieve the physical location of the source construct referenced
        ///  by the given cursor.
        ///
        /// The location of a declaration is typically the location of the name of that
        /// declaration, where the name of that declaration would occur if it is
        /// unnamed, or some keyword that introduces that particular declaration.
        /// The location of a reference is where that reference occurs within the
        /// source code.
        /// </summary>
        public SourceLocation Location
        {
            get { return _location ??(_location = _itemFactory.CreateSourceLocation(Library.clang_getCursorLocation(Handle))); }
        }

        /// <summary>
        /// Return the Type object associated wiht this cursor.
        /// </summary>
        public Type Type
        {
            get { return _type; }
        }

        /// <summary>
        /// Return a Unified Symbol Resolution (USR) for the entity referenced
        /// by the given cursor.
        ///
        /// A Unified Symbol Resolution (USR) is a string that identifies a particular
        /// entity (function, class, variable, etc.) within a program. USRs can be
        /// compared across translation units to determine, e.g., when references in
        /// one translation refer to an entity defined in another translation unit.
        /// </summary>
        public string Usr
        {
            get { return _usr ?? (_usr = Library.clang_getCursorUSR(Handle).ManagedString);}        
        }

        /// <summary>
        /// Returns the canonical cursor, if any, for the entity referred to by the given cursor.
        /// May return 'this'.
        /// </summary>
        public Cursor CanonicalCursor
        {
            get 
            {
                Library.Cursor cur = Library.clang_getCursorDefinition(Handle);
                if (cur == Handle)
                    return this;
                return cur.IsNull ? null : _itemFactory.CreateCursor(cur);
            }
        }

        /// <summary>
        /// Returns true if the declaration pointed to by this cursor
        /// is also a definition of that entity.
        /// </summary>
        public bool IsDefinition
        {
            get { return Library.clang_isCursorDefinition(Handle) != 0; }
        }

        /// <summary>
        /// For a cursor that is either a reference to or a declaration
        /// of some entity, return a cursor that describes the definition of
        /// that entity. 
        /// </summary>
        public Cursor Definition
        {
            get 
            {
                Library.Cursor cur = Library.clang_getCursorDefinition(Handle);
                return (cur.IsNull || cur == Handle) ? null : _itemFactory.CreateCursor(cur);
            }
        }

        /// <summary>
        /// Returns true if the given cursor kind represents a simple reference.
        /// </summary>
        public bool IsReference
        {
            get { return Library.clang_isReference(Kind) != 0; }
        }
        
        /// <summary>
        /// For a cursor that is a reference, retrieve a cursor representing the
        /// entity that it references.
        /// </summary>
        public Cursor CursorReferenced
        {
            get
            {
                Library.Cursor cur = Library.clang_getCursorReferenced(Handle);
                return (cur.IsNull || cur == Handle) ? null : _itemFactory.CreateCursor(cur);
            }
        }

        /// <summary>
        /// Returns the lexical parent of the given cursor.
        /// </summary>
        public Cursor LexicalParentCurosr
        {
            get
            {
                Library.Cursor cur = Library.clang_getCursorLexicalParent(Handle);
                return cur.IsNull ? null : _itemFactory.CreateCursor(cur);
            }
        }

        /// <summary>
        /// Returns the semantic parent of the given cursor.
        /// </summary>
        public Cursor SemanticParentCurosr
        {
            get
            {
                Library.Cursor cur = Library.clang_getCursorSemanticParent(Handle);
                return cur.IsNull ? null : _itemFactory.CreateCursor(cur);
            }
        }

        /// <summary>
        /// Returns true if the cursor's kind represents an invalid cursor.
        /// </summary>
        public bool Valid { get { return Library.clang_isInvalid(Kind) == 0; } }

        #endregion

        #region Child visitation
        
        /// <summary>
        /// Value returned by the visitor callback to control the ast navigation.
        /// </summary>
        public enum ChildVisitResult
        {
            Break,
            Continue,
            Recurse,
        };

        /// <summary>
        /// Cursor visitor callback
        /// </summary>
        /// <param name="cursor">Cursor being visited.</param>
        /// <param name="parent">Parent of cursor being visited.</param>
        /// <returns>ChildVisitResult</returns>
        public delegate ChildVisitResult CursorVisitor(Cursor cursor, Cursor parent);

        /// <summary>
        /// Inspect cursor's children via a callback.
        /// </summary>
        /// <param name="visitor"></param>
        public void VisitChildren(CursorVisitor visitor)
        {
            Library.clang_visitChildren(
                Handle,
                (cursor, parent, data) => visitor(_itemFactory.CreateCursor(cursor), _itemFactory.CreateCursor(parent)),
                IntPtr.Zero);
        }

        public IList<Cursor> Children
        {
            get
            {
                IList<Cursor> children = new List<Cursor>();

                VisitChildren(delegate(Cursor cursor, Cursor parent)
                {
                    children.Add(cursor);
                    return ChildVisitResult.Continue;
                });
                return children;
            }
        }

        #endregion

        #region Object overrides

        public override string ToString()
        {
            return Kind + " at " + Location.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is Cursor)
            {
                return (obj as Cursor).Handle == Handle;
            }
            return false;
        }

        private int hash = 0;

        public override int GetHashCode()
        {
            return hash == 0 ? (hash = Handle.GetHashCode()) : hash;
        }

        #endregion

        #region Static operator functions

        public static bool operator == (Cursor left, Cursor right)
        {
            if ((object)left == null && (object)right == null) return true;
            if ((object)left == null || (object)right == null) return false;
            return left.Handle == right.Handle;
        }

        public static bool operator !=(Cursor left, Cursor right)
        {
            return !(left == right);
        }

        #endregion
    }
}
