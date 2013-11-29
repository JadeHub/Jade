using System;
using System.Diagnostics;

namespace LibClang.Indexer
{
    public class Indexer
    {
        public interface IObserver
        {
            bool Abort(Indexer indexer);
            //void PPIncludeFile(Indexer indexer, IncludeFileInfo includeFile);
            void IncludeFile(Indexer indexer, string path, SourceLocation[] inclutionStack);
            void EntityDeclaration(Indexer indexer, DeclInfo decl);
            void EntityReference(Indexer indexer, EntityReference reference);
        }

        #region Data

        private Dll.IndexerCallbacks _cbs;
        private Dll.CXInclusionVisitor _includeCallback;
        
        private TranslationUnit _translationUnit;
        
        #endregion

        #region Constructor

        public Indexer(Index idx, TranslationUnit tu) : this()
        {            
            Index = idx;
            _translationUnit = tu;
            //_session = Index.CreateIndexingSession();
        }

        /*
        public Indexer(Index idx, string fileName): this()
        {
            Index = idx;
            Filename = fileName;
            _translationUnit = new LibClang.TranslationUnit(Index, Filename);
            _translationUnit.Parse();            
        }*/
        
        private unsafe Indexer()
        {
            _cbs = new Dll.IndexerCallbacks();
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
                
        private IObserver _observer;

        public int Parse(IObserver o, IntPtr session)
        {
            _observer = o;

            IntPtr tu = IntPtr.Zero;

            Dll.clang_getInclusions(_translationUnit.Handle, _includeCallback, IntPtr.Zero);

            /*
            int err = Dll.clang_indexSourceFile(session, IntPtr.Zero, new Dll.IndexerCallbacks[1] { _cbs },
                                            (uint)System.Runtime.InteropServices.Marshal.SizeOf(_cbs), 0, Filename, null, 0, null, 0, out tu, 0);
             */
            int err = Dll.clang_indexTranslationUnit(session, IntPtr.Zero, new Dll.IndexerCallbacks[1] { _cbs },
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

        private unsafe void OnCxxIncludeVisit(IntPtr fileHandle, Dll.SourceLocation* inclusionStack, uint includeStackSize, IntPtr clientData)
        {
            SourceLocation[] locs = new SourceLocation[includeStackSize];
            Dll.SourceLocation* incStackPtr = inclusionStack;
            for (uint i = 0; i < includeStackSize; i++)
            {
                locs[i] = new SourceLocation(*incStackPtr);
                incStackPtr++;
            }
            string path = Dll.clang_getFileName(fileHandle).ManagedString;
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
            string name = Dll.clang_getFileName(mainFile).ManagedString;
            Debug.WriteLine("Enter main file " + name);
            return IntPtr.Zero;
        }

        private unsafe IntPtr OnIndexerPPIncludedFile(IntPtr client_data, Dll.IndexerIncludeFileInfo* includedFileInfo)
        {
            //never called
         //   _observer.PPIncludeFile(this, new IncludeFileInfo(*includedFileInfo));
            return IntPtr.Zero;
        }
        
        private IntPtr OnIndexerImportedASTFile(IntPtr clientData, Dll.IndexerImportedAstFileInfo astFileInfo)
        {
            return IntPtr.Zero;
        }

        private IntPtr OnIndexerStartTranslationUnit(IntPtr clientData, IntPtr reserved)
        {
            Debug.WriteLine("StartTranslationUnit");
            return IntPtr.Zero;
        }

        private unsafe void OnIndexerDeclaration(IntPtr clientData, Dll.IndexerDeclarationInfo* decl)
        {
            _observer.EntityDeclaration(this, new DeclInfo(*decl));
        }

        private unsafe void OnIndexerEntityReference(IntPtr clientData, Dll.IndexerEntityReferenceInfo* reference)
        {
            _observer.EntityReference(this, new EntityReference(*reference));
        }

        #endregion

    }
}
