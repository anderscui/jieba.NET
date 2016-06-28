using System;
using System.Linq;
using NUnit.Framework;

namespace JiebaNet.Segmenter.Tests.FCL
{
    [TestFixture]
    public class TestNumbers 
    {
        [TestCase]
        public void TestEqualityOfDoubles()
        {
            double d1 = -3.14e100;
            double d2 = -6.28e100;
            Console.WriteLine(Math.Abs(d1 - d2) < 1e-100);
            Console.WriteLine(d1 > d2);
            Console.WriteLine(d1 < d2);
        }

        [TestCase]
        public void TestMinDouble()
        {
            Console.WriteLine(double.MinValue);
            //Console.WriteLine(-3.14e500);
        }
    }
}