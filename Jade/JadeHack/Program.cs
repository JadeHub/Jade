using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LibClang;
using LibClang.Indexer;

namespace JadeHack
{
    public class IndexObserver : Indexer.IObserver
    {
        public bool Abort(Indexer indexer)
        {
            return false;
        }

        public void PPIncludeFile(Indexer indexer, IncludeFileInfo includeFile)
        {

        }

        public void EntityDeclaration(Indexer indexer, DeclInfo decl)
        {

        }

        public void EntityReference(Indexer indexer, EntityReference reference)
        {

        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Index idx = new Index(true, true);

            //TranslationUnit tu = new TranslationUnit(idx, @"C:\Code\clang\llvm\tools\clang\tools\libclang\CIndex.cpp");
            TranslationUnit tu = new TranslationUnit(idx, @"C:\Code\GitHub\Jade\TestData\CppTest\test.cpp");
            Indexer indexer = new Indexer(idx, tu);

            tu.Parse();

            indexer.Abc(new IndexObserver());
            
        }
    }
}
