using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LibClang.Test
{
    /// <summary>
    /// Summary description for Indexer
    /// </summary>
    [TestClass]
    public class IndexerTests
    {
        Index index;
        //TranslationUnit testcpp;

        [TestInitialize()]
        public void InitializeTest()
        {
            index = new Index(true, false);
            //testcpp = index.CreateTranslationUnit("TestFiles\\test.cc");
            Assert.AreNotEqual(index.Handle, IntPtr.Zero);
          //  Assert.AreNotEqual(testcpp.Handle, IntPtr.Zero);
        }

        [TestCleanup()]
        public void CleanupTest()
        {
            index.Dispose();
            index = null;
        }

        [TestMethod]
        public void TestMethod1()
        {
            Indexer indexer = new Indexer(index, "TestFiles\\test.cc");
            indexer.Parse();
        }
    }
}
