using System;

namespace LibClang
{
    public enum TypeKind
    {
        /**
        * \brief Reprents an invalid type (e.g., where no type is available).
        */
        Invalid = 0,

        /**
        * \brief A type whose specific kind is not exposed via this
        * interface.
        */
        Unexposed = 1,

        /* Builtin types */
        Void = 2,
        Bool = 3,
        Char_U = 4,
        UChar = 5,
        Char16 = 6,
        Char32 = 7,
        UShort = 8,
        UInt = 9,
        ULong = 10,
        ULongLong = 11,
        UInt128 = 12,
        Char_S = 13,
        SChar = 14,
        WChar = 15,
        Short = 16,
        Int = 17,
        Long = 18,
        LongLong = 19,
        Int128 = 20,
        Float = 21,
        Double = 22,
        LongDouble = 23,
        NullPtr = 24,
        Overload = 25,
        Dependent = 26,
        ObjCId = 27,
        ObjCClass = 28,
        ObjCSel = 29,
        FirstBuiltin = Void,
        LastBuiltin = ObjCSel,

        Complex = 100,
        Pointer = 101,
        BlockPointer = 102,
        LValueReference = 103,
        RValueReference = 104,
        Record = 105,
        Enum = 106,
        Typedef = 107,
        ObjCInterface = 108,
        ObjCObjectPointer = 109,
        FunctionNoProto = 110,
        FunctionProto = 111,
        ConstantArray = 112,
        Vector = 113,
        IncompleteArray = 114,
        VariableArray = 115,
        DependentSizedArray = 116,
        MemberPointer = 117
    }

    public class Type
    {
        #region Data

        private Dll.Type _handle;

        #endregion

        internal Type(Dll.Type handle)
        {
            _handle = handle;
        }

        #region Properties

        public bool Valid 
        { 
            get 
            {
                return _handle.kind != TypeKind.Invalid;
            }
        }

        public Type Canonical
        {
            get { return new Type(Dll.clang_getCanonicalType(_handle)); }
        }

        public Type Pointee
        {
            get { return new Type(Dll.clang_getPointeeType(_handle)); }
        }

        public Type Result
        {
            get { return new Type(Dll.clang_getResultType(_handle)); }
        }

        public Cursor Declaration
        {
            get { return new Cursor(Dll.clang_getTypeDeclaration(_handle)); }
        }

        public TypeKind Kind
        {
            get { return _handle.kind; }
        }

        public string TypeKindSpelling
        {
            get
            {
                return Dll.clang_getTypeKindSpelling(Kind).ManagedString;
            }
        }

        #endregion

        public bool Equals(Type other)
        {
            return Dll.clang_equalTypes(_handle, other._handle) != 0;
        }

        public override bool Equals(object obj)
        {
            return obj is Type && Equals((Type)obj);
        }

        public override int GetHashCode()
        {
            return _handle.GetHashCode();
        }

        public override string ToString()
        {
            if(Valid)
                return Spelling;
            return "Invalid";
        }

        public string Spelling
        {
            get
            {
                switch (Kind)
                {
                    case TypeKind.Void:
                        return "void";
                    case TypeKind.Bool:
                        return "bool";
                    case TypeKind.UChar:
                    case TypeKind.Char_U:
                        return "unsigned char";
                    case TypeKind.Char16:
                        return "char16_t";
                    case TypeKind.Char32:
                        return "char32_t";
                    case TypeKind.UShort:
                        return "unsigned short int";
                    case TypeKind.UInt:
                        return "unsigned int";
                    case TypeKind.ULong:
                        return "unsigned long int";
                    case TypeKind.ULongLong:
                        return "unsigned long long int";
                    case TypeKind.UInt128:
                        return "uint128_t";
                    case TypeKind.Char_S:
                        return "char";
                    case TypeKind.SChar:
                        return "signed char";
                    case TypeKind.WChar:
                        return "wchar_t";
                    case TypeKind.Short:
                        return "short int";
                    case TypeKind.Int:
                        return "int";
                    case TypeKind.Long:
                        return "long int";
                    case TypeKind.LongLong:
                        return "long long int";
                    case TypeKind.Int128:
                        return "int128_t";
                    case TypeKind.Float:
                        return "float";
                    case TypeKind.Double:
                        return "double";
                    case TypeKind.LongDouble:
                        return "long double";
                    case TypeKind.NullPtr:
                        return "nullptr";
                    case TypeKind.Pointer:
                        return Pointee.Spelling + "*";
                    default:
                        return Declaration.Spelling;
                }
            }
        }
    }
}
