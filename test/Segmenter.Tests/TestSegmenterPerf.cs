using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using JiebaNet.Segmenter.Common;
using NUnit.Framework;

namespace JiebaNet.Segmenter.Tests
{
    [TestFixture]
    [Ignore]
    public class TestSegmenterPerf
    {
        private string[] GetTestText()
        {
            return File.ReadAllLines(@"Cases\jieba_test.txt");
        }

        private string[] GetTestSentences()
        {
            var sentences = File.ReadAllText(@"Cases\jieba_test.txt");
            var more = new List<string>();
            for (int i = 0; i < 1000; i++)
            {
                more.Add(sentences);
            }

            return more.ToArray();
        }

        [TestCase]
        public void TestCutLargeFile()
        {
            var fileName = @"Resources\围城.txt";
            var weiCheng = File.ReadAllText(fileName);
            var fileSize = (new FileInfo(fileName)).Length;

            var seg = new JiebaSegmenter();
            seg.Cut("热身一下");

            Console.WriteLine("Start to cut");
            const int n = 20;
            var stopWatch = new Stopwatch();

            // Accurate mode
            stopWatch.Start();

            for (var i = 0; i < n; i++)
            {
                seg.Cut(weiCheng);
            }

            stopWatch.Stop();
            var timeConsumed = (double)stopWatch.ElapsedMilliseconds / (1000 * n);
            Console.WriteLine("Accurate mode: {0} ms, average: {1} / second",
                                timeConsumed, fileSize / timeConsumed);

            // Full mode
            stopWatch.Reset();
            stopWatch.Start();

            for (var i = 0; i < n; i++)
            {
                seg.Cut(weiCheng, true);
            }

            stopWatch.Stop();

            timeConsumed = (double)stopWatch.ElapsedMilliseconds / (1000 * n);
            Console.WriteLine("Full mode: {0} ms, average: {1} / second",
                                timeConsumed, fileSize / timeConsumed);
        }

        [TestCase]
        public void TestCutLargeFileParallelly()
        {
            var fileName = @"Resources\围城.txt";
            var weiCheng = File.ReadAllText(fileName);
            var fileSize = (new FileInfo(fileName)).Length;

            var seg = new JiebaSegmenter(true);
            seg.Cut("热身一下");

            Console.WriteLine("Start to cut");
            const int n = 20;
            var stopWatch = new Stopwatch();

            // Accurate mode
            stopWatch.Start();

            for (var i = 0; i < n; i++)
            {
                seg.Cut(weiCheng);
            }

            stopWatch.Stop();
            var timeConsumed = (double)stopWatch.ElapsedMilliseconds / (1000 * n);
            Console.WriteLine("Accurate mode: {0} ms, average: {1} / second",
                                timeConsumed, fileSize / timeConsumed);

            // Full mode
            stopWatch.Reset();
            stopWatch.Start();

            for (var i = 0; i < n; i++)
            {
                seg.Cut(weiCheng, true);
            }

            stopWatch.Stop();

            timeConsumed = (double)stopWatch.ElapsedMilliseconds / (1000 * n);
            Console.WriteLine("Full mode: {0} ms, average: {1} / second",
                                timeConsumed, fileSize / timeConsumed);
        }

        [TestCase]
        public void TestCutManySentences()
        {
            var text = GetTestSentences().Join(string.Empty);
            var fileSize = 1532 * 100;

            var seg = new JiebaSegmenter();
            seg.Cut("热身一下");

            Console.WriteLine("Start to cut");
            const int n = 20;
            var stopWatch = new Stopwatch();

            // Accurate mode
            stopWatch.Start();

            for (var i = 0; i < n; i++)
            {
                seg.Cut(text);
            }

            stopWatch.Stop();
            var timeConsumed = (double)stopWatch.ElapsedMilliseconds / (1000 * n);
            Console.WriteLine("Accurate mode: {0} ms, average: {1} / second",
                                timeConsumed, fileSize / timeConsumed);

            // Full mode
            stopWatch.Reset();
            stopWatch.Start();

            for (var i = 0; i < n; i++)
            {
                seg.Cut(text, true);
            }

            stopWatch.Stop();

            timeConsumed = (double)stopWatch.ElapsedMilliseconds / (1000 * n);
            Console.WriteLine("Full mode: {0} ms, average: {1} / second",
                                timeConsumed, fileSize / timeConsumed);
        }

        [TestCase]
        public void TestCutManySentencesParallelly()
        {
            var text = GetTestSentences().Join(string.Empty);
            var fileSize = 1532 * 100;

            var seg = new JiebaSegmenter(true);
            seg.Cut("热身一下");

            Console.WriteLine("Start to cut");
            const int n = 20;
            var stopWatch = new Stopwatch();

            // Accurate mode
            stopWatch.Start();

            for (var i = 0; i < n; i++)
            {
                seg.Cut(text);
            }

            stopWatch.Stop();
            var timeConsumed = (double)stopWatch.ElapsedMilliseconds / (1000 * n);
            Console.WriteLine("Accurate mode: {0} ms, average: {1} / second",
                                timeConsumed, fileSize / timeConsumed);

            // Full mode
            stopWatch.Reset();
            stopWatch.Start();

            for (var i = 0; i < n; i++)
            {
                seg.Cut(text, true);
            }

            stopWatch.Stop();

            timeConsumed = (double)stopWatch.ElapsedMilliseconds / (1000 * n);
            Console.WriteLine("Full mode: {0} ms, average: {1} / second",
                                timeConsumed, fileSize / timeConsumed);
        }
    }
}