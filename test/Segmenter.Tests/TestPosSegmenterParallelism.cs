using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using NUnit.Framework;

using JiebaNet.Segmenter.PosSeg;

namespace JiebaNet.Segmenter.Tests
{
    [TestFixture]
    public class TestPosSegmenterParallelism
    {
        private string[] GetTestSentences()
        {
            return File.ReadAllLines(TestHelper.GetCaseFilePath("jieba_test.txt"));
        }

        [TestCase]
        public void TestCutInParallel()
        {
            var seg = new JiebaSegmenter();
            var posSeg = new PosSegmenter(seg);
            TestParallelCutFunction(posSeg.CutInParallel, true, TestHelper.GetCaseFilePath("pos_cut_hmm.txt"));
        }
        
        [TestCase]
        public void TestCutInParallelWithouHmm()
        {
            var seg = new JiebaSegmenter();
            var posSeg = new PosSegmenter(seg);
            TestParallelCutFunction(posSeg.CutInParallel, false, TestHelper.GetCaseFilePath("pos_cut_no_hmm.txt"));
        }

        #region Private Helpers

        private void TestParallelCutFunction(Func<IEnumerable<string>, bool, IEnumerable<IEnumerable<Pair>>> method,
            bool useHmm,
            string testResultFile)
        {
            var testCases = GetTestSentences();
            var testResults = method(testCases, useHmm).ToList();
            var expectedResults = File.ReadAllLines(testResultFile);
            Assert.That(testResults.Count, Is.EqualTo(expectedResults.Length));
            
            foreach (var item in testResults.Zip(expectedResults, Tuple.Create))
            {
                var actualResult = string.Join(" ", item.Item1.Select(token => $"{token.Word}/{token.Flag}"));
                var expectedResult = item.Item2;
                Assert.That(actualResult, Is.EqualTo(expectedResult));
            }
        }

        #endregion
    }
}