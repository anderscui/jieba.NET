using System;
using System.IO;
using NUnit.Framework;

namespace JiebaNet.Segmenter.Tests
{
    [TestFixture]
    public class TestDict
    {
        [TestCase]
        public void TestMainDictPath()
        {
            var mainDict = ConfigManager.MainDictFile;
            Assert.That(mainDict, Is.Not.Null);
            Assert.That(File.Exists(mainDict));
        }

        [TestCase]
        public void TestDictTrie()
        {
            var dict = WordDictionary.Instance;
            Console.WriteLine(dict.Trie.Count);
            Console.WriteLine(dict.Total);
        }
    }
}