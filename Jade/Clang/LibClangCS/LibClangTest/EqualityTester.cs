using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LibClangTest
{
    internal class EqualityTester<TestType> where TestType : class
    {
        static public void Test(TestType obj, TestType sameAsObj, TestType different)
        {
            Assert.IsNotNull(obj);
            Assert.IsNotNull(sameAsObj);
            Assert.IsNotNull(different);
            
            Assert.AreEqual(obj, sameAsObj);
            Assert.IsTrue(obj == sameAsObj);
            Assert.IsFalse(obj != sameAsObj);

            Assert.AreNotEqual(obj, different);
            Assert.IsFalse(obj == different);
            Assert.IsTrue(obj != different);

            //comparing to null
            Assert.AreNotEqual(obj, null);
            Assert.AreNotEqual(null, obj);
            Assert.IsFalse(obj == null);
            Assert.IsFalse(null == obj);
            Assert.IsTrue(obj != null);
            Assert.IsTrue(null != obj);
        }
    }
}
