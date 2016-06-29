using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JiebaNet.Segmenter.PosSeg;
using NUnit.Framework;

namespace JiebaNet.Segmenter.Tests
{
    [TestFixture]
    public class TestPosSegmenter
    {
        private string[] GetTestSentences()
        {
            return File.ReadAllLines(@"Cases\jieba_test.txt");
        }

        [TestCase]
        public void TestCut()
        {
            var seg = new JiebaSegmenter();
            var posSeg = new PosSegmenter(seg);
            TestCutFunction(posSeg.Cut, true, @"Cases\pos_cut_hmm.txt");
        }

        [TestCase]
        public void TestCutWithouHmm()
        {
            var seg = new JiebaSegmenter();
            var posSeg = new PosSegmenter(seg);
            TestCutFunction(posSeg.Cut, false, @"Cases\pos_cut_no_hmm.txt");
        }

        [TestCase]
        public void TestCutNames()
        {
            var posSeg = new PosSegmenter();
            var tokens = posSeg.Cut("吉林的省会是长春");
            var result = string.Join(" ", tokens.Select(token => string.Format("{0}/{1}", token.Word, token.Flag)));
            Console.WriteLine(result);
        }

        [TestCase]
        [Category("Issue")]
        public void TestNewords()
        {
            var text = "元祐";
            TestPosSegmenterCut(text);

            text = "整併整併整併整併";
            TestPosSegmenterCut(text);
        }

        private static void TestPosSegmenterCut(string text)
        {
            var posSeg = new PosSegmenter();
            var tokens = posSeg.Cut(text);
            var result = string.Join(" ", tokens.Select(token => string.Format("{0}/{1}", token.Word, token.Flag)));
            Console.WriteLine(result);
        }

        #region Private Helpers

        private void TestCutFunction(Func<string, bool, IEnumerable<Pair>> method,
                                     bool useHmm,
                                     string testResultFile)
        {
            var testCases = GetTestSentences();
            var testResults = File.ReadAllLines(testResultFile);
            Assert.That(testCases.Length, Is.EqualTo(testResults.Length));
            for (var i = 0; i < testCases.Length; i++)
            {
                var testCase = testCases[i];
                var testResult = testResults[i];

                var tokens = method(testCase, useHmm);
                var actualResult = string.Join(" ", tokens.Select(token => string.Format("{0}/{1}", token.Word, token.Flag)));

                Assert.That(actualResult, Is.EqualTo(testResult));
            }
        }

        #endregion
    }
}