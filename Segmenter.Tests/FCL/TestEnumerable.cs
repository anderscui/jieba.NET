using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace JiebaNet.Segmenter.Tests.FCL
{
    [TestFixture]
    public class TestEnumerable
    {
        [TestCase]
        public void TestForEachWithIndex()
        {
            var numbers = new [] {1, 2, 3, 4, 5};
            foreach (var number in numbers.Select((n, i) => new { Number = n, Index = i }))
            {
                Console.WriteLine("{0}: {1}", number.Index, number.Number);
            }
        }
    }
}