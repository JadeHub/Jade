using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace LibClangTest
{
    using LibClang;

    [TestClass]
    public class TranslationUnitTests
    {
        public TranslationUnitTests()
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
        // [TestInitialize()]
        public void MyTestInitialize() 
        {

        }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void ParseNonExistantFileThrows()
        {
            try
            {
                using (TranslationUnit tu = new TranslationUnit(TestCode.TranslationUnits.Index, "BlahBlah"))
                {
                    tu.Parse(null);
                }                
            }
            catch (System.IO.FileNotFoundException)
            {
                return;
            }
            Assert.Fail();
        }

        [TestMethod]
        public void ParseAndBecomeValid()
        {
            using (TranslationUnit tu = new TranslationUnit(TestCode.TranslationUnits.Index, TestCode.SimpleClassCppFile.Path))
            {
                Assert.IsFalse(tu.Valid);
                tu.Parse(null);
                Assert.IsTrue(tu.Valid);
            }
        }

        [TestMethod]
        public void ReparseAndStayValid()
        {
            using (TranslationUnit tu = TestCode.TranslationUnits.SimpleClassCpp)
            {
                Assert.IsTrue(tu.Valid);
                tu.Parse(null);
                Assert.IsTrue(tu.Valid);
            }
        }

        [TestMethod]
        public void GetCursorAtOffset()
        {
            using (TranslationUnit tu = TestCode.TranslationUnits.SimpleClassCpp)
            {
                //Valid location
                Cursor cur = tu.GetCursorAt(TestCode.SimpleClassCppFile.Path, TestCode.SimpleClassCppFile.NamespaceDefinition);
                Assert.IsNotNull(cur);
                Assert.AreEqual(cur.Kind, CursorKind.Namespace);

                //Invalid offset
                Assert.IsNull(tu.GetCursorAt(TestCode.SimpleClassCppFile.Path, 10000));
            }
        }
        
        [TestMethod]
        public void GetLocation()
        {
            using (TranslationUnit tu = TestCode.TranslationUnits.SimpleClassCpp)
            {
                //Valid location
                SourceLocation sl = tu.GetLocation(TestCode.SimpleClassCppFile.Path, TestCode.SimpleClassCppFile.NamespaceDefinition);
                Assert.IsNotNull(sl);
                Assert.AreEqual(sl.File.Name.ToLower(), TestCode.SimpleClassCppFile.Path.ToLower());
                Assert.AreEqual(sl.Offset, TestCode.SimpleClassCppFile.NamespaceDefinition);

                //Invalid Location
                Assert.IsNull(tu.GetLocation(TestCode.SimpleClassCppFile.Path, 10000));

                //Invalid file
                Assert.IsNull(tu.GetLocation("blah", 0));
            }
        }

        [TestMethod]
        public void GetCursorAtLocation()
        {
            using (TranslationUnit tu = TestCode.TranslationUnits.SimpleClassCpp)
            {
                //Valid Location
                SourceLocation sl = tu.GetLocation(TestCode.SimpleClassCppFile.Path, TestCode.SimpleClassCppFile.NamespaceDefinition);
                Assert.IsNotNull(sl);
                Cursor cur = tu.GetCursorAt(sl);
                Assert.IsNotNull(cur);
                Assert.AreEqual(cur.Kind, CursorKind.Namespace);
            }
        }

        [TestMethod]
        public void GetSourceLocation()
        {
            using (TranslationUnit tu = TestCode.TranslationUnits.SimpleClassCpp)
            {
                //Valid Location
                SourceLocation nsStart = tu.GetLocation(TestCode.SimpleClassCppFile.Path, TestCode.SimpleClassCppFile.NamespaceStart);
                SourceLocation nsEnd = tu.GetLocation(TestCode.SimpleClassCppFile.Path, TestCode.SimpleClassCppFile.NamespaceStart);
                SourceRange sr = tu.GetSourceRange(nsStart, nsEnd);
                Assert.IsNotNull(sr);
                
                //Invald?
            }
        }

        [TestMethod]
        public void GetFile()
        {
            using (TranslationUnit tu = TestCode.TranslationUnits.SimpleClassCpp)
            {
                File f = tu.GetFile(TestCode.SimpleClassCppFile.Path);
                Assert.IsNotNull(f);
                Assert.AreEqual(f.Name.ToLower(), TestCode.SimpleClassCppFile.Path.ToLower());

                //Invalid path
                Assert.IsNull(tu.GetFile("blah"));
            }
        }

        private class FindReferencesAccumulator
        {
            private IList<Cursor> _curs;

            public FindReferencesAccumulator(IList<Cursor> curs)
            {
                _curs = curs;
            }

            public bool Accumulate(Cursor c, SourceRange r)
            {
                _curs.Add(c);
                return true;
            }
        }

        [TestMethod]
        public void FindReferences()
        {
            using (TranslationUnit tu = TestCode.TranslationUnits.SimpleClassCpp)
            {
                File cpp = tu.GetFile(TestCode.SimpleClassCppFile.Path);
                File header = tu.GetFile(TestCode.SimpleClassHeaderFile.Path);
                Cursor cur = tu.GetCursorAt(TestCode.SimpleClassCppFile.Path, TestCode.SimpleClassCppFile.NamespaceDefinition);
                List<Cursor> cs = new List<Cursor>();
                FindReferencesAccumulator accumulator = new FindReferencesAccumulator(cs);
                Assert.IsTrue(tu.FindReferencesTo(cur, cpp, accumulator.Accumulate));
                Assert.AreEqual(cs.Count, 1);
            }
        }

        [TestMethod]
        public void Headers()
        {
            using (TranslationUnit tu = TestCode.TranslationUnits.HeaderTestsCpp)
            {
               // Diagnostic d = tu.Diagnostics.Diagnostics.First();
                List<TranslationUnit.HeaderInfo> headers = new List<TranslationUnit.HeaderInfo>();
                headers.AddRange(tu.HeaderFiles);
            }
        }

        [TestMethod]
        public void Test123()
        {
            using (TranslationUnit tu = TestCode.TranslationUnits.SimpleClassCpp)
            {
                Cursor c = tu.GetCursorAt(TestCode.SimpleClassCppFile.Path, 327);
            }
        }
    }
}
