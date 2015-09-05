using System;
using System.IO;
using JiebaNet.Analyser;
using NUnit.Framework;

namespace JiebaNet.Segmenter.Tests
{
    [TestFixture]
    public class TestTfidfExtractor
    {
        private string[] GetTestSentences()
        {
            return File.ReadAllLines(@"Cases\jieba_test.txt");
        }

        private string GetFileContents(string fileName)
        {
            return File.ReadAllText(fileName);
        }

        [TestCase]
        public void TestExtractTags()
        {
            var tfidf = new Tfidf();
            var text = GetFileContents(@"Resources\article.txt");
            var result = tfidf.ExtractTags(text, 10);
            foreach (var tag in result)
            {
                Console.WriteLine(tag);
            }
        }
    }
}