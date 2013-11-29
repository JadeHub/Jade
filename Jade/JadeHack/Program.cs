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

        public void IncludeFile(LibClang.Indexer.Indexer indexer, string path, LibClang.SourceLocation[] includeStack)
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
            CppView.ProjectSymbolTable st = new CppView.ProjectSymbolTable();
            CppView.IProjectSourceIndex si = new CppView.ProjectSourceIndex(st);

            CppView.IndexBuilder builder = new CppView.IndexBuilder(si);

            builder.AddSourceFile(new JadeUtils.IO.FileHandle(@"C:\Code\GitHub\Jade\TestData\CppTest\test2.cpp"), CppView.IndexBuilderItemPriority.Immediate);
     //       builder.AddSourceFile(new JadeUtils.IO.FileHandle(@"C:\Code\GitHub\Jade\TestData\CppTest\main.cpp"), CppView.IndexBuilderItemPriority.Immediate);

            st.Dump();
            //pi.Dump();
        }
    }
}
