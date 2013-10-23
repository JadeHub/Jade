using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibClang
{
    public class Indexer
    {
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

        public Indexer(Index idx, string fileName)
        {
            Index = idx;
            Filename = fileName;
        }

        public int Parse()
        {
            IntPtr session = Index.CreateIndexingSession();

            Dll.IndexerCallbacks cbs = new Dll.IndexerCallbacks();
            cbs.enterMainFile = OnEnteredMainFile;
            IntPtr tu ;
            int sz = System.Runtime.InteropServices.Marshal.SizeOf(cbs);
            System.Diagnostics.Debug.WriteLine(sz.ToString());
            int err = Dll.clang_indexSourceFile(session, IntPtr.Zero, new Dll.IndexerCallbacks[]{cbs}, (uint)System.Runtime.InteropServices.Marshal.SizeOf(cbs),
                                        0, Filename, null, 0, null, 0, out tu, 0);

            return 0;
        }

        #region event handlers

        private IntPtr OnEnteredMainFile(IntPtr clientData, IntPtr mainFile, IntPtr reserved)
        {
            return IntPtr.Zero;
        }

        #endregion

    }
}
