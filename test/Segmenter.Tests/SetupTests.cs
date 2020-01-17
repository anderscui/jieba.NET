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

            // ConfigManager.ConfigFileBaseDir = @"C:\jiebanet\config";
        }

        [OneTimeTearDown]
        public void RunAfterAnyTests()
        {
            Console.WriteLine("Job Done");
        }
    }
}