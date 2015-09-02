using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Segmenter.Tests.FCL
{
    [TestFixture]
    public class TestRegex
    {
        [TestCase]
        public void TestMatches()
        {
            var re = new Regex(@"\d+");
            var input = "12 34 567";
            var mat = re.Matches(input);
            foreach (Match m in mat)
            {
                Console.WriteLine("{0} {1}", m.Index, m.Length);
            }
        }

        [TestCase]
        public void TestNumbers()
        {
            var re = new Regex("[.0-9]+");
            Assert.That(re.IsMatch("..."));
            Assert.That(re.IsMatch("123"));
            Assert.That(re.IsMatch("1.23"));
            Assert.That(re.IsMatch("1.2.3"));
            Assert.That(re.IsMatch("a.1.2.3"));
        }
    }
}