using System;
using System.Diagnostics;

namespace LibClang
{
    internal enum TranslationUnitFlags
    {
        /**
         * \brief Used to indicate that no special translation-unit options are
         * needed.
         */
        None = 0x0,

        /**
         * \brief Used to indicate that the parser should construct a "detailed"
         * preprocessing record, including all macro definitions and instantiations.
         *
         * Constructing a detailed preprocessing record requires more memory
         * and time to parse, since the information contained in the record
         * is usually not retained. However, it can be useful for
         * applications that require more detailed information about the
         * behavior of the preprocessor.
         */
        DetailedPreprocessingRecord = 0x01,

        /**
         * \brief Used to indicate that the translation unit is incomplete.
         *
         * When a translation unit is considered "incomplete", semantic
         * analysis that is typically performed at the end of the
         * translation unit will be suppressed. For example, this suppresses
         * the completion of tentative declarations in C and of
         * instantiation of implicitly-instantiation function templates in
         * C++. This option is typically used when parsing a header with the
         * intent of producing a precompiled header.
         */
        Incomplete = 0x02,

        /**
         * \brief Used to indicate that the translation unit should be built with an 
         * implicit precompiled header for the preamble.
         *
         * An implicit precompiled header is used as an optimization when a
         * particular translation unit is likely to be reparsed many times
         * when the sources aren't changing that often. In this case, an
         * implicit precompiled header will be built containing all of the
         * initial includes at the top of the main file (what we refer to as
         * the "preamble" of the file). In subsequent parses, if the
         * preamble or the files in it have not changed, \c
         * clang_reparseTranslationUnit() will re-use the implicit
         * precompiled header to improve parsing performance.
         */
        PrecompiledPreamble = 0x04,

        /**
         * \brief Used to indicate that the translation unit should cache some
         * code-completion results with each reparse of the source file.
         *
         * Caching of code-completion results is a performance optimization that
         * introduces some overhead to reparsing but improves the performance of
         * code-completion operations.
         */
        CacheCompletionResults = 0x08,

        /**
         * \brief Used to indicate that the translation unit will be serialized with
         * \c clang_saveTranslationUnit.
         *
         * This option is typically used when parsing a header with the intent of
         * producing a precompiled header.
         */
        ForSerialization = 0x10,

        /**
         * \brief DEPRECATED: Enabled chained precompiled preambles in C++.
         *
         * Note: this is a *temporary* option that is available only while
         * we are testing C++ precompiled preamble support. It is deprecated.
         */
        CXXChainedPCH = 0x20,

        /**
         * \brief Used to indicate that function/method bodies should be skipped while
         * parsing.
         *
         * This option can be used to search for declarations/definitions while
         * ignoring the usages.
         */
        SkipFunctionBodies = 0x40,

        /**
         * \brief Used to indicate that brief documentation comments should be
         * included into the set of code completions returned from this translation
         * unit.
         */
        IncludeBriefCommentsInCodeCompletion = 0x80
    };

    public sealed class Index : IDisposable
    {
        public readonly IntPtr Handle;

        public Index(bool excludeDeclarationsFromPch, bool displayDiagnostics)
        {
            Handle = Library.clang_createIndex(excludeDeclarationsFromPch ? 1 : 0, displayDiagnostics ? 1 : 0);
            Debug.Assert(Handle != IntPtr.Zero);
        }

        public void Dispose()
        {
            Library.clang_disposeIndex(Handle);
            GC.SuppressFinalize(this);
        }

        ~Index()
        {
            Library.clang_disposeIndex(Handle);
        }
        
        public IntPtr CreateIndexingSession()
        {
            return Library.clang_IndexAction_create(Handle);
        }
    }
}
