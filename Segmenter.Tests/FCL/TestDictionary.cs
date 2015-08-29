using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace Segmenter.Tests.FCL
{
    [TestFixture]
    public class TestDictionary
    {
        [TestCase]
        public void TestDeclaration()
        {
            var d = new Dictionary<char, double>
            {
                {'B', -0.26268660809250016},
                {'E', -3.14e+100},
                {'M', -3.14e+100},
                {'S', -1.4652633398537678}
            };

            Console.WriteLine(d.Count);
            Console.WriteLine(d['B']);
            Console.WriteLine(d['E']);
            Console.WriteLine(double.MinValue);
        }
    }
}