using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;

namespace JiebaNet.Segmenter.Tests
{
    [SetUpFixture]
    public class SetUpClass
    {
        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Directory.SetCurrentDirectory(dir);
        }

        [OneTimeTearDown]
        public void RunAfterAnyTests()
        {
            Console.WriteLine("Job Done");
        }
    }
}