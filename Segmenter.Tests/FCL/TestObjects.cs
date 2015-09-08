using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace JiebaNet.Segmenter.Tests.FCL
{
    [TestFixture]
    public class TestObjects
    {
        [TestCase]
        public void TestNullValue()
        {
            var defaultVal = "default";

            string s = null;
            var actual = s ?? defaultVal;
            Assert.That(actual, Is.EqualTo("default"));

            s = string.Empty;
            actual = s ?? defaultVal;
            Assert.That(actual, Is.EqualTo(string.Empty));

            s = "some";
            actual = s ?? defaultVal;
            Assert.That(actual, Is.EqualTo("some"));
        }
    }
}