using System.Diagnostics;

namespace LibClang
{
    /// <summary>
    /// An (incomplete) immutable wrapper around libclang's CXType type.
    /// Represents a the type of an element in the abstract syntax tree.
    /// </summary>
    public sealed class Type
    {
        #region Data

        internal Library.CXType Handle { get; private set; }

        private ITranslationUnitItemFactory _itemFactory;
        private string _spelling;
        private string _typeKindSpelling;

        #endregion

        #region Constructor

        internal delegate Type CreateTypeDel(Library.CXType handle);

        internal Type(Library.CXType handle, ITranslationUnitItemFactory itemFactory)
        {
            Debug.Assert(handle.IsValid);
            Handle = handle;
            _itemFactory = itemFactory;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The canonical type is the underlying type with all the "sugar" removed.  For example, if 'T' is a typedef
        /// for 'int', the canonical type for 'T' would be 'int'.
        /// </summary>
        public Type Canonical
        {
            get { return _itemFactory.CreateType(Library.clang_getCanonicalType(Handle)); }
        }

        /// <summary>
        /// Returns the Type object representing the type pointed to if kind is TypeKind.Pointer.
        /// </summary>
        public Type Pointee
        {
            get { return _itemFactory.CreateType(Library.clang_getPointeeType(Handle)); }
        }

        /// <summary>
        /// Retrieve the result type associated with a function type.
        /// </summary>
        public Type Result
        {
            get { return _itemFactory.CreateType(Library.clang_getResultType(Handle)); }
        }

        /// <summary>
        /// Returns the Cursor object representing the type's declaration.
        /// </summary>
        public Cursor Declaration
        {
            get { return _itemFactory.CreateCursor(Library.clang_getTypeDeclaration(Handle)); }
        }

        /// <summary>
        /// Returns the kind.
        /// </summary>
        public TypeKind Kind
        {
            get { return Handle.kind; }
        }

        public string TypeKindSpelling
        {
            get { return _typeKindSpelling ?? (_typeKindSpelling = Library.clang_getTypeKindSpelling(Kind).ManagedString); }
        }

        public string Spelling
        {
            get { return _spelling ?? (_spelling = Library.clang_getTypeSpelling(Handle).ManagedString); }
        }

        #endregion

        #region object interface

        public static bool operator ==(Type left, Type right)
        {
            if ((object)left == null && (object)right == null) return true;
            if ((object)left == null || (object)right == null) return false;
            return left.Handle == right.Handle;
        }

        public static bool operator !=(Type left, Type right)
        {
            return !(left == right);
        }
        
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            return obj is Type && this == obj as Type;
        }

        public override int GetHashCode()
        {
            return Handle.GetHashCode();
        }

        public override string ToString()
        {
            return Spelling;
        }

        #endregion
    }
}
