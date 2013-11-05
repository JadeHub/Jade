using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LibClang.Test
{
    /// <summary>
    /// Summary description for BigFile
    /// </summary>
    [TestClass]
    public class BigFile
    {
        string _path = @"C:\Code\clang\llvm\tools\clang\tools\libclang\CIndex.cpp";
        Index index;
        TranslationUnit testcpp;


        [TestInitialize()]
        public void InitializeTest()
        {
            index = new Index(true, false);
            testcpp = new TranslationUnit(index, _path);
            Assert.AreNotEqual(index.Handle, IntPtr.Zero);
            Assert.AreNotEqual(testcpp.Handle, IntPtr.Zero);
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
            Cursor c = testcpp.Cursor;
            c.VisitChildren((cursor, parent) =>
            {
               // System.Diagnostics.Debug.WriteLine(cursor.ToString());
                if(cursor.Location.File == testcpp.File)
                    return Cursor.ChildVisitResult.Recurse;
                return Cursor.ChildVisitResult.Continue;
            }
            );
        }
    }
}
