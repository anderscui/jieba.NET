using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JiebaNet.Segmenter.Common;
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
            Assert.That(s.Sub(0, 3), Is.EqualTo("012"));
            Assert.That(s.Sub(3, 6), Is.EqualTo("345"));
        }

        [TestCase]
        public void TestCharToInt32()
        {
            var c = 'A';
            Assert.That(65, Is.EqualTo(c.ToInt32()));
        }
    }
}