using System;
using System.Diagnostics;

namespace LibClang.Indexer
{
    public class Indexer
    {
        public interface IObserver
        {
            bool Abort(Indexer indexer);
            void PPIncludeFile(Indexer indexer, IncludeFileInfo includeFile);
            void EntityDeclaration(Indexer indexer, DeclInfo decl);
            void EntityReference(Indexer indexer, EntityReference reference);
        }

        #region Data

        private IntPtr _session;
        private Dll.IndexerCallbacks _cbs;
        
        private TranslationUnit _translationUnit;
        
        #endregion

        #region Constructor

        public Indexer(Index idx, TranslationUnit tu) : this()
        {            
            Index = idx;
            _translationUnit = tu;
            _session = Index.CreateIndexingSession();
        }

        public Indexer(Index idx, string fileName): this()
        {
            Index = idx;
            Filename = fileName;
            _session = Index.CreateIndexingSession();
        }
        
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
        }

        #endregion
                
        private IObserver _observer;

        public int Abc(IObserver o)
        {
            _observer = o;

            int err = Dll.clang_indexTranslationUnit(_session, IntPtr.Zero, new Dll.IndexerCallbacks[] { _cbs },
                                            (uint)System.Runtime.InteropServices.Marshal.SizeOf(_cbs), 0, _translationUnit.Handle);
            /*
            _translationUnit = null;

            DateTime dt1 = DateTime.Now;
            
            int err = Dll.clang_indexSourceFile(_session, IntPtr.Zero, new Dll.IndexerCallbacks[]{_cbs}, 
                                            (uint)System.Runtime.InteropServices.Marshal.SizeOf(_cbs),
                                            (int)(0x01 | 0x10), Filename, null, 0, null, 0, out _translationUnitPtr, 0x40);
            

            DateTime dt2 = DateTime.Now;
            System.Diagnostics.Debug.WriteLine(string.Format("{0}", (dt2 - dt1).Seconds));

            int err = Dll.clang_indexTranslationUnit(_session, IntPtr.Zero, new Dll.IndexerCallbacks[] { _cbs },
                                            (uint)System.Runtime.InteropServices.Marshal.SizeOf(_cbs), 0, _translationUnitPtr);

            DateTime dt3 = DateTime.Now;
            System.Diagnostics.Debug.WriteLine(string.Format("{0}", (dt3 - dt2).Seconds));
            */
            return err;
        }

        #region Properties
            
        public Index Index
        {
            get;
            private set;
        }

        public string Filename
        {
            get;
            private set;
        }
        
        public TranslationUnit TranslationUnit
        {
            get
            {
                return _translationUnit;
            }
        }

        #endregion

        #region event handlers

        private int OnIndexerAbortQuery(IntPtr clientData, IntPtr reserved)
        {
            return _observer.Abort(this) ? 1 : 0;
        }

        private void OnIndexerDiagnostic(IntPtr clientData, IntPtr diagnosticSet, IntPtr reserved)
        {
            
        }

        private IntPtr OnIndexerEnteredMainFile(IntPtr clientData, IntPtr mainFile, IntPtr reserved)
        {
            Debug.WriteLine("Enter main file");
            return IntPtr.Zero;
        }

        private unsafe IntPtr OnIndexerPPIncludedFile(IntPtr client_data, Dll.IndexerIncludeFileInfo* includedFileInfo)
        {
            _observer.PPIncludeFile(this, new IncludeFileInfo(*includedFileInfo));
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
