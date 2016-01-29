using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using NUnit.Framework;

namespace JiebaNet.Segmenter.Tests.FCL
{
    [TestFixture]
    public class TestPLinq
    {
        private int Double(int i)
        {
            Thread.Sleep(100);
            return i*2;
        }

        [TestCase]
        public void TestWithoutOrderPreservation()
        {
            var sw = new Stopwatch();
            sw.Start();

            var numbers = Enumerable.Range(1, 50);
            var processed = (from n in numbers.AsParallel()
                select Double(n)).ToList();
            foreach (var i in processed)
            {
                Console.WriteLine(i);
            }

            sw.Stop();
            Console.WriteLine(sw.Elapsed);
        }

        [TestCase]
        public void TestWithOrderPreservation()
        {
            var sw = new Stopwatch();
            sw.Start();

            var numbers = Enumerable.Range(1, 50);
            var processed = (from n in numbers.AsParallel().AsOrdered()
                             select Double(n)).ToList();
            foreach (var i in processed)
            {
                Console.WriteLine(i);
            }

            sw.Stop();
            Console.WriteLine(sw.Elapsed);
        }

        [TestCase]
        public void TestWithoutParallelism()
        {
            var sw = new Stopwatch();
            sw.Start();

            var numbers = Enumerable.Range(1, 50);
            var processed = (from n in numbers
                             select Double(n)).ToList();
            foreach (var i in processed)
            {
                Console.WriteLine(i);
            }

            sw.Stop();
            Console.WriteLine(sw.Elapsed);
        }

        [TestCase]
        public void TestCut()
        {
            var sw = new Stopwatch();
            sw.Start();

            var sb = new StringBuilder();
            for (int i = 0; i < 20000; i++)
            {
                sb.AppendLine("PS: 我觉得开源有一个好处，就是能够敦促自己不断改进，避免敞帚自珍");
            }

            var text = sb.ToString();
            var lines = Regex.Split(text, "\r?\n");

            var seg = new JiebaSegmenter();
            seg.Cut("热身");

            var raw = seg.Cut(text);
            Console.WriteLine(raw.Count());

            sw.Stop();
            Console.WriteLine(sw.Elapsed);

            sw.Restart();

            var processed = (from line in lines.AsParallel().AsOrdered()
                             select seg.Cut(line)).SelectMany(s => s);
            Console.WriteLine(processed.Count());

            sw.Stop();
            Console.WriteLine(sw.Elapsed);
        }
    }
}