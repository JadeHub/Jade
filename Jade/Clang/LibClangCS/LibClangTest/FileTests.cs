using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LibClangTest
{
    using LibClang;

    [TestClass]
    public class FileTests
    {
        [TestMethod]
        public void EqualityTests()
        {
            TranslationUnit tu = TestCode.TranslationUnits.SimpleClassCpp;
            File cpp1 = tu.GetFile("TestCode\\simple_class.cpp");
            File cpp2 = tu.GetFile("TestCode\\simple_class.cpp");
            File h = tu.GetFile("TestCode\\simple_class.h");

            EqualityTester<File>.Test(cpp1, cpp2, h);
        }
    }
}
