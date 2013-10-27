using System;

namespace LibClang.Indexer
{
    public class Indexer
    {
        #region Data

        private IntPtr _session;
        private Dll.IndexerCallbacks _cbs;
        private IntPtr _translationUnitPtr;
        private TranslationUnit _translationUnit;
        
        #endregion

        #region Constructor

        public Indexer(Index idx, string fileName)
        {
            Index = idx;
            Filename = fileName;
            _session = Index.CreateIndexingSession();

            MakeCallbacks();
        }
        
        #endregion

        private unsafe void MakeCallbacks()
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

        public int Parse()
        {
            _translationUnit = null;

            DateTime dt1 = DateTime.Now;

            int err = Dll.clang_indexSourceFile(_session, IntPtr.Zero, new Dll.IndexerCallbacks[]{_cbs}, 
                                            (uint)System.Runtime.InteropServices.Marshal.SizeOf(_cbs),
                                            (int)(0x01 | 0x10), Filename, null, 0, null, 0, out _translationUnitPtr, 0);

            DateTime dt2 = DateTime.Now;

            err = Dll.clang_indexSourceFile(_session, IntPtr.Zero, new Dll.IndexerCallbacks[] { _cbs },
                                            (uint)System.Runtime.InteropServices.Marshal.SizeOf(_cbs),
                                            (int)(0x01 | 0x10), Filename, null, 0, null, 0, out _translationUnitPtr, 0);

            DateTime dt3 = DateTime.Now;

            System.Diagnostics.Debug.WriteLine(string.Format("{0} vs {1}", (dt2 - dt1).Seconds, (dt3 - dt2).Seconds));

            return 0;
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
                if (_translationUnit == null && _translationUnitPtr != null)
                    _translationUnit = new TranslationUnit(Filename, _translationUnitPtr);
                return _translationUnit;
            }
        }

        #endregion

        #region event handlers

        private int OnIndexerAbortQuery(IntPtr clientData, IntPtr reserved)
        {
            return 0;
        }

        private void OnIndexerDiagnostic(IntPtr clientData, IntPtr diagnosticSet, IntPtr reserved)
        {

        }

        private IntPtr OnIndexerEnteredMainFile(IntPtr clientData, IntPtr mainFile, IntPtr reserved)
        {
            return IntPtr.Zero;
        }

        private unsafe IntPtr OnIndexerPPIncludedFile(IntPtr client_data, Dll.IndexerIncludeFileInfo* includedFileInfo)
        {
            IncludeFileInfo inc = new IncludeFileInfo(*includedFileInfo);
            return IntPtr.Zero;
        }
        
        private IntPtr OnIndexerImportedASTFile(IntPtr clientData, Dll.IndexerImportedAstFileInfo astFileInfo)
        {
            return IntPtr.Zero;
        }

        private IntPtr OnIndexerStartTranslationUnit(IntPtr clientData, IntPtr reserved)
        {
            return IntPtr.Zero;
        }

        private unsafe void OnIndexerDeclaration(IntPtr clientData, Dll.IndexerDeclarationInfo* a)
        {
            DeclInfo decl = new DeclInfo(*a);
         //   System.Diagnostics.Debug.WriteLine("Decl: " + decl.EntityInfo.ToString());
        }

        private unsafe void OnIndexerEntityReference(IntPtr clientData, Dll.IndexerEntityReferenceInfo* reference)
        {
            EntityReference refer = new EntityReference(*reference);
        //    System.Diagnostics.Debug.WriteLine("Refr: " + refer.ReferencedEntity.ToString());
        }

        #endregion

    }
}
