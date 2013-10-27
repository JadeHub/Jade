using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LibClang;
using LibClang.Indexer;

namespace JadeHack
{
    class Program
    {
        static void Main(string[] args)
        {
            Index idx = new Index(true, true);

            //Indexer indexer = new Indexer(idx, "C:\\Code\\clang\\llvm\\tools\\clang\\tools\\libclang\\CIndex.cpp");
            Indexer indexer = new Indexer(idx, @"C:\Code\clang\llvm\tools\clang\tools\libclang\CIndex.cpp");

            indexer.Parse();
        }
    }
}
