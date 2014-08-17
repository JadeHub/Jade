using System;
using System.Runtime.InteropServices;

namespace LibClang
{
    using CXTranslationUnit = System.IntPtr;
    using CXIndex = IntPtr;
    using CXIndexAction = IntPtr;
    using CXIdxClientASTFile = System.IntPtr;
    using CXIdxClientContainer = System.IntPtr;
    using CXIdxClientFile = System.IntPtr;
    using CXFile = System.IntPtr;
    using CXClientData = System.IntPtr;
    using CXDiagnostic = System.IntPtr;
    using CXDiagnosticSet = System.IntPtr;

    /// <summary>
    /// Provides access to libclang.dll
    /// </summary>
    internal class Library
    {
        #region CXString

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        internal unsafe struct CXString
        {
            readonly void* Spelling;
            readonly uint Flags;

            internal string ManagedString
            {
                get
                {
                    string res = Marshal.PtrToStringAnsi(clang_getCString(this));
                    Library.clang_disposeString(this);
                    return res;
                }
            }
        }
                
        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern IntPtr clang_getCString(CXString str);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern void clang_disposeString(CXString str);

        #endregion

        #region CXIndex

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXIndex clang_createIndex(int excludeDeclarationsFromPch, int displayDiagnostics);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void clang_disposeIndex(CXIndex index);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXIndexAction clang_IndexAction_create(CXIndex index);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void clang_IndexAction_dispose(CXIndex index);
        
        #endregion

        #region CXTranslationUnit

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        internal struct UnsavedFile
        {
            internal string Filename;
            internal string Contents;
            internal UInt32 Length;
        }

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint clang_defaultEditingTranslationUnitOptions();

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern CXTranslationUnit clang_parseTranslationUnit(CXIndexAction index, string sourceFilename,
            string[] clangCommandLineArgs, int numClangCommandLineArgs, UnsavedFile[] unsavedFiles, uint numUnsavedFiles, int options);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void clang_disposeTranslationUnit(CXTranslationUnit tu);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern int clang_reparseTranslationUnit(CXTranslationUnit tu, uint numUnsavedFiles, UnsavedFile[] unsavedFiles, int options);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern CXTranslationUnit clang_createTranslationUnitFromSourceFile(CXIndex index, string sourceFilename,
            int numClangCommandLineArgs, string[] clangCommandLineArgs, uint numUnsavedFiles, UnsavedFile[] unsavedFiles);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXCursor clang_getTranslationUnitCursor(CXTranslationUnit tu);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern CXString clang_getTranslationUnitSpelling(CXTranslationUnit tu);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern CXFile clang_getIncludedFile(CXCursor c);

        #endregion

        #region CXFile

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern CXString clang_getFileName(CXFile fileHandle);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern long clang_getFileTime(CXFile fileHandle);
                
        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern CXFile clang_getFile(CXTranslationUnit tu, string filename);

        [StructLayout(LayoutKind.Sequential)]
        internal struct CXFileUniqueID
        {
            internal UInt64 data1;
            internal UInt64 data2;
            internal UInt64 data3;
        }

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern unsafe int clang_getFileUniqueID(CXFile file, CXFileUniqueID* id);
                
        #endregion

        #region CXSourceRange

        [StructLayout(LayoutKind.Sequential)]
        internal struct CXSourceRange
        {
            static public CXSourceRange NullRange;

            static CXSourceRange()
            {
                NullRange = Library.clang_getNullRange();
            }

            public bool IsNull { get { return this == NullRange; } }

            readonly IntPtr data0; //SourceManager object pointer
            readonly IntPtr data1; //LangOptions object pointer
            readonly uint beginIntData; //Raw 32bit encoding of start location
            readonly uint endIntData; //Raw 32bit encoding of end location

            public static bool operator ==(CXSourceRange left, CXSourceRange right)
            {
                return left.beginIntData == right.beginIntData &&
                    left.endIntData == right.endIntData &&
                    left.data1 == right.data1 &&
                    left.data0 == right.data0;
            }

            public static bool operator !=(CXSourceRange left, CXSourceRange right)
            {
                return !(left == right);
            }

            public override bool Equals(object obj)
            {
                if (obj != null && obj is CXSourceRange)
                {
                    return (CXSourceRange)obj == this;
                }
                return false;
            }

            public override int GetHashCode()
            {
                return (int)((beginIntData << 16) | (endIntData & 0xffff));
            }
        }

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXSourceRange clang_getNullRange();

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint clang_equalRanges(CXSourceRange r1, CXSourceRange r2);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXSourceLocation clang_getRangeStart(CXSourceRange range);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXSourceLocation clang_getRangeEnd(CXSourceRange range);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXSourceRange clang_getRange(CXSourceLocation start, CXSourceLocation end);
        
        #endregion

        #region CXSourceLocation

        [StructLayout(LayoutKind.Sequential)]
        public struct CXSourceLocation
        {
            static public CXSourceLocation NullLocation;

            static CXSourceLocation()
            {
                NullLocation = Library.clang_getNullLocation();
            }

            public bool IsNull { get { return this == NullLocation; } }

            readonly IntPtr data0; //Pointer to the SourceManager object
            readonly IntPtr data1; //Pointer to the LangOptions object
            readonly uint data2; //Raw 32bit encoding of the location

            public static bool operator ==(CXSourceLocation left, CXSourceLocation right)
            {
                return left.data2 == right.data2 &&
                    left.data1 == right.data1 &&
                    left.data0 == right.data0;
            }

            public static bool operator !=(CXSourceLocation left, CXSourceLocation right)
            {
                return !(left == right);
            }

            public override bool Equals(object obj)
            {
                if (obj != null && obj is CXSourceLocation)
                {
                    return (CXSourceLocation)obj == this;
                }
                return false;
            }

            public override int GetHashCode()
            {                
                return (int)data2;
            }
        }

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXSourceLocation clang_getLocationForOffset(CXTranslationUnit tu, CXFile fileHandle, uint offset);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXSourceLocation clang_getNullLocation();

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint clang_equalLocations(CXSourceLocation l1, CXSourceLocation l2);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal unsafe static extern void clang_getInstantiationLocation(CXSourceLocation location, CXFile* file,
                                            out uint line, out uint column, out uint offset);

        #endregion

        #region CXType

        [StructLayout(LayoutKind.Sequential)]
        internal struct CXType
        {
            internal LibClang.TypeKind kind;
            readonly IntPtr data0;
            readonly IntPtr data1;

            public bool IsValid
            {
                get { return kind != TypeKind.Invalid; }
            }

            public static bool operator ==(CXType left, CXType right)
            {
                if ((object)left == null && (object)right == null)
                    return true;
                if ((object)left == null || (object)right == null)
                    return false;
                return Library.clang_equalTypes(left, right) != 0;
            }

            public static bool operator !=(CXType left, CXType right)
            {
                return !(left == right);
            }

            public override bool Equals(object obj)
            {
                if (obj != null && obj is CXType)
                {
                    return (CXType)obj == this;
                }
                return false;
            }

            public override int GetHashCode()
            {
                return data0.ToInt32();
            }
        };

        internal enum CXCallingConv
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

        internal enum CXRefQualifierKind
        {
            /// No ref-qualifier was provided.
            None = 0,
            /// An lvalue ref-qualifier was provided (\c &).
            LValue,
            /// An rvalue ref-qualifier was provided (\c &&).
            RValue
        };
        
        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_equalTypes(CXType a, CXType b);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXString clang_getTypeSpelling(CXType c);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType clang_getCanonicalType(CXType t);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType clang_getPointeeType(CXType t);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursor clang_getTypeDeclaration(CXType t);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern CXString clang_getTypeKindSpelling(LibClang.TypeKind k);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType clang_getResultType(CXType t);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_isConstQualifiedType(CXType t);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_isVolatileQualifiedType(CXType t);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_isRestrictQualifiedType(CXType t);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCallingConv clang_getFunctionTypeCallingConv(CXType t);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        public static extern int clang_getNumArgTypes(CXType t);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType clang_getArgType(CXType t, uint i);
                
        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_isFunctionTypeVariadic(CXType t);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_isPODType(CXType t);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType clang_getElementType(CXType t);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int64 clang_getNumElements(CXType t);
          
        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType clang_getArrayElementType(CXType t);
         
        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int64 clang_getArraySize(CXType t);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXRefQualifierKind clang_Type_getCXXRefQualifier(CXType t);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int64 clang_Type_getOffsetOf(CXType t, string s);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int64 clang_Type_getSizeOf(CXType t);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType clang_Type_getClassType(CXType t);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int64 clang_Type_getAlignOf(CXType t);

        #endregion

        #region CXCursor

        internal enum CXXAccessSpecifier
        {
            Invalid = 0,
            Public = 1,
            Protected = 2,
            Private = 3
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct CXCursor
        {
            static public CXCursor NullCursor;

            static CXCursor()
            {
                NullCursor = Library.clang_getNullCursor();
            }

            public bool IsNull { get { return this == NullCursor; } }

            public bool IsValid
            {
                get
                {
                    return !IsNull && Library.clang_isInvalid(kind) == 0;
                }
            }

            readonly CursorKind kind;
            readonly int xdata;
            readonly IntPtr data0, data1, data2;

            public static bool operator ==(CXCursor left, CXCursor right)
            {
                return Library.clang_equalCursors(left, right) != 0;
            }

            public static bool operator !=(CXCursor left, CXCursor right)
            {
                return !(left == right);
            }

            public override bool Equals(object obj)
            {
                if (obj != null && obj is CXCursor)
                {
                    return (CXCursor)obj == this;
                }
                return false;
            }
                        
            public override int GetHashCode()
            {
                return (int)Library.clang_hashCursor(this);
            }
        };

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate LibClang.Cursor.ChildVisitResult CXCursorVisitor(CXCursor cursor, CXCursor parent, CXClientData data);
                
        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CursorKind clang_getCursorKind(CXCursor c);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint clang_isReference(CursorKind ck);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_isInvalid(CursorKind kc);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern CXString clang_getCursorSpelling(CXCursor c);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern uint clang_visitChildren(CXCursor parent, CXCursorVisitor visitor, CXClientData clientData);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXSourceLocation clang_getCursorLocation(CXCursor cursor);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXCursor clang_getCursorReferenced(CXCursor c);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXSourceRange clang_getCursorExtent(CXCursor cursor);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType clang_getCursorType(CXCursor c);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursor clang_getNullCursor();

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursor clang_getCursorLexicalParent(CXCursor c);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursor clang_getCursorSemanticParent(CXCursor c);
        
        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint clang_isCursorDefinition(CXCursor c);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXCursor clang_getCursorDefinition(CXCursor c);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern CXString clang_getCursorUSR(CXCursor c);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXCursor clang_getCanonicalCursor(CXCursor c);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXCursor clang_getCursor(CXTranslationUnit tu, CXSourceLocation loc);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint clang_equalCursors(CXCursor c1, CXCursor c2);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint clang_hashCursor(CXCursor c);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXXAccessSpecifier clang_getCXXAccessSpecifier(CXCursor c);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int clang_Cursor_getNumArguments(CXCursor c);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXCursor clang_Cursor_getArgument(CXCursor C, uint i);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXType clang_getCursorResultType(CXCursor c);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXString clang_getCursorDisplayName(CXCursor c);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static unsafe extern void clang_getOverriddenCursors(CXCursor c, CXCursor** overridden, uint* num_overridden);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static unsafe extern void clang_disposeOverriddenCursors(CXCursor* overridden);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint clang_isVirtualBase(CXCursor c);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint clang_CXXMethod_isPureVirtual(CXCursor c);
        
        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint clang_CXXMethod_isStatic(CXCursor C);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint clang_CXXMethod_isVirtual(CXCursor C);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXType clang_getEnumDeclIntegerType(CXCursor C);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int clang_Cursor_isDynamicCall(CXCursor C); //returns int not uint

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint clang_Cursor_isVariadic(CXCursor C);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint clang_Cursor_isBitField(CXCursor C);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXCursor clang_getSpecializedCursorTemplate(CXCursor c);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CursorKind clang_getTemplateCursorKind(CXCursor c);
        
        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint clang_getNumOverloadedDecls(CXCursor cursor);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXCursor clang_getOverloadedDecl(CXCursor cursor, uint index);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXComment clang_Cursor_getParsedComment(CXCursor c);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXSourceRange clang_getCursorReferenceNameRange(CXCursor c, uint flags, uint pieceIndex);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXType clang_getTypedefDeclUnderlyingType(CXCursor c);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern Int64 clang_getEnumConstantDeclValue(CXCursor c);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern UInt64 clang_getEnumConstantDeclUnsignedValue(CXCursor c);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int clang_getFieldDeclBitWidth(CXCursor c);

        #endregion

        #region CXTokens

        [StructLayout(LayoutKind.Sequential)]
        internal struct CXToken
        {
            readonly uint data0;
            readonly uint data1;
            readonly uint data2;
            readonly uint data3;

            readonly IntPtr ptr_data;

            public static bool operator ==(CXToken left, CXToken right)
            {
                return left.data0 == right.data0 &&
                    left.data1 == right.data1 &&
                    left.data2 == right.data2 &&
                    left.data3 == right.data3 &&
                    left.ptr_data == right.ptr_data;
            }

            public static bool operator !=(CXToken left, CXToken right)
            {
                return !(left == right);
            }

            public override bool Equals(object obj)
            {
                if (obj != null && obj is CXToken)
                {
                    return (CXToken)obj == this;
                }
                return false;
            }

            public override int GetHashCode()
            {
                return (int)ptr_data.ToInt32();
            }
        }

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern TokenKind clang_getTokenKind(CXToken tok);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXString clang_getTokenSpelling(CXTranslationUnit tu, CXToken tok);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXSourceLocation clang_getTokenLocation(CXTranslationUnit tu, CXToken tok);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXSourceRange clang_getTokenExtent(CXTranslationUnit tu, CXToken tok);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static unsafe extern void clang_tokenize(CXTranslationUnit tu, CXSourceRange Range, CXToken** Tokens, uint* NumTokens);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static unsafe extern void clang_disposeTokens(CXTranslationUnit tu, CXToken* Tokens, uint NumTokens);

        #endregion

        #region Indexing

        [StructLayout(LayoutKind.Sequential)]
        internal struct CXIdxLoc
        {
            readonly IntPtr data0;
            readonly IntPtr data1;
            readonly uint data2;

            public bool Equals(CXIdxLoc other)
            {
                return data0 == other.data0 && data1 == other.data1 && data2 == other.data2;
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        internal unsafe struct CXIdxIncludedFileInfo
        {
            internal CXIdxLoc location;
            internal sbyte* fileName;
            internal IntPtr file;
            internal int isImport;
            internal int isAngled;
            internal int isModuleImport;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        internal struct CXIdxImportedASTFileInfo
        {
            internal IntPtr file;
            internal IntPtr module; 
            internal CXSourceLocation location;
            internal int isImplicit;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        internal struct CXIdxAttrInfo
        {
            internal Indexer.AttributeKind kind;
            internal CXCursor cursor;
            internal CXIdxLoc location;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        internal unsafe struct CXIdxEntityInfo
        {
            internal Indexer.EntityKind kind;
            internal Indexer.EntityCXXTemplateKind templateKind;
            internal Indexer.EntityLanguage language;
            internal sbyte* name;
            internal sbyte* USR;
            internal CXCursor cursor;
            internal CXIdxAttrInfo** attributes;
            internal int numAttributes;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal unsafe struct CXIdxContainerInfo
        {
            internal CXCursor cursor;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal unsafe struct CXIdxDeclInfo
        {
            internal CXIdxEntityInfo* entityInfo;
            internal CXCursor cursor;
            internal CXIdxLoc location;
            internal CXIdxContainerInfo* semanticContainer;
            internal CXIdxContainerInfo* lexicalContainer;
            internal int isRedeclaration;
            internal int isDefinition;
            internal int isContainer;
            internal CXIdxContainerInfo* declAsContainer;
            internal int isImplicit;
            internal CXIdxAttrInfo** attributes;
            internal int numAttributes;
            internal int flags;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal unsafe struct CXIdxEntityRefInfo
        {
            internal Indexer.EntityReferenceKind kind;
            internal CXCursor cursor;
            internal CXIdxLoc location;
            internal CXIdxEntityInfo* referencedEntity;
            internal CXIdxEntityInfo* parentEntity;
            internal CXIdxContainerInfo* container;
        }
                
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int IndexerAbort(CXClientData clientData, IntPtr reserved);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void IndexerDiagnostic(CXClientData clientData, CXDiagnosticSet diagnosticSet, IntPtr reserved);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate CXIdxClientFile IndexerEnteredMainFile(CXClientData clientData, CXFile mainFile, IntPtr reserved);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal unsafe delegate CXIdxClientFile IndexerPPIncludedFile(CXClientData client_data, CXIdxIncludedFileInfo* includedFileInfo);
  
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate CXIdxClientASTFile IndexerImportedASTFile(CXClientData clientData, CXIdxImportedASTFileInfo astFileInfo);
  
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate CXIdxClientContainer IndexerStartTranslationUnit(CXClientData clientData, IntPtr reserved);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal unsafe delegate void IndexerDeclaration(CXClientData clientData, CXIdxDeclInfo* a);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal unsafe delegate void IndexerEntityReference(CXClientData clientData, CXIdxEntityRefInfo* reference);
        
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        internal struct IndexerCallbacks
        {
            /// <summary>
            /// Called periodically to check whether indexing should be aborted.
            /// return 0 to continue, and non-zero to abort.
            /// </summary>
            internal IndexerAbort abortQuery;

            /// <summary>
            /// Called at the end of indexing; passes the complete diagnostic set.
            /// </summary>
            internal IndexerDiagnostic diagnostic;

            /// <summary>
            /// </summary>
            internal IndexerEnteredMainFile enterMainFile;

            /// <summary>
            /// (Not) called when a file gets #included or #imported.
            /// </summary>
            internal IndexerPPIncludedFile ppIncludedFile;

            /// <summary>
            /// Called when a AST file (PCH or module) gets imported.
            ///
            /// AST files will not get indexed (there will not be callbacks to index all
            /// the entities in an AST file). The recommended action is that, if the AST
            /// file is not already indexed, to initiate a new indexing job specific to
            /// the AST file.
            /// </summary>
            internal IndexerImportedASTFile astImportFile;

            /// <summary>
            /// Called at the beginning of indexing a translation unit.
            /// </summary>
            internal IndexerStartTranslationUnit startTU;

            /// <summary>
            /// Called to index the definition of an entity.
            /// </summary>
            internal IndexerDeclaration index;

            /// <summary>
            /// Called to index a reference of an entity.
            /// </summary>
            internal IndexerEntityReference entityRef;
        }

        
        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern int clang_indexSourceFile(CXIndexAction session, CXClientData clientData,
                                IndexerCallbacks[] cbs, uint cbsSize, uint indexOptions,
                                string fileName, string[] cmdLineArgs, int cmdLineCount,
                                UnsavedFile[] unsavedFiles, uint numUnsavedFiles, out TranslationUnit translationUnit, uint tuOptions);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern CXSourceLocation clang_indexLoc_getCXSourceLocation(CXIdxLoc idxLoc);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern int clang_indexTranslationUnit(CXIndexAction session, CXClientData clientData,
                                IndexerCallbacks[] cbs, uint cbsSize, uint indexOptions, CXTranslationUnit translationUnit);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal unsafe delegate void CXInclusionVisitor(CXFile fileHandle, CXSourceLocation* inclusion_stack, uint include_len, CXClientData clientData);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void clang_getInclusions(CXTranslationUnit tu, CXInclusionVisitor visitor, CXClientData clientData);

        #endregion

        #region FindReferencesInFile

        internal enum CXVisitorResult
        {
            CXVisit_Break,
            CXVisit_Continue
        };

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate CXVisitorResult Visit(IntPtr context, CXCursor cursor, CXSourceRange range);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        internal struct CXCursorAndRangeVisitor
        {
            internal IntPtr context;
            internal Visit visit;
        };

        internal enum CXResult
        {
            /// <summary>
            /// Function returned successfully.
            /// </summary>
            CXResult_Success = 0,

            /// <summary>
            /// One of the parameters was invalid for the function.
            /// </summary>
            CXResult_Invalid = 1,

            /// <summary>
            /// The function was terminated by a callback (e.g. it returned CXVisit_Break)
            /// </summary>
            CXResult_VisitBreak = 2
        };

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXResult clang_findReferencesInFile(CXCursor c, CXFile f, CXCursorAndRangeVisitor visitor);

        #endregion

        #region CXDiagnostic

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint clang_getNumDiagnostics(CXTranslationUnit tu);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXDiagnostic clang_getDiagnostic(CXTranslationUnit tu, uint index);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern Diagnostic.Severity clang_getDiagnosticSeverity(CXDiagnostic diagnostic);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXSourceLocation clang_getDiagnosticLocation(CXDiagnostic diagnostic);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXString clang_getDiagnosticSpelling(CXDiagnostic diagnostic);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal unsafe static extern CXString clang_getDiagnosticOption(CXDiagnostic diagnostic, CXString* disable);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint clang_getDiagnosticCategory(CXDiagnostic diagnostic);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXString clang_getDiagnosticCategoryName(uint category);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXString clang_getDiagnosticCategoryText(CXDiagnostic diagnostic);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint clang_getDiagnosticNumRanges(CXDiagnostic diagnostic);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXSourceRange clang_getDiagnosticRange(CXDiagnostic diagnostic, uint rangeIdx);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint clang_getDiagnosticNumFixIts(CXDiagnostic diagnostic);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal unsafe static extern CXString clang_getDiagnosticFixIt(CXDiagnostic diagnostic, uint fixIt, CXSourceRange* replacementRange);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void clang_disposeDiagnostic(CXDiagnostic dianostic);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXString clang_formatDiagnostic(CXDiagnostic diagnostic, uint options);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint clang_defaultDiagnosticDisplayOptions();

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint clang_getNumDiagnosticsInSet(CXDiagnosticSet handle);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXDiagnostic clang_getDiagnosticInSet(CXDiagnosticSet set, uint index);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void clang_disposeDiagnosticSet(CXDiagnosticSet handle);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXDiagnosticSet clang_getDiagnosticSetFromTU(CXTranslationUnit tu);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXDiagnosticSet clang_getChildDiagnostics(CXDiagnostic diag);

        #endregion

        #region CXComment

        [StructLayout(LayoutKind.Sequential)]
        internal struct CXComment
        {
            IntPtr ASTNode;
            CXTranslationUnit tu;
        }

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CommentKind clang_Comment_getKind(CXComment Comment);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint clang_Comment_getNumChildren(CXComment Comment);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXComment clang_Comment_getChild(CXComment Comment, uint ChildIdx);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint clang_Comment_isWhitespace(CXComment Comment);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint clang_InlineContentComment_hasTrailingNewline(CXComment Comment);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXString clang_TextComment_getText(CXComment Comment);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXString clang_InlineCommandComment_getCommandName(CXComment Comment);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CommentInlineCommandRenderKind clang_InlineCommandComment_getRenderKind(CXComment Comment);
                
        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint clang_InlineCommandComment_getNumArgs(CXComment Comment);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXString clang_InlineCommandComment_getArgText(CXComment Comment, uint argIndex);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXString clang_HTMLTagComment_getTagName(CXComment Comment);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint clang_HTMLStartTagComment_isSelfClosing(CXComment Comment);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint clang_HTMLStartTag_getNumAttrs(CXComment Comment);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXString clang_HTMLStartTag_getAttrName(CXComment Comment, uint AttrIdx);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXString clang_HTMLStartTag_getAttrValue(CXComment Comment, uint AttrIdx);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXString clang_BlockCommandComment_getCommandName(CXComment Comment);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint clang_BlockCommandComment_getNumArgs(CXComment Comment);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXString clang_BlockCommandComment_getArgText(CXComment Comment, uint argIndex);
            
        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXComment clang_BlockCommandComment_getParagraph(CXComment Comment);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXString clang_ParamCommandComment_getParamName(CXComment Comment);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint clang_ParamCommandComment_isParamIndexValid(CXComment Comment);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint clang_ParamCommandComment_getParamIndex(CXComment Comment);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint clang_ParamCommandComment_isDirectionExplicit(CXComment Comment);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CommentParamPassDirection clang_ParamCommandComment_getDirection(CXComment Comment);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXString clang_TParamCommandComment_getParamName(CXComment Comment);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint clang_TParamCommandComment_isParamPositionValid(CXComment Comment);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint clang_TParamCommandComment_getDepth(CXComment Comment);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint clang_TParamCommandComment_getIndex(CXComment Comment, uint Depth);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXString clang_VerbatimBlockLineComment_getText(CXComment Comment);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXString clang_VerbatimLineComment_getText(CXComment Comment);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXString clang_HTMLTagComment_getAsString(CXComment Comment);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXString clang_FullComment_getAsHTML(CXComment Comment);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXString clang_FullComment_getAsXML(CXComment Comment);

        #endregion
    }
}
