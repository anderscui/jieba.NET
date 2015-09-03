using System;
using System.IO;
using NUnit.Framework;

namespace Segmenter.Tests.FCL
{
    [TestFixture]
    public class TestUnicode
    {
        [TestCase]
        public void TestNormalizePath()
        {
            var s = "\u4e00\u4e01\u4e03";
            Console.WriteLine(s);
            File.WriteAllText("out.txt", s);
        }
    }
}