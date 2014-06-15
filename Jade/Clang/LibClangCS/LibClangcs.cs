﻿using System;
using System.Runtime.InteropServices;
using IndexClientAstFile = System.IntPtr;
using IndexClientContainer = System.IntPtr;
using IndexClientFile = System.IntPtr;
using IndexingSession = System.IntPtr;

namespace LibClang
{
    /// <summary>
    /// Provides access to libclang.dll
    /// </summary>
    internal class Library
    {
        #region CXString

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        internal unsafe struct ClangString
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
        internal static extern IntPtr clang_getCString(ClangString str);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern void clang_disposeString(ClangString str);

        #endregion

        #region CXIndex

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr clang_createIndex(int excludeDeclarationsFromPch, int displayDiagnostics);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void clang_disposeIndex(IntPtr index);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr clang_IndexAction_create(IntPtr index);
        
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
        internal static extern IntPtr clang_parseTranslationUnit(IntPtr index, string sourceFilename,
            string[] clangCommandLineArgs, int numClangCommandLineArgs, UnsavedFile[] unsavedFiles, uint numUnsavedFiles, int options);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void clang_disposeTranslationUnit(IntPtr tu);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern int clang_reparseTranslationUnit(IntPtr tu, uint numUnsavedFiles, UnsavedFile[] unsavedFiles, int options);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern IntPtr clang_createTranslationUnitFromSourceFile(IntPtr index, string sourceFilename,
            int numClangCommandLineArgs, string[] clangCommandLineArgs, uint numUnsavedFiles, UnsavedFile[] unsavedFiles);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXCursor clang_getTranslationUnitCursor(IntPtr tu);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern ClangString clang_getTranslationUnitSpelling(IntPtr tu);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern IntPtr clang_getIncludedFile(CXCursor c);

        #endregion

        #region CXFile

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern ClangString clang_getFileName(IntPtr fileHandle);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern long clang_getFileTime(IntPtr fileHandle);
                
        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern IntPtr clang_getFile(IntPtr tu, string filename);
                
        #endregion

        #region CXSourceRange

        [StructLayout(LayoutKind.Sequential)]
        internal struct SourceRange
        {
            static public SourceRange NullRange;

            static SourceRange()
            {
                NullRange = Library.clang_getNullRange();
            }

            public bool IsNull { get { return this == NullRange; } }

            readonly IntPtr data0;
            readonly IntPtr data1;
            readonly uint beginIntData;
            readonly uint endIntData;

            public static bool operator ==(SourceRange left, SourceRange right)
            {
                return Library.clang_equalRanges(left, right) != 0;
            }

            public static bool operator !=(SourceRange left, SourceRange right)
            {
                return !(left == right);
            }

            public override bool Equals(object obj)
            {
                if (obj != null && obj is SourceRange)
                {
                    return (SourceRange)obj == this;
                }
                return false;
            }

            public override int GetHashCode()
            {
                return (int)((beginIntData << 16) | (endIntData & 0xff));
            }
        }

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern SourceRange clang_getNullRange();

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint clang_equalRanges(SourceRange r1, SourceRange r2);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern SourceLocation clang_getRangeStart(SourceRange range);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern SourceLocation clang_getRangeEnd(SourceRange range);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern SourceRange clang_getRange(SourceLocation start, SourceLocation end);
        
        #endregion

        #region CXSourceLocation

        [StructLayout(LayoutKind.Sequential)]
        public struct SourceLocation
        {
            static public SourceLocation NullLocation;

            static SourceLocation()
            {
                NullLocation = Library.clang_getNullLocation();
            }

            public bool IsNull { get { return this == NullLocation; } }

            readonly IntPtr data0;
            readonly IntPtr data1;
            readonly uint data2;

            public static bool operator ==(SourceLocation left, SourceLocation right)
            {
                return Library.clang_equalLocations(left, right) != 0;
            }

            public static bool operator !=(SourceLocation left, SourceLocation right)
            {
                return !(left == right);
            }

            public override bool Equals(object obj)
            {
                if (obj != null && obj is SourceLocation)
                {
                    return (SourceLocation)obj == this;
                }
                return false;
            }

            public override int GetHashCode()
            {
                return data0.ToInt32();
            }
        }

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern SourceLocation clang_getLocationForOffset(IntPtr tu, IntPtr fileHandle, uint offset);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern SourceLocation clang_getNullLocation();

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint clang_equalLocations(SourceLocation l1, SourceLocation l2);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal unsafe static extern void clang_getInstantiationLocation(SourceLocation location, IntPtr* file,
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
        internal static extern ClangString clang_getTypeSpelling(CXType c);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType clang_getCanonicalType(CXType t);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXType clang_getPointeeType(CXType t);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXCursor clang_getTypeDeclaration(CXType t);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ClangString clang_getTypeKindSpelling(LibClang.TypeKind k);

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
        public struct CXCursor
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
                    return !IsNull && kind != CursorKind.NoDeclFound;
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
        internal delegate LibClang.Cursor.ChildVisitResult CursorVisitor(CXCursor cursor, CXCursor parent, IntPtr data);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void ChildVistor(CXCursor cursor, CXCursor parent, IntPtr clientData);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CursorKind clang_getCursorKind(CXCursor c);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint clang_isReference(CursorKind ck);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_isInvalid(CursorKind kc);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern ClangString clang_getCursorSpelling(CXCursor c);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern uint clang_visitChildren(CXCursor parent, CursorVisitor visitor, IntPtr clientData);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern SourceLocation clang_getCursorLocation(CXCursor cursor);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXCursor clang_getCursorReferenced(CXCursor c);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern SourceRange clang_getCursorExtent(CXCursor cursor);

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
        internal static extern ClangString clang_getCursorUSR(CXCursor c);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXCursor clang_getCanonicalCursor(CXCursor c);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXCursor clang_getCursor(IntPtr tu, SourceLocation loc);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint clang_equalCursors(CXCursor c1, CXCursor c2);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint clang_hashCursor(CXCursor c);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXXAccessSpecifier clang_getCXXAccessSpecifier(CXCursor c);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint clang_Cursor_getNumArguments(CXCursor c);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXCursor clang_Cursor_getArgument(CXCursor C, uint i);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXType clang_getCursorResultType(CXCursor c);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern ClangString clang_getCursorDisplayName(CXCursor c);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static unsafe extern void clang_getOverriddenCursors(CXCursor c, CXCursor** overridden, uint* num_overridden);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static unsafe extern void clang_disposeOverriddenCursors(CXCursor* overridden);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static unsafe extern uint clang_isVirtualBase(CXCursor c);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static unsafe extern uint clang_CXXMethod_isPureVirtual(CXCursor c);
        
        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static unsafe extern uint clang_CXXMethod_isStatic(CXCursor C);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static unsafe extern uint clang_CXXMethod_isVirtual(CXCursor C);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static unsafe extern CXType clang_getEnumDeclIntegerType(CXCursor C);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static unsafe extern int clang_Cursor_isDynamicCall(CXCursor C); //returns int not uint

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static unsafe extern uint clang_Cursor_isVariadic(CXCursor C);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static unsafe extern CXCursor clang_getSpecializedCursorTemplate(CXCursor c);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static unsafe extern CursorKind clang_getTemplateCursorKind(CXCursor c);
        
        #endregion

        #region CXTokens

        [StructLayout(LayoutKind.Sequential)]
        internal struct Token
        {
            readonly uint data0;
            readonly uint data1;
            readonly uint data2;
            readonly uint data3;

            readonly IntPtr ptr_data;

            public static bool operator ==(Token left, Token right)
            {
                return left.data0 == right.data0 &&
                    left.data1 == right.data1 &&
                    left.data2 == right.data2 &&
                    left.data3 == right.data3 &&
                    left.ptr_data == right.ptr_data;
            }

            public static bool operator !=(Token left, Token right)
            {
                return !(left == right);
            }

            public override bool Equals(object obj)
            {
                if (obj != null && obj is Token)
                {
                    return (Token)obj == this;
                }
                return false;
            }

            public override int GetHashCode()
            {
                return (int)ptr_data.ToInt32();
            }
        }

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern TokenKind clang_getTokenKind(Token tok);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern ClangString clang_getTokenSpelling(IntPtr tu, Token tok);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern SourceLocation clang_getTokenLocation(IntPtr tu, Token tok);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern SourceRange clang_getTokenExtent(IntPtr tu, Token tok);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static unsafe extern void clang_tokenize(IntPtr tu, SourceRange Range, Token** Tokens, uint* NumTokens);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static unsafe extern void clang_disposeTokens(IntPtr tu, Token* Tokens, uint NumTokens);

        #endregion

        #region Indexing

        [StructLayout(LayoutKind.Sequential)]
        internal struct IndexLocation
        {
            readonly IntPtr data0;
            readonly IntPtr data1;
            readonly uint data2;

            public bool Equals(IndexLocation other)
            {
                return data0 == other.data0 && data1 == other.data1 && data2 == other.data2;
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        internal unsafe struct IndexerIncludeFileInfo
        {
            internal IndexLocation location;
            internal sbyte* fileName;
            internal IntPtr file;
            internal int isImport;
            internal int isAngled;
            internal int isModuleImport;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        internal struct IndexerImportedAstFileInfo
        {
            internal IntPtr file;
            internal IntPtr module; 
            internal SourceLocation location;
            internal int isImplicit;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        internal struct IndexerAttributeInfo
        {
            internal Indexer.AttributeKind kind;
            internal CXCursor cursor;
            internal IndexLocation location;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        internal unsafe struct EntityInfo
        {
            internal Indexer.EntityKind kind;
            internal Indexer.EntityCXXTemplateKind templateKind;
            internal Indexer.EntityLanguage language;
            internal sbyte* name;
            internal sbyte* USR;
            internal CXCursor cursor;
            internal IndexerAttributeInfo** attributes;
            internal int numAttributes;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal unsafe struct IndexerContainerInfo
        {
            internal CXCursor cursor;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal unsafe struct IndexerDeclarationInfo
        {
            internal EntityInfo* entityInfo;
            internal CXCursor cursor;
            internal IndexLocation location;
            internal IndexerContainerInfo* semanticContainer;
            internal IndexerContainerInfo* lexicalContainer;
            internal int isRedeclaration;
            internal int isDefinition;
            internal int isContainer;
            internal IndexerContainerInfo* declAsContainer;
            internal int isImplicit;
            internal IndexerAttributeInfo** attributes;
            internal int numAttributes;
            internal int flags;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal unsafe struct IndexerEntityReferenceInfo
        {
            internal Indexer.EntityReferenceKind kind;
            internal CXCursor cursor;
            internal IndexLocation location;
            internal EntityInfo* referencedEntity;
            internal EntityInfo* parentEntity;
            internal IndexerContainerInfo* container;
        }
                
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int IndexerAbort(IntPtr clientData, IntPtr reserved);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void IndexerDiagnostic(IntPtr clientData, IntPtr diagnosticSet, IntPtr reserved);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate IndexClientFile IndexerEnteredMainFile(IntPtr clientData, IntPtr mainFile, IntPtr reserved);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal unsafe delegate IndexClientFile IndexerPPIncludedFile(IntPtr client_data, IndexerIncludeFileInfo* includedFileInfo);
  
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate IndexClientAstFile IndexerImportedASTFile(IntPtr clientData, IndexerImportedAstFileInfo astFileInfo);
  
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate IndexClientContainer IndexerStartTranslationUnit(IntPtr clientData, IntPtr reserved);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal unsafe delegate void IndexerDeclaration(IntPtr clientData, IndexerDeclarationInfo* a);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal unsafe delegate void IndexerEntityReference(IntPtr clientData, IndexerEntityReferenceInfo* reference);
        
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
        internal static extern int clang_indexSourceFile(IndexingSession session, IntPtr clientData,
                                IndexerCallbacks[] cbs, uint cbsSize, uint indexOptions,
                                string fileName, string[] cmdLineArgs, int cmdLineCount,
                                UnsavedFile[] unsavedFiles, uint numUnsavedFiles, out IntPtr translationUnit, uint tuOptions);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern SourceLocation clang_indexLoc_getCXSourceLocation(IndexLocation idxLoc);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern int clang_indexTranslationUnit(IndexingSession session, IntPtr clientData,
                                IndexerCallbacks[] cbs, uint cbsSize, uint indexOptions, IntPtr translationUnit);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal unsafe delegate void CXInclusionVisitor(IntPtr fileHandle, SourceLocation* inclusion_stack, uint include_len,  IntPtr clientData);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void clang_getInclusions(IntPtr tu, CXInclusionVisitor visitor, IntPtr clientData);

        #endregion

        #region FindReferencesInFile

        internal enum CXVisitorResult
        {
            CXVisit_Break,
            CXVisit_Continue
        };

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate CXVisitorResult Visit(IntPtr context, CXCursor cursor, SourceRange range);

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
        internal static extern CXResult clang_findReferencesInFile(CXCursor c, IntPtr f, CXCursorAndRangeVisitor visitor);

        #endregion

        #region CXDiagnostic

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint clang_getNumDiagnostics(IntPtr tu);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr clang_getDiagnostic(IntPtr tu, uint index);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern Diagnostic.Severity clang_getDiagnosticSeverity(IntPtr diagnostic);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern SourceLocation clang_getDiagnosticLocation(IntPtr diagnostic);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern ClangString clang_getDiagnosticSpelling(IntPtr diagnostic);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal unsafe static extern ClangString clang_getDiagnosticOption(IntPtr diagnostic, ClangString* disable);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint clang_getDiagnosticCategory(IntPtr diagnostic);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern ClangString clang_getDiagnosticCategoryName(uint category);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern ClangString clang_getDiagnosticCategoryText(IntPtr diagnostic);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint clang_getDiagnosticNumRanges(IntPtr diagnostic);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern SourceRange clang_getDiagnosticRange(IntPtr diagnostic, uint rangeIdx);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint clang_getDiagnosticNumFixIts(IntPtr diagnostic);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal unsafe static extern ClangString clang_getDiagnosticFixIt(IntPtr diagnostic, uint fixIt, SourceRange* replacementRange);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void clang_disposeDiagnostic(IntPtr dianostic);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern ClangString clang_formatDiagnostic(IntPtr diagnostic, uint options);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint clang_defaultDiagnosticDisplayOptions();

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint clang_getNumDiagnosticsInSet(IntPtr handle);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr clang_getDiagnosticInSet(IntPtr set, uint index);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void clang_disposeDiagnosticSet(IntPtr handle);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr clang_getDiagnosticSetFromTU(IntPtr Unit);  

        #endregion

    }
}
