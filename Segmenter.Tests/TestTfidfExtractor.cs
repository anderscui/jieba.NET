using System;
using System.IO;
using System.Runtime.InteropServices;
using JiebaNet.Analyser;
using JiebaNet.Segmenter.PosSeg;
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
            var tfidf = new TfidfExtractor();
            var text = GetFileContents(@"Resources\article.txt");
            var result = tfidf.ExtractTags(text);
            foreach (var tag in result)
            {
                Console.WriteLine(tag);
            }
        }

        [TestCase]
        public void TestExtractTagsWithWeights()
        {
            var tfidf = new TfidfExtractor();
            var text = GetFileContents(@"Resources\article.txt");
            var result = tfidf.ExtractTagsWithWeight(text);
            foreach (var tag in result)
            {
                Console.WriteLine("({0}, {1})", tag.Word, tag.Weight);
            }
        }

        [TestCase]
        public void TestExtractSportsTags()
        {
            var tfidf = new TfidfExtractor();
            var text = GetFileContents(@"Resources\article_sports.txt");
            var result = tfidf.ExtractTags(text);
            foreach (var tag in result)
            {
                Console.WriteLine(tag);
            }
        }

        [TestCase]
        public void TestSetStopWords()
        {
            var tfidf = new TfidfExtractor();
            tfidf.SetStopWords(@"Resources\stop_words_test.txt");
            var text = GetFileContents(@"Resources\article_sports.txt");
            var result = tfidf.ExtractTags(text, 30);
            foreach (var tag in result)
            {
                Console.WriteLine(tag);
            }
        }

        [TestCase]
        public void TestExtractSportsTagsSocialNews()
        {
            var tfidf = new TfidfExtractor();
            var text = GetFileContents(@"Resources\article_social.txt");
            var result = tfidf.ExtractTags(text, 30);
            foreach (var tag in result)
            {
                Console.WriteLine(tag);
            }
        }

        [TestCase]
        public void TestExtractTagsWithPos()
        {
            var tfidf = new TfidfExtractor();
            var text = GetFileContents(@"Resources\article_social.txt");
            var result = tfidf.ExtractTags(text, 30, Constants.NounAndVerbPos);
            foreach (var tag in result)
            {
                Console.WriteLine(tag);
            }
        }

        [TestCase]
        public void TestExtractIdioms()
        {
            var tfidf = new TfidfExtractor();
            var text = GetFileContents(@"Resources\article_social.txt");
            var result = tfidf.ExtractTags(text, 50, Constants.IdiomPos);
            foreach (var tag in result)
            {
                Console.WriteLine(tag);
            }
        }

        [TestCase]
        public void TestTextRankExtractorWithWeight()
        {
            var s =  "此外，公司拟对全资子公司吉林欧亚置业有限公司增资4.3亿元，增资后，吉林欧亚置业注册资本由7000万元增加到5亿元。吉林欧亚置业主要经营范围为房地产开发及百货零售等业务。目前在建吉林欧亚城市商业综合体项目 2013年，实现营业收入0万元，实现净利润-139.13万元。";
            var extractor = new TextRankExtractor();
            var result = extractor.ExtractTagsWithWeight(s);
            foreach (var tag in result)
            {
                Console.WriteLine("({0}, {1})", tag.Word, tag.Weight);
            }
        }

        [TestCase]
        public void TestTextRankExtractor()
        {
            var s = "此外，公司拟对全资子公司吉林欧亚置业有限公司增资4.3亿元，增资后，吉林欧亚置业注册资本由7000万元增加到5亿元。吉林欧亚置业主要经营范围为房地产开发及百货零售等业务。目前在建吉林欧亚城市商业综合体项目 2013年，实现营业收入0万元，实现净利润-139.13万元。";
            var extractor = new TextRankExtractor();
            var result = extractor.ExtractTags(s);
            foreach (var tag in result)
            {
                Console.WriteLine(tag);
            }
        }
    }
}