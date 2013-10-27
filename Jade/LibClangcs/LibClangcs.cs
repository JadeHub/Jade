using System;
using System.Runtime.InteropServices;
using IndexClientAstFile = System.IntPtr;
using IndexClientContainer = System.IntPtr;
using IndexClientFile = System.IntPtr;
using IndexingSession = System.IntPtr;

namespace LibClang
{
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
                    string res = Marshal.PtrToStringAnsi((IntPtr)Spelling);
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
            readonly CursorKind kind;
            readonly int xdata;
            readonly IntPtr data0, data1, data2;
        };

        [StructLayout(LayoutKind.Sequential)]
        internal struct SourceLocation
        {
            readonly IntPtr data0;
            readonly IntPtr data1;
            readonly uint data2;

            public bool Equals(SourceLocation other)
            {
                return data0 == other.data0 && data1 == other.data1 && data2 == other.data2;
            }
        }
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate LibClang.Cursor.ChildVisitResult CursorVisitor(Cursor cursor, Cursor parent, IntPtr data);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void ChildVistor(Cursor cursor, Cursor parent, IntPtr clientData);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern void clang_disposeString(ClangString str);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern IntPtr clang_createIndex(int excludeDeclarationsFromPch, int displayDiagnostics);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern void clang_disposeIndex(IntPtr index);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern IntPtr clang_createTranslationUnitFromSourceFile(IntPtr index, string sourceFilename, 
            int numClangCommandLineArgs, string[] clangCommandLineArgs, uint numUnsavedFiles, UnsavedFile[] unsavedFiles);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern Cursor clang_getTranslationUnitCursor(IntPtr tu);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern CursorKind clang_getCursorKind(Cursor c);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern ClangString clang_getCursorSpelling(Cursor c);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern uint clang_visitChildren(Cursor parent, CursorVisitor visitor, IntPtr clientData);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern ClangString clang_getFileName(IntPtr fileHandle);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern long clang_getFileTime(IntPtr fileHandle);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern SourceLocation clang_getCursorLocation(Cursor cursor);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal unsafe static extern void clang_getInstantiationLocation(SourceLocation location, IntPtr* file, 
                                                            out uint line, out uint column, out uint offset);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern IntPtr clang_getFile(IntPtr tu, string filename);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern ClangString clang_getTranslationUnitSpelling(IntPtr tu);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern IntPtr clang_IndexAction_create(IntPtr index);

        
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

        #endregion
    }
}
