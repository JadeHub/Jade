using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JadeUtils.IO;

namespace JadeCode.Test
{
    [TestClass]
    public class PathTests
    {
        [TestMethod]
        public void CalculateRelativePathTest()
        {
            Assert.AreEqual(PathUtils.CalculateRelativePath(@"C:\test\path1\path2\",    @"C:\Test\"),            @"..\..\");
            Assert.AreEqual(PathUtils.CalculateRelativePath(@"C:\tesT\path1\file",      @"C:\test\"),            @"..\");
            Assert.AreEqual(PathUtils.CalculateRelativePath(@"C:\test\path1\path2\",    @"C:\"),                 @"..\..\..\");
            Assert.AreEqual(PathUtils.CalculateRelativePath(@"C:\test\path1\path2\",    @"D:\"),                 @"D:\");
            Assert.AreEqual(PathUtils.CalculateRelativePath(@"C:\test\path1\path2\",    @"C:\Test\path1\pathA"), @"..\pathA");
            Assert.AreEqual(PathUtils.CalculateRelativePath(@"C:\test\",                @"C:\tEst\"),            @"");
            Assert.AreEqual(PathUtils.CalculateRelativePath(@"C:\Test\",                @"C:\teSt\file"),        @"file");
            Assert.AreEqual(PathUtils.CalculateRelativePath(@"C:\test\file",            @"C:\tesT\"),            @".\");
            Assert.AreEqual(PathUtils.CalculateRelativePath(@"C:\Test\path #1!\path2\", @"C:\test\"),            @"..\..\");
        }
    }
}
