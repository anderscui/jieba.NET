using System;
using System.IO;
using NUnit.Framework;

namespace JiebaNet.Segmenter.Tests.FCL
{
    [TestFixture]
    public class TestIO
    {
        [TestCase]
        public void TestNormalizePath()
        {
            var p = @"..\test.txt";
            Assert.That(Path.IsPathRooted(p));
            Console.WriteLine(Path.GetFullPath(p));

            p = @"C:\test.txt";
            Assert.That(Path.IsPathRooted(p), Is.False);
            Console.WriteLine(Path.GetFullPath(p));
        }
    }
}