using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibClang.Test
{
    [TestClass]
    public class TranslationUnitTest
    {
        Index index;
        TranslationUnit testcpp;
        
        [TestInitialize()]
        public void InitializeTest()
        {
            index = new Index(true, false);
            testcpp = new TranslationUnit(index, "TestFiles\\test.cc");
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
        public void Filename()
        {
            Assert.AreEqual(testcpp.Filename, "TestFiles\\test.cc");
        }

        [TestMethod]
        public void CreateCursors()
        {
            IList<Cursor> curs = GetChildren(testcpp.Cursor);
            Assert.IsTrue(curs.Count > 0);
            IEnumerable<Cursor> locals = curs.Where(cur => cur.Location.File == testcpp.File);
            Assert.IsTrue(locals.Count() == 2);
            SourceLocation sl = locals.First().Location;
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
