using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace JiebaNet.Segmenter.Tests.FCL
{
    [TestFixture]
    public class TestApp
    {
        [TestCase]
        public void TestAppBaseDir()
        {
            Console.WriteLine(AppDomain.CurrentDomain.BaseDirectory);
        }
    }
}