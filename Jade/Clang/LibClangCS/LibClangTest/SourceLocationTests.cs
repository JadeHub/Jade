using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LibClangTest
{
    using LibClang;

    [TestClass]
    public class SourceLocationTests
    {
        [TestMethod]
        public void EqualityTests()
        { 
            TranslationUnit tu = TestCode.TranslationUnits.SimpleClassCpp;
            SourceLocation sl = tu.GetLocation(TestCode.SimpleClassCppFile.Path, 0);
            SourceLocation sl2 = tu.GetLocation(TestCode.SimpleClassCppFile.Path, 0);
            SourceLocation different = tu.GetLocation(TestCode.SimpleClassCppFile.Path, TestCode.SimpleClassCppFile.NamespaceStart);

            EqualityTester<SourceLocation>.Test(sl, sl2, different);
        }

        [TestMethod]
        public void GreaterLesserThanTests()
        {
            TranslationUnit tu = TestCode.TranslationUnits.SimpleClassCpp;
            SourceLocation low = tu.GetLocation(TestCode.SimpleClassCppFile.Path, 0);
            SourceLocation low2 = tu.GetLocation(TestCode.SimpleClassCppFile.Path, 0);
            SourceLocation high = tu.GetLocation(TestCode.SimpleClassCppFile.Path, TestCode.SimpleClassCppFile.NamespaceDefinition);
            SourceLocation high2 = tu.GetLocation(TestCode.SimpleClassCppFile.Path, TestCode.SimpleClassCppFile.NamespaceDefinition);

            //Greater
            Assert.IsTrue(high > low);
            Assert.IsFalse(low > high);
            Assert.IsFalse(low > low2);
            Assert.IsFalse(high > high2);
            Assert.IsTrue(high >= low);
            Assert.IsFalse(low >= high);
            Assert.IsTrue(low >= low2);
            Assert.IsTrue(high >= high2);            

            //Lesser
            Assert.IsTrue(low < high);
            Assert.IsFalse(high < low);
            Assert.IsFalse(low < low2);
            Assert.IsFalse(high < high2);
            Assert.IsTrue(low <= high);
            Assert.IsFalse(high <= low);
            Assert.IsTrue(low <= low2);
            Assert.IsTrue(high <= high2);

            //Check attempting to compare different files throws
            bool cought = false;
            SourceLocation header = tu.GetLocation(TestCode.SimpleClassHeaderFile.Path, 0);
            try
            {
                bool b = low < header;
            }
            catch (ArgumentException)
            {
                cought = true;
            }
            Assert.IsTrue(cought);

            cought = false;
            try
            {
                bool b = low <= header;
            }
            catch (ArgumentException)
            {
                cought = true;
            }
            Assert.IsTrue(cought);

            cought = false;
            try
            {
                bool b = low > header;
            }
            catch (ArgumentException)
            {
                cought = true;
            }
            Assert.IsTrue(cought);

            cought = false;
            try
            {
                bool b = low >= header;
            }
            catch (ArgumentException)
            {
                cought = true;
            }
            Assert.IsTrue(cought);
        }        

        [TestMethod]
        public void CompareToTests()
        {
            TranslationUnit tu = TestCode.TranslationUnits.SimpleClassCpp;
            SourceLocation low = tu.GetLocation(TestCode.SimpleClassCppFile.Path, 0);
            SourceLocation low2 = tu.GetLocation(TestCode.SimpleClassCppFile.Path, 0);
            SourceLocation high = tu.GetLocation(TestCode.SimpleClassCppFile.Path, TestCode.SimpleClassCppFile.NamespaceDefinition);
            SourceLocation high2 = tu.GetLocation(TestCode.SimpleClassCppFile.Path, TestCode.SimpleClassCppFile.NamespaceDefinition);

            Assert.IsTrue(low.CompareTo(high) < 0);
            Assert.IsTrue(low.CompareTo(low2) == 0);

            Assert.IsTrue(high.CompareTo(low) > 0);
            Assert.IsTrue(high.CompareTo(high2) == 0);
            
            SourceLocation header = tu.GetLocation(TestCode.SimpleClassHeaderFile.Path, 0);
            try
            {
                header.CompareTo(low);
            }
            catch (ArgumentException)
            {
                return;
            }
            Assert.IsTrue(false);            
        }

        [TestMethod]
        public void PropertyTests()
        {
            TranslationUnit tu = TestCode.TranslationUnits.SimpleClassCpp;
            SourceLocation sl = tu.GetLocation(TestCode.SimpleClassCppFile.Path, TestCode.SimpleClassCppFile.NamespaceDefinition);
            
            Assert.IsNotNull(sl);

            Assert.IsTrue(sl.Column > 0);
            Assert.IsTrue(sl.Line > 0);
            Assert.AreEqual(sl.Offset, TestCode.SimpleClassCppFile.NamespaceDefinition);
            Assert.IsNotNull(sl.File);
            Assert.AreEqual(sl.File.Name.ToLower(), TestCode.SimpleClassCppFile.Path.ToLower());
        }
    }
}
