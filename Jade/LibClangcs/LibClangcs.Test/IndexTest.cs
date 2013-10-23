using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace LibClang.Test
{
    [TestClass]
    public class IndexTest
    {
        [TestMethod]
        public void CreateDestroy()
        {
            Index i = new Index(true, false);
            Assert.AreNotEqual(i.Handle, IntPtr.Zero);
            i.Dispose();
        }

        [TestMethod]
        public void CreateTranslationUnit()
        {
            Index i = new Index(true, false);
            TranslationUnit tu = i.CreateTranslationUnit("TestFiles\\test.cc");
            Assert.AreNotEqual(tu.Handle, IntPtr.Zero);
            i.Dispose();            
        }

        [TestMethod]
        public void CreateTranslationUnitFilesBadFilename()
        {
            Index i = new Index(true, true);
            try
            {
                TranslationUnit tu = i.CreateTranslationUnit("badfilename.cc");
                Assert.Fail();
            }
            catch (System.IO.FileNotFoundException e)
            {
                //expected exception
                Assert.AreEqual(e.FileName, "badfilename.cc");
            }
            finally
            {
                i.Dispose();
            }
        }
    }
}
