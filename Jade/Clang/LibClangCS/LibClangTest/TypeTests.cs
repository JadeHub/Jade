using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LibClangTest
{
    using LibClang;

    [TestClass]
    public class TypeTests
    {
        [TestMethod]
        public void EqualityTests()
        {
            TranslationUnit tu = TestCode.TranslationUnits.SimpleClassCpp;
            Type method = tu.GetCursorAt(TestCode.SimpleClassCppFile.Path, TestCode.SimpleClassCppFile.MethodDefinition).Type;
            Type method2 = tu.GetCursorAt(TestCode.SimpleClassCppFile.Path, TestCode.SimpleClassCppFile.MethodDefinition).Type;
            Type ctor = tu.GetCursorAt(TestCode.SimpleClassCppFile.Path, TestCode.SimpleClassCppFile.ConstructorDefinition).Type;

            EqualityTester<Type>.Test(method, method2, ctor);
        }
    }
}
