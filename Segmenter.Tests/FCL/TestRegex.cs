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
            Regex re = new Regex("\\d+");
            var input = "12 34 567";
            var mat = re.Matches(input);
            foreach (Match m in mat)
            {
                Console.WriteLine("{0} {1}", m.Index, m.Length);
            }
        }
    }
}