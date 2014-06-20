using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LibClangTest
{
    using LibClang;

    [TestClass]
    public class CursorTests
    {        
        [TestMethod]
        public void EqualityTests()
        {
            using (TranslationUnit tu = TestCode.TranslationUnits.SimpleClassCpp)
            {
                Cursor ns1 = tu.GetCursorAt(TestCode.SimpleClassCppFile.Path, TestCode.SimpleClassCppFile.NamespaceDefinition);
                Cursor ns2 = tu.GetCursorAt(TestCode.SimpleClassCppFile.Path, TestCode.SimpleClassCppFile.NamespaceDefinition);
                Cursor md = tu.GetCursorAt(TestCode.SimpleClassCppFile.Path, TestCode.SimpleClassCppFile.MethodDefinition);

                EqualityTester<Cursor>.Test(ns1, ns2, md);
            }
        }

        [TestMethod]
        public void StringInclude()
        {
            using (TranslationUnit tu = TestCode.TranslationUnits.StringIncludeCpp)
            {
                Cursor c = tu.GetCursorAt(TestCode.StringIncludeCppFile.Path, 0);
                Assert.AreEqual(c.Kind, CursorKind.InclusionDirective);
                c = tu.GetCursorAt(TestCode.StringIncludeCppFile.Path, 1);
                Assert.AreEqual(c.Kind, CursorKind.InclusionDirective);
                c = tu.GetCursorAt(TestCode.StringIncludeCppFile.Path, 2);
                Assert.AreEqual(c.Kind, CursorKind.InclusionDirective);
                c = tu.GetCursorAt(TestCode.StringIncludeCppFile.Path, 3);
                Assert.AreEqual(c.Kind, CursorKind.InclusionDirective);
                c = tu.GetCursorAt(TestCode.StringIncludeCppFile.Path, 4);
                Assert.AreEqual(c.Kind, CursorKind.InclusionDirective);
                c = tu.GetCursorAt(TestCode.StringIncludeCppFile.Path, 5);
                Assert.AreEqual(c.Kind, CursorKind.InclusionDirective);
                c = tu.GetCursorAt(TestCode.StringIncludeCppFile.Path, 6);
                Assert.AreEqual(c.Kind, CursorKind.InclusionDirective);
                c = tu.GetCursorAt(TestCode.StringIncludeCppFile.Path, 7);
                Assert.AreEqual(c.Kind, CursorKind.InclusionDirective);
                c = tu.GetCursorAt(TestCode.StringIncludeCppFile.Path, 8);
                Assert.AreEqual(c.Kind, CursorKind.InclusionDirective);
                c = tu.GetCursorAt(TestCode.StringIncludeCppFile.Path, 9);
                Assert.AreEqual(c.Kind, CursorKind.InclusionDirective);
           /*     c = tu.GetCursorAt(TestCode.StringIncludeCppFile.Path, 10);
                Assert.AreEqual(c.Kind, CursorKind.InclusionDirective);
            */ 
            }
        }

        [TestMethod]
        public void PropertyTests()
        {
            using (TranslationUnit tu = TestCode.TranslationUnits.SimpleClassCpp)
            {
                Cursor method = tu.GetCursorAt(TestCode.SimpleClassCppFile.Path, TestCode.SimpleClassCppFile.MethodDefinition);
                Cursor ns = tu.GetCursorAt(TestCode.SimpleClassCppFile.Path, TestCode.SimpleClassCppFile.NamespaceDefinition);
                Cursor classCursor = tu.GetCursorAt(TestCode.SimpleClassHeaderFile.Path, TestCode.SimpleClassHeaderFile.ClassDefinition);
                Assert.IsNotNull(ns);
                Assert.IsNotNull(method);
                Assert.IsNotNull(classCursor);

                //Extent of namespace
                SourceRange extent = ns.Extent;
                Assert.IsNotNull(extent);
                Assert.AreEqual(extent.Start.Offset, (int)TestCode.SimpleClassCppFile.NamespaceStart);
                Assert.AreEqual(extent.End.Offset, TestCode.SimpleClassCppFile.NamespaceEnd);

                //Kind
                Assert.AreEqual(ns.Kind, CursorKind.Namespace);
                Assert.AreEqual(method.Kind, CursorKind.CXXMethod);

                //Spelling
                Assert.AreEqual(ns.Spelling, "TestCode");
                Assert.AreEqual(method.Spelling, "Method");

                //Location            
                Assert.IsNotNull(ns.Location);
                Assert.AreEqual(ns.Location.Offset, TestCode.SimpleClassCppFile.NamespaceDefinition);
                Assert.IsNotNull(method.Location);
                Assert.AreEqual(method.Location.Offset, TestCode.SimpleClassCppFile.MethodDefinition);

                //Type
                Assert.IsNull(ns.Type);
                Assert.IsNotNull(method.Type);
                Assert.AreEqual(method.Type.Kind, TypeKind.FunctionProto);

                //Usr
                Assert.IsFalse(string.IsNullOrEmpty(ns.Usr));
                Assert.IsFalse(string.IsNullOrEmpty(method.Usr));

                //ConicalCursor
                Assert.IsNotNull(ns.CanonicalCursor);
                Assert.AreEqual(ns, ns.CanonicalCursor);
                Assert.IsNotNull(method.CanonicalCursor);
                Assert.AreEqual(method, method.CanonicalCursor);

                //IsDefinition
                Assert.IsTrue(ns.IsDefinition);
                Assert.IsTrue(method.IsDefinition);

                //Definition
                Assert.IsNull(ns.DefinitionCursor);
                Assert.IsNull(method.DefinitionCursor);

                //IsReference
                Assert.IsFalse(ns.IsReference);
                Assert.IsFalse(method.IsReference);

                //CursorReferenced
                Assert.IsNull(ns.CursorReferenced);
                Assert.IsNull(method.CursorReferenced);

                //LexicalParentCurosr
                Assert.IsNotNull(ns.LexicalParentCurosr);
                Assert.AreEqual(ns.LexicalParentCurosr, tu.Cursor);
                Assert.IsNotNull(method.LexicalParentCurosr);
                Assert.AreEqual(method.LexicalParentCurosr, ns);

                //SemanticParentCurosr
                Assert.IsNotNull(ns.SemanticParentCurosr);
                Assert.AreEqual(ns.SemanticParentCurosr, tu.Cursor);
                Assert.IsNotNull(method.SemanticParentCurosr);
                Assert.AreEqual(method.SemanticParentCurosr, classCursor);

                //Valid
                Assert.IsTrue(ns.Valid);
                Assert.IsTrue(method.Valid);
            }
        }
    }
}
