using System;
using System.Diagnostics;

namespace LibClang.Indexer
{
    /// <summary>
    /// A class that exposes Libclang's indexing functionality.
    /// Indexer provides a high level representation of a translation unit as a set of declarations and 
    /// a set of references to those declarations. 
    /// 
    /// The Indexer is suitable for implementing refactoring functionality, eg. Renaming a type and all references to that type.
    /// 
    /// Declarations and References are provided to the client via callbacks
    /// </summary>
    public class Indexer
    {
        /// <summary>
        /// Callback interface
        /// </summary>
        public interface IObserver
        {
            bool Abort(Indexer indexer);
            void IncludeFile(Indexer indexer, string path, SourceLocation[] inclutionStack);
            void EntityDeclaration(Indexer indexer, DeclInfo decl);
            void EntityReference(Indexer indexer, EntityReference reference);
        }

        #region Data

        //Libclang indexer callbacks
        private Library.IndexerCallbacks _cbs;
        private Library.CXInclusionVisitor _includeCallback;
        
        private TranslationUnit _translationUnit;

        //Client's callbacks
        private IObserver _observer;
        
        #endregion

        #region Constructor

        public Indexer(Index idx, TranslationUnit tu) : this()
        {            
            Index = idx;
            _translationUnit = tu;
        }

        private unsafe Indexer()
        {
            _cbs = new Library.IndexerCallbacks();
            _cbs.abortQuery = OnIndexerAbortQuery;
            _cbs.diagnostic = OnIndexerDiagnostic;
            _cbs.enterMainFile = OnIndexerEnteredMainFile;
            _cbs.ppIncludedFile = OnIndexerPPIncludedFile;
            _cbs.astImportFile = OnIndexerImportedASTFile;
            _cbs.startTU = OnIndexerStartTranslationUnit;
            _cbs.index = OnIndexerDeclaration;
            _cbs.entityRef = OnIndexerEntityReference;

            _includeCallback = OnCxxIncludeVisit;
        }

        #endregion
                
        
        public int Parse(IObserver o, IntPtr session)
        {
            _observer = o;

            IntPtr tu = IntPtr.Zero;

            Library.clang_getInclusions(_translationUnit.Handle, _includeCallback, IntPtr.Zero);

            int err = Library.clang_indexTranslationUnit(session, IntPtr.Zero, new Library.IndexerCallbacks[1] { _cbs },
                                            (uint)System.Runtime.InteropServices.Marshal.SizeOf(_cbs), 0x2, _translationUnit.Handle);
            return err;
        }

        #region Properties
            
        public Index Index
        {
            get;
            private set;
        }

        /*
        public string Filename
        {
            get;
            private set;
        }*/
        
        public TranslationUnit TranslationUnit
        {
            get
            {
                return _translationUnit;
            }
        }

        #endregion

        #region event handlers

        private unsafe void OnCxxIncludeVisit(IntPtr fileHandle, Library.SourceLocation* inclusionStack, uint includeStackSize, IntPtr clientData)
        {
            SourceLocation[] locs = new SourceLocation[includeStackSize];
            Library.SourceLocation* incStackPtr = inclusionStack;
            for (uint i = 0; i < includeStackSize; i++)
            {
               // locs[i] = new SourceLocation(*incStackPtr);
                incStackPtr++;
            }
            string path = Library.clang_getFileName(fileHandle).ManagedString;
            if (includeStackSize > 0)
                _observer.IncludeFile(this, path, locs);
        }

        private int OnIndexerAbortQuery(IntPtr clientData, IntPtr reserved)
        {
            return _observer.Abort(this) ? 1 : 0;
        }

        private void OnIndexerDiagnostic(IntPtr clientData, IntPtr diagnosticSet, IntPtr reserved)
        {
            
        }

        private IntPtr OnIndexerEnteredMainFile(IntPtr clientData, IntPtr mainFile, IntPtr reserved)
        {
            string name = Library.clang_getFileName(mainFile).ManagedString;
            Debug.WriteLine("Enter main file " + name);
            return IntPtr.Zero;
        }

        private unsafe IntPtr OnIndexerPPIncludedFile(IntPtr client_data, Library.IndexerIncludeFileInfo* includedFileInfo)
        {
            //never called
            return IntPtr.Zero;
        }
        
        private IntPtr OnIndexerImportedASTFile(IntPtr clientData, Library.IndexerImportedAstFileInfo astFileInfo)
        {
            return IntPtr.Zero;
        }

        private IntPtr OnIndexerStartTranslationUnit(IntPtr clientData, IntPtr reserved)
        {
            Debug.WriteLine("StartTranslationUnit");
            return IntPtr.Zero;
        }

        private unsafe void OnIndexerDeclaration(IntPtr clientData, Library.IndexerDeclarationInfo* decl)
        {
            _observer.EntityDeclaration(this, new DeclInfo(*decl, _translationUnit.ItemFactory));
        }

        private unsafe void OnIndexerEntityReference(IntPtr clientData, Library.IndexerEntityReferenceInfo* reference)
        {
            _observer.EntityReference(this, new EntityReference(*reference, _translationUnit.ItemFactory));
        }

        #endregion

    }
}
