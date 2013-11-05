using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace LibClang.Test
{
    /// <summary>
    /// Summary description for CursorTest
    /// </summary>
    [TestClass]
    public class CursorTest
    {
        Index index;
        TranslationUnit testcpp;

        public CursorTest()
        {
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize() 
        {
            index = new Index(true, false);
            testcpp = new TranslationUnit(index, "TestFiles\\test.cc");
            Assert.AreNotEqual(index.Handle, IntPtr.Zero);
            Assert.AreNotEqual(testcpp.Handle, IntPtr.Zero);            
        }
        //
        // Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup() 
        {
            index.Dispose();
            index = null;
        }
        
        #endregion

        [TestMethod]
        public void RecurseAll()
        {
            Cursor c = testcpp.Cursor;
            c.VisitChildren((cursor, parent) => 
            {
                System.Diagnostics.Debug.WriteLine(cursor.ToString());
                return Cursor.ChildVisitResult.Recurse;
            }
            );
        }

        private IList<Cursor> GetChildren(Cursor c)
        {
            List<Cursor> lc = new List<Cursor>();

            c.VisitChildren((cursor, parent) =>
            {
                lc.Add(cursor);
                return Cursor.ChildVisitResult.Continue;
            });
            return lc;
        }
    }
}
