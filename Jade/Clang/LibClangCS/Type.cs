using System;
using System.Diagnostics;

namespace LibClang
{
    /// <summary>
    /// An (incomplete) immutable wrapper around libclang's CXType type.
    /// Represents a the type of an element in the abstract syntax tree.
    /// </summary>
    public sealed class Type
    {
        public enum CallingConv
        {
            Default = 0,
            C = 1,
            X86StdCall = 2,
            X86FastCall = 3,
            X86ThisCall = 4,
            X86Pascal = 5,
            AAPCS = 6,
            AAPCS_VFP = 7,
            PnaclCall = 8,
            IntelOclBicc = 9,
            X86_64Win64 = 10,
            X86_64SysV = 11,
            Invalid = 100,
            Unexposed = 200
        };

        public enum RefQualifierKind
        {
            /// No ref-qualifier was provided.
            None = 0,
            /// An lvalue ref-qualifier was provided (\c &).
            LValue,
            /// An rvalue ref-qualifier was provided (\c &&).
            RValue
        };


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
        /// Determine whether a CXType has the "const" qualifier set,
        /// without looking through typedefs that may have added "const" at a different level.
        /// </summary>
        public bool IsConstQualified
        {
            get { return Library.clang_isConstQualifiedType(Handle) != 0; }
        }

        /// <summary>
        /// Returns true if the "volatile" qualifier is set,
        /// without looking through typedefs that may have added "volatile" at a different level. 
        /// </summary>
        public bool IsVolatileQualified
        {
            get { return Library.clang_isVolatileQualifiedType(Handle) != 0; }
        }

        /// <summary>
        /// Determine whether a CXType has the "restrict" qualifier set,
        /// without looking through typedefs that may have added "restrict" at a different level.
        /// </summary>
        public bool IsRestrictQualified
        {
            get { return Library.clang_isRestrictQualifiedType(Handle) != 0; }
        }

        /// <summary>
        /// Retrieve the calling convention associated with a function type.
        /// </summary>
        public CallingConv CallingConvention
        {
            get { return (CallingConv)Library.clang_getFunctionTypeCallingConv(Handle); }
        }

        /// <summary>
        /// Returns the number of non-variadic arguments associated with a function type.
        /// If a non-function type is passed in, -1 is returned.
        /// </summary>
        public int NumberOfArguments
        {
            get { return Library.clang_getNumArgTypes(Handle); }
        }

        /// <summary>
        /// Returns the type of an argument of a function type.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public Type GetArgumentType(uint arg)
        {
            return ReturnType(Library.clang_getArgType(Handle, arg));
        }

        /// <summary>
        /// Returns true if the type is a variadic function type
        /// </summary>
        public bool IsFunctionTypeVariadic
        {
            get { return Library.clang_isFunctionTypeVariadic(Handle) != 0; }
        }

        /// <summary>
        /// Returns true if the type is a POD.
        /// </summary>
        public bool IsPOD
        {
            get { return Library.clang_isPODType(Handle) != 0; }
        }

        /// <summary>
        /// Returns the element type of an array, complex, or vector type.
        /// </summary>
        public Type ElementType
        {
            get { return ReturnType(Library.clang_getElementType(Handle)); }
        }

        /// <summary>
        /// Returns the number of elements of an array or vector type.
        /// </summary>
        public Int64 NumberOfElements
        {
            get { return Library.clang_getNumElements(Handle); }
        }

        /// <summary>
        /// Returns the element type of an array type.
        /// </summary>
        public Type ArrayElementType
        {
            get { return ReturnType(Library.clang_getArrayElementType(Handle)); }
        }

        /// <summary>
        /// Return the array size of a constant array.
        /// </summary>
        public Int64 ArraySize
        {
            get { return Library.clang_getArraySize(Handle); }
        }

        /// <summary>
        /// Return the alignment of a type in bytes as per C++[expr.alignof] standard.
        ///
        /// If the type declaration is invalid, CXTypeLayoutError_Invalid is returned.
        /// If the type declaration is an incomplete type, CXTypeLayoutError.Incomplete
        ///   is returned.
        /// If the type declaration is a dependent type, CXTypeLayoutError.Dependent is returned.
        /// If the type declaration is not a constant size type, CXTypeLayoutError.NotConstantSize is returned.
        /// </summary>
        public Int64 Alignment
        {
            get { return Library.clang_Type_getAlignOf(Handle); }
        }

        /// <summary>
        /// Return the class type of an member pointer type.
        /// </summary>
        public Type ClassType
        {
            get 
            {
                return ReturnType(Library.clang_Type_getClassType(Handle));
            }
        }

        /// <summary>
        /// Return the size of a type in bytes as per C++[expr.sizeof] standard.
        /// If the type declaration is invalid, CXTypeLayoutError_Invalid is returned.
        /// If the type declaration is an incomplete type, CXTypeLayoutError_Incomplete is returned.
        /// If the type declaration is a dependent type, CXTypeLayoutError_Dependent is returned.
        /// </summary>
        public Int64 SizeOf
        {
            get { return Library.clang_Type_getSizeOf(Handle); }
        }

        /// <summary>
        /// Return the offset of a field named S in a record of type T in bits
        ///   as it would be returned by __offsetof__ as per C++11[18.2p4]
        /// If the cursor is not a record field declaration, CXTypeLayoutError_Invalid is returned.
        /// If the field's type declaration is an incomplete type,
        ///   CXTypeLayoutError_Incomplete is returned.
        /// If the field's type declaration is a dependent type,
        ///   CXTypeLayoutError_Dependent is returned.
        /// If the field's name S is not found,
        ///   CXTypeLayoutError_InvalidFieldName is returned.
        /// </summary>
        public Int64 GetOffsetOf(string fieldName)
        {
            return Library.clang_Type_getOffsetOf(Handle, fieldName);
        }

        public RefQualifierKind RefQualifier
        {
            get { return (RefQualifierKind)Library.clang_Type_getCXXRefQualifier(Handle); }

        }

        private Type ReturnType(Library.CXType t)
        {
            return t != null && t.IsValid ? _itemFactory.CreateType(t) : null;
        }

        /// <summary>
        /// The canonical type is the underlying type with all the "sugar" removed.  For example, if 'T' is a typedef
        /// for 'int', the canonical type for 'T' would be 'int'.
        /// </summary>
        public Type CanonicalType
        {
            get { return ReturnType(Library.clang_getCanonicalType(Handle)); }
        }

        /// <summary>
        /// Returns the Type object representing the type pointed to if kind is TypeKind.Pointer.
        /// </summary>
        public Type PointeeType
        {
            get 
            {
                return ReturnType(Library.clang_getPointeeType(Handle));
            }
        }

        /// <summary>
        /// Retrieve the result type associated with a function type.
        /// </summary>
        public Type ResultType
        {
            get 
            {
                return ReturnType(Library.clang_getResultType(Handle)); 
            }
        }

        private Cursor _declaration;

        /// <summary>
        /// Returns the Cursor object representing the type's declaration.
        /// </summary>
        public Cursor DeclarationCursor
        {
            get 
            {
                if (_declaration == null)
                {
                    Library.CXCursor c = Library.clang_getTypeDeclaration(Handle);
                    if (c != null && c.IsValid)
                        _declaration = _itemFactory.CreateCursor(c);
                }
                return _declaration; 
            }
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
