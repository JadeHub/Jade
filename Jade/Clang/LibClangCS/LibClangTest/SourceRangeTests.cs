using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LibClangTest
{
    using LibClang;

    [TestClass]
    public class SourceRangeTests
    {
        [TestMethod]
        public void EqualityTests()
        {
            TranslationUnit tu = TestCode.TranslationUnits.SimpleClassCpp;
            SourceLocation ns = tu.GetLocation(TestCode.SimpleClassCppFile.Path, TestCode.SimpleClassCppFile.NamespaceDefinition);
            SourceLocation nsStart = tu.GetLocation(TestCode.SimpleClassCppFile.Path, TestCode.SimpleClassCppFile.NamespaceStart);
            SourceLocation nsEnd = tu.GetLocation(TestCode.SimpleClassCppFile.Path, TestCode.SimpleClassCppFile.NamespaceEnd);
            SourceRange sr = tu.GetSourceRange(nsStart, nsEnd);
            SourceRange sr2 = tu.GetSourceRange(nsStart, nsEnd);
            SourceRange different = tu.GetSourceRange(ns, nsEnd);

            EqualityTester<SourceRange>.Test(sr, sr2, different);
        }

        [TestMethod]
        public void PropertyTests()
        {
            TranslationUnit tu = TestCode.TranslationUnits.SimpleClassCpp;
            SourceLocation nsStart = tu.GetLocation(TestCode.SimpleClassCppFile.Path, TestCode.SimpleClassCppFile.NamespaceStart);
            SourceLocation nsEnd = tu.GetLocation(TestCode.SimpleClassCppFile.Path, TestCode.SimpleClassCppFile.NamespaceEnd);
            SourceRange sr = tu.GetSourceRange(nsStart, nsEnd);

            Assert.IsNotNull(sr);
            Assert.IsNotNull(sr.Start);
            Assert.IsNotNull(sr.End);
            Assert.AreEqual(sr.Start, nsStart);
            Assert.AreEqual(sr.End, nsEnd);
        }
    }
}
