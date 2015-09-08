using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;

namespace JiebaNet.Segmenter.Tests
{
    [TestFixture]
    public class TestSegmenterPerf
    {
        private string[] GetTestSentences()
        {
            return File.ReadAllLines(@"Cases\jieba_test.txt");
        }

        [TestCase]
        [Ignore]
        public void TestCutLargeFile()
        {
            var weiCheng = File.ReadAllText(@"Resources\围城.txt");
            var seg = new JiebaSegmenter();
            seg.Cut("热身");

            Console.WriteLine("Start to cut");
            var n = 20;
            var stopWatch = new Stopwatch();

            // Accurate mode
            stopWatch.Start();

            for (var i = 0; i < n; i++)
            {
                seg.Cut(weiCheng);
            }
            
            stopWatch.Stop();
            Console.WriteLine("Accurate mode: {0} ms", stopWatch.ElapsedMilliseconds / n);

            // Full mode
            stopWatch.Reset();
            stopWatch.Start();

            for (var i = 0; i < n; i++)
            {
                seg.Cut(weiCheng, true);
            }

            stopWatch.Stop();
            Console.WriteLine("Full mode: {0} ms", stopWatch.ElapsedMilliseconds / n);
        }

        #region Private Helpers

        private void TestCutFunction(Func<string, bool, bool, IEnumerable<string>> method,
                                     bool cutAll, bool useHmm,
                                     string testResultFile)
        {
            var testCases = GetTestSentences();
            var testResults = File.ReadAllLines(testResultFile);
            Assert.That(testCases.Length, Is.EqualTo(testResults.Length));
            for (int i = 0; i < testCases.Length; i++)
            {
                var testCase = testCases[i];
                var testResult = testResults[i];
                Assert.That(method(testCase, cutAll, useHmm).Join("/ "), Is.EqualTo(testResult));
            }
        }

        #endregion
    }
}