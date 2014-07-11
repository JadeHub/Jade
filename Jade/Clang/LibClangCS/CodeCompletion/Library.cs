using System;
using System.Runtime.InteropServices;

namespace LibClang.CodeCompletion
{
    using CXCompletionString = System.IntPtr;
    using CXString = LibClang.Library.CXString;
    using CXCursor = LibClang.Library.CXCursor;
    using CXUnsavedFile = LibClang.Library.UnsavedFile;
    using CXTranslationUnit = System.IntPtr;
    using CXDiagnostic = System.IntPtr;

    internal class Library
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        internal unsafe struct CXCompletionResult
        {
            internal readonly CursorKind CursorKind;
            internal readonly CXCompletionString CompletionString;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        internal unsafe struct CXCodeCompleteResults
        {
            internal CXCompletionResult* Results;
            internal readonly uint NumberResults;
        }

        internal enum Flags
        {
            IncludeMacros,
            IncludeCodePatterns,
            IncludeBriefComments
        }

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern ChunkKind clang_getCompletionChunkKind(CXCompletionString cs, uint chunkNumber);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXString clang_getCompletionChunkText(CXCompletionString cs, uint chunkNumber);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXCompletionString clang_getCompletionChunkCompletionString(CXCompletionString cs, uint chunk_number);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint clang_getNumCompletionChunks(CXCompletionString cs);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint clang_getCompletionPriority(CXCompletionString cs);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)] 
        internal static extern  AvailabilityKind clang_getCompletionAvailability(CXCompletionString cs);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)] 
        internal static extern uint clang_getCompletionNumAnnotations(CXCompletionString cs);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXString clang_getCompletionAnnotation(CXCompletionString cs, uint annotationNumber);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal unsafe static extern CXString clang_getCompletionParent(CXCompletionString cs, out CursorKind kind);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXString clang_getCompletionBriefComment(CXCompletionString cs);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal static extern CXCompletionString clang_getCursorCompletionString(CXCursor cursor);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)] 
        internal static extern uint clang_defaultCodeCompleteOptions();

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)] 
        internal unsafe static extern CXCodeCompleteResults* clang_codeCompleteAt(CXTranslationUnit tu,
                                                                                string filename,
                                                                                uint line,
                                                                                uint column,
                                                                                CXUnsavedFile[] unsavedFiles,
                                                                                uint num_unsaved_files,
                                                                                uint options);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal unsafe static extern void clang_disposeCodeCompleteResults(CXCodeCompleteResults* r);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal unsafe static extern uint clang_codeCompleteGetNumDiagnostics(CXCodeCompleteResults* results);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal unsafe static extern CXDiagnostic clang_codeCompleteGetDiagnostic(CXCodeCompleteResults* results, uint Index);

        [DllImport("libclang", CallingConvention = CallingConvention.Cdecl)]
        internal unsafe static extern Int64 clang_codeCompleteGetContexts(CXCodeCompleteResults* results);

        /*
        unsigned long long clang_codeCompleteGetContexts(
                                                CXCodeCompleteResults *Results);


        enum CXCursorKind clang_codeCompleteGetContainerKind(
                                                 CXCodeCompleteResults *Results,
                                                     unsigned *IsIncomplete);

        CXString clang_codeCompleteGetContainerUSR(CXCodeCompleteResults *Results);
        */

    }
}
