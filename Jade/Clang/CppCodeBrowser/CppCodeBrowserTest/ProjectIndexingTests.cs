using CppCodeBrowser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace CppCodeBrowserTest
{
    [TestClass]
    public class ProjectIndexingTests
    {
        [TestMethod]
        public void IndexProject()
        {
            Project proj = TestProject.MakeProject();
            ICodeBrowser b = new JumpToBrowser(proj.Index);

            List<ICodeLocation> results = new List<ICodeLocation>();
            results.AddRange(b.BrowseFrom(new CodeLocation("TestCode\\class_a.cpp", 53)));

            /*ICodeLocation loc = b.JumpTo(new CodeLocation("TestCode\\class_a.h", 67));
            Assert.IsNotNull(loc);*/
        }
    }
}
