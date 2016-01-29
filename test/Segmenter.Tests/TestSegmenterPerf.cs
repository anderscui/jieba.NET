using System;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;

namespace JiebaNet.Segmenter.Tests
{
    [TestFixture]
    public class TestSegmenterPerf
    {
        private string[] GetTestText()
        {
            return File.ReadAllLines(@"Cases\jieba_test.txt");
        }

        [TestCase]
        [Ignore]
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
    }
}