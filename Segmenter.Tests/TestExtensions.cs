using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace JiebaNet.Segmenter.Tests
{
    [TestFixture]
    public class TestExtensions
    {
        [TestCase]
        public void TestJavaStyleSubstring()
        {
            var s = "0123456789";
            Assert.That(s.Sub(3, 6), Is.EqualTo("345"));
        }
    }
}