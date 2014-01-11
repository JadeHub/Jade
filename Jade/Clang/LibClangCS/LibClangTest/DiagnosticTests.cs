using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace LibClangTest
{
    using LibClang;

    [TestClass]
    public class DiagnosticTests
    {
        [TestMethod]
        public void EqualityTests()
        {
            using (TranslationUnit tu = TestCode.TranslationUnits.ErrorWarningClassCpp)
            {
                Diagnostic error = tu.Diagnostics.Where(item => item.DiagnosticSeverity == Diagnostic.Severity.Error).First();
                Diagnostic error2 = tu.Diagnostics.Where(item => item.DiagnosticSeverity == Diagnostic.Severity.Error).First();
                Diagnostic warn = tu.Diagnostics.Where(item => item.DiagnosticSeverity == Diagnostic.Severity.Warning).First();

                Assert.IsNotNull(error);
                Assert.IsNotNull(error2);
                Assert.IsNotNull(warn);
                EqualityTester<Diagnostic>.Test(error, error2, warn);
            }
        }

        [TestMethod]
        public void PropertyTests()
        {
            using (TranslationUnit tu = TestCode.TranslationUnits.ErrorWarningClassCpp)
            {
                Diagnostic error = tu.Diagnostics.Where(item => item.DiagnosticSeverity == Diagnostic.Severity.Error).First();
                Diagnostic warn = tu.Diagnostics.Where(item => item.DiagnosticSeverity == Diagnostic.Severity.Warning).First();

                Assert.IsNotNull(error);
                Assert.IsNotNull(warn);

                Assert.AreEqual(error.DiagnosticSeverity, Diagnostic.Severity.Error);
                Assert.AreEqual(warn.DiagnosticSeverity, Diagnostic.Severity.Warning);

                Assert.IsNotNull(error.Location);
                Assert.IsNotNull(warn.Location);
            }
        }
    }
}
