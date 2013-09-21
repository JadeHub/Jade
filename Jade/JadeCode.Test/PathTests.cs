using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JadeCore.IO;

namespace JadeCode.Test
{
    [TestClass]
    public class PathTests
    {
        [TestMethod]
        public void CalculateRelativePathTest()
        {
            Assert.AreEqual(Path.CalculateRelativePath(@"C:\test\path1\path2\",    @"C:\Test\"),            @"..\..\");
            Assert.AreEqual(Path.CalculateRelativePath(@"C:\tesT\path1\file",      @"C:\test\"),            @"..\");
            Assert.AreEqual(Path.CalculateRelativePath(@"C:\test\path1\path2\",    @"C:\"),                 @"..\..\..\");
            Assert.AreEqual(Path.CalculateRelativePath(@"C:\test\path1\path2\",    @"D:\"),                 @"D:\");
            Assert.AreEqual(Path.CalculateRelativePath(@"C:\test\path1\path2\",    @"C:\Test\path1\pathA"), @"..\pathA");
            Assert.AreEqual(Path.CalculateRelativePath(@"C:\test\",                @"C:\tEst\"),            @"");
            Assert.AreEqual(Path.CalculateRelativePath(@"C:\Test\",                @"C:\teSt\file"),        @"file");
            Assert.AreEqual(Path.CalculateRelativePath(@"C:\test\file",            @"C:\tesT\"),            @".\");
            Assert.AreEqual(Path.CalculateRelativePath(@"C:\Test\path #1!\path2\", @"C:\test\"),            @"..\..\");
        }
    }
}
