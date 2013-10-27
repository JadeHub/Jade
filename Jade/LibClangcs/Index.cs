using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibClang
{
    

    public class Index : IDisposable
    {
        public readonly IntPtr Handle;

        public Index(bool excludeDeclarationsFromPch, bool displayDiagnostics)
        {
            Handle = Dll.clang_createIndex(excludeDeclarationsFromPch ? 1 : 0, displayDiagnostics ? 1 : 0);
        }

        public void Dispose()
        {
            Dll.clang_disposeIndex(Handle);
            GC.SuppressFinalize(this);
        }

        ~Index()
        {
            Dll.clang_disposeIndex(Handle);
        }

        public TranslationUnit CreateTranslationUnit(string filename)
        {
            if (!System.IO.File.Exists(filename))
            {
                throw new System.IO.FileNotFoundException("Couldn't find input file.", filename);
            }
            
            return new TranslationUnit(
                filename,
                Dll.clang_createTranslationUnitFromSourceFile(
                    Handle,
                    filename,
                    0,
                    null,
                    0,
                    null));
        }

        public IntPtr CreateIndexingSession()
        {
            return Dll.clang_IndexAction_create(Handle);
        }
    }
}
