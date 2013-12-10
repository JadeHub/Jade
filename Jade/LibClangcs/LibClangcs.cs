using System;
using System.Runtime.InteropServices;
using IndexClientAstFile = System.IntPtr;
using IndexClientContainer = System.IntPtr;
using IndexClientFile = System.IntPtr;
using IndexingSession = System.IntPtr;

namespace LibClang
{
    public class Helper
    {

    }

    internal class Dll
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        internal unsafe struct ClangString
        {
            readonly void* Spelling;
            readonly uint Flags;

            internal string ManagedString
            {
                get
                {
                    //string res = Marshal.PtrToStringAnsi((IntPtr)Spelling);
                    string res = Marshal.PtrToStringAnsi(clang_getCString(this));
                    Dll.clang_disposeString(this);
                    return res;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        internal struct UnsavedFile
        {
            internal string Filename;
            internal string Contents;
            internal ulong Length;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct Cursor
        {
            static private Cursor nullCursor;

            static Cursor()
            {
                nullCursor = clang_getNullCursor();
            }

            public bool IsNull()
            {
                return this == nullCursor;
            }

            readonly CursorKind kind;
            readonly int xdata;
            readonly IntPtr data0, data1, data2;

            public static bool operator ==(Cursor left, Cursor right)
            {
                return Dll.clang_equalCursors(left, right) != 0;
            }

            public static bool operator !=(Cursor left, Cursor right)
            {
                return !(left == right);
            }

            public override bool Equals(object obj)
            {
                if (obj != null && obj is Cursor)
                {
                    return (Cursor)obj == this;
                }
                return false;
            }

            public override int GetHashCode()
            {
                return (int)Dll.clang_hashCursor(this);    
            }
        };

        [StructLayout(LayoutKind.Sequential)]
        internal struct Type
        {
            internal LibClang.TypeKind kind;
            readonly IntPtr data0;
            readonly IntPtr data1;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct SourceLocation
        {
            readonly IntPtr data0;
            readonly IntPtr data1;
            readonly uint data2;

            public bool Equals(SourceLocation other)
            {
                return data0 == other.data0 && data1 == other.data1 && data2 == other.data2;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SourceRange
        {
            readonly IntPtr data0;
            readonly IntPtr data1;
            readonly uint beginIntData;
            readonly uint endIntData;
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate LibClang.Cursor.ChildVisitResult CursorVisitor(Cursor cursor, Cursor parent, IntPtr data);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void ChildVistor(Cursor cursor, Cursor parent, IntPtr clientData);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern void clang_disposeString(ClangString str);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern IntPtr clang_getCString(ClangString str);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr clang_createIndex(int excludeDeclarationsFromPch, int displayDiagnostics);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void clang_disposeIndex(IntPtr index);

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
        internal static extern Cursor clang_getTranslationUnitCursor(IntPtr tu);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CursorKind clang_getCursorKind(Cursor c);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint clang_isReference(CursorKind ck);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_isInvalid(CursorKind kc);
                
        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern ClangString clang_getCursorSpelling(Cursor c);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern uint clang_visitChildren(Cursor parent, CursorVisitor visitor, IntPtr clientData);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern ClangString clang_getFileName(IntPtr fileHandle);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern long clang_getFileTime(IntPtr fileHandle);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern SourceLocation clang_getCursorLocation(Cursor cursor);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern Cursor clang_getCursorReferenced(Cursor c);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern SourceRange clang_getCursorExtent(Cursor cursor);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern SourceRange clang_getNullRange();

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        public static extern SourceLocation clang_getRangeStart(SourceRange range);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        public static extern SourceLocation clang_getRangeEnd(SourceRange range);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal unsafe static extern void clang_getInstantiationLocation(SourceLocation location, IntPtr* file, 
                                                            out uint line, out uint column, out uint offset);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern IntPtr clang_getFile(IntPtr tu, string filename);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern ClangString clang_getTranslationUnitSpelling(IntPtr tu);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr clang_IndexAction_create(IntPtr index);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        public static extern Type clang_getCursorType(Cursor c);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        public static extern Cursor clang_getNullCursor();

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        public static extern Cursor clang_getCursorLexicalParent(Cursor c);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        public static extern Cursor clang_getCursorSemanticParent(Cursor c);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint clang_equalTypes(Type a, Type b);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        public static extern Type clang_getCanonicalType(Type t);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        public static extern Type clang_getPointeeType(Type t);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        public static extern Cursor clang_getTypeDeclaration(Type t);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern ClangString clang_getTypeKindSpelling(LibClang.TypeKind k);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        public static extern Type clang_getResultType(Type t);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint clang_isCursorDefinition(Cursor c);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern Cursor clang_getCursorDefinition(Cursor c);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern ClangString clang_getCursorUSR(Cursor c);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern Cursor clang_getCanonicalCursor(Cursor c);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern SourceLocation clang_getLocationForOffset(IntPtr tu, IntPtr fileHandle, uint offset);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern Cursor clang_getCursor(IntPtr tu, SourceLocation loc);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint clang_equalCursors(Dll.Cursor c1, Dll.Cursor c2);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint clang_hashCursor(Dll.Cursor c);
                        
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
            internal Cursor cursor;
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
            internal Cursor cursor;
            internal IndexerAttributeInfo** attributes;
            internal int numAttributes;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal unsafe struct IndexerContainerInfo
        {
            internal Cursor cursor;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal unsafe struct IndexerDeclarationInfo
        {
            internal EntityInfo* entityInfo;
            internal Cursor cursor;
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
            internal Cursor cursor;
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
            /**
            * \brief Called periodically to check whether indexing should be aborted.
            * Should return 0 to continue, and non-zero to abort.
            */
            internal IndexerAbort abortQuery;

            /**
            * \brief Called at the end of indexing; passes the complete diagnostic set.
            */
            internal IndexerDiagnostic diagnostic;

            internal IndexerEnteredMainFile enterMainFile;

            /**
            * \brief Called when a file gets \#included/\#imported.
            */
            internal IndexerPPIncludedFile ppIncludedFile;

            /**
            * \brief Called when a AST file (PCH or module) gets imported.
            * 
            * AST files will not get indexed (there will not be callbacks to index all
            * the entities in an AST file). The recommended action is that, if the AST
            * file is not already indexed, to initiate a new indexing job specific to
            * the AST file.
            */
            internal IndexerImportedASTFile astImportFile;

            /**
            * \brief Called at the beginning of indexing a translation unit.
            */
            internal IndexerStartTranslationUnit startTU;

            internal IndexerDeclaration index;

            /**
            * \brief Called to index a reference of an entity.
            */
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
    }
}
