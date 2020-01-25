using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JiebaNet.Segmenter.Common;
using NUnit.Framework;

namespace JiebaNet.Segmenter.Tests
{
    [TestFixture]
    public class TestSegmenterParallelism
    {
        private string[] GetTestSentences()
        {
            return File.ReadAllLines(TestHelper.GetCaseFilePath("jieba_test.txt"));
        }

        private JiebaSegmenter GetParalellSegmenter()
        {
            return new JiebaSegmenter();
        }

        [TestCase]
        public void TestCutLargeText()
        {
            var seg = GetParalellSegmenter();
            
            var lines = Enumerable.Repeat("生成句子中汉字所有可能成词情况所构成的有向无环图", 1).ToList();
            var n = seg.Cut(lines[0]).Count();

            var largeText = string.Join(Environment.NewLine, lines);
            // Console.WriteLine(largeText);
            var result = seg.CutInParallel(largeText);
            Assert.That(result.Count(), Is.EqualTo(n * lines.Count));
        }
        
        [TestCase]
        public void TestCutList()
        {
            var seg = GetParalellSegmenter();
            
            var lines = Enumerable.Repeat("生成句子中汉字所有可能成词情况所构成的有向无环图", 1000).ToList();
            var n = seg.Cut(lines[0]).Count();
            
            var result = seg.CutInParallel(lines).SelectMany(x => x);
            Assert.That(result.Count(), Is.EqualTo(n * lines.Count));
        }
        
        [TestCase]
        public void TestCut()
        {
            TestCutInParallelFunction(GetParalellSegmenter().CutInParallel, false, true, TestHelper.GetCaseFilePath("accurate_hmm.txt"));
        }

        [TestCase]
        public void TestCutAll()
        {
            TestCutInParallelFunction(GetParalellSegmenter().CutInParallel, true, false, TestHelper.GetCaseFilePath("cut_all.txt"));
        }

        [TestCase]
        public void TestCutWithoutHmm()
        {
            TestCutInParallelFunction(GetParalellSegmenter().CutInParallel, false, false, TestHelper.GetCaseFilePath("accurate_no_hmm.txt"));
        }

        [TestCase]
        public void TestCutForSearch()
        {
            TestCutSearchFunction(GetParalellSegmenter().CutForSearchInParallel, true, TestHelper.GetCaseFilePath("cut_search_hmm.txt"));
        }

        [TestCase]
        public void TestCutForSearchWithoutHmm()
        {
            TestCutSearchFunction(GetParalellSegmenter().CutForSearchInParallel, false, TestHelper.GetCaseFilePath("cut_search_no_hmm.txt"));
        }

        [TestCase]
        public void TestTokenize()
        {
            var seg = GetParalellSegmenter();
            seg.AddWord("机器学习");
            seg.AddWord("自然语言处理");
            foreach (var token in seg.Tokenize("小明最近在学习机器学习、自然语言处理、云计算和大数据"))
            {
                Console.WriteLine(token);
            }

            foreach (var token in seg.Tokenize("小明最近在学习机器学习、自然语言处理、云计算和大数据", TokenizerMode.Search))
            {
                Console.WriteLine(token);
            }
        }

        [TestCase]
        public void TestAddWord()
        {
            var seg = GetParalellSegmenter();
            var s = "小明最近在学习机器学习和自然语言处理";

            // TODO: affected by other test cases
            seg.DeleteWord("机器学习");

            var segments = seg.Cut(s);
            Assert.That(segments, Contains.Item("机器"));
            Assert.That(segments, Contains.Item("学习"));

            seg.AddWord("机器学习");
            segments = seg.Cut(s);
            Assert.That(segments, Contains.Item("机器学习"));
            Assert.That(segments, Is.Not.Contains("机器"));
        }

        #region Private Helpers

        private void TestCutInParallelFunction(Func<IEnumerable<string>, bool, bool, IEnumerable<IEnumerable<string>>> method,
                                     bool cutAll, bool useHmm,
                                     string testResultFile)
        {
            var testCases = GetTestSentences();
            var testResults = method(testCases, cutAll, useHmm).ToList();
            var expectedResults = File.ReadAllLines(testResultFile);
            Assert.That(testResults.Count, Is.EqualTo(expectedResults.Length));
            
            foreach (var item in testResults.Zip(expectedResults, Tuple.Create))
            {
                var actualResult = item.Item1.ToList();
                var expectedResult = item.Item2;
                Assert.That(actualResult.Join("/ "), Is.EqualTo(expectedResult));
            }
        }

        private void TestCutSearchFunction(Func<IEnumerable<string>, bool, IEnumerable<IEnumerable<string>>> method,
                                     bool useHmm,
                                     string testResultFile)
        {
            var testCases = GetTestSentences();
            var testResults = method(testCases, useHmm).ToList();
            var expectedResults = File.ReadAllLines(testResultFile);
            Assert.That(testResults.Count, Is.EqualTo(expectedResults.Length));
            
            foreach (var item in testResults.Zip(expectedResults, Tuple.Create))
            {
                var actualResult = item.Item1.ToList();
                var expectedResult = item.Item2;
                Assert.That(actualResult.Join("/ "), Is.EqualTo(expectedResult));
            }
        }

        #endregion
    }
}