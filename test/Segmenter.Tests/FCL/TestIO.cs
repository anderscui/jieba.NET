using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using JiebaNet.Segmenter.Common;
using NUnit.Framework;
using NUnit.Framework.Internal;
using OSPlatform = NUnit.Framework.Internal.OSPlatform;

namespace JiebaNet.Segmenter.Tests.FCL
{
    [TestFixture]
    public class TestIO
    {
        [TestCase]
        public void TestNormalizePath()
        {
            if (TestHelper.IsOnWindows())
            {
                var p = @"..\test.txt";
                Assert.That(Path.IsPathRooted(p), Is.False);
                Console.WriteLine(Path.GetFullPath(p));

                p = @"C:\test.txt";
                Assert.That(Path.IsPathRooted(p), Is.True);
                Console.WriteLine(Path.GetFullPath(p));

                p = @"c:\a\b\c\..\test.txt";
                Assert.That(Path.IsPathRooted(p), Is.True);
                Console.WriteLine(Path.GetFullPath(p));
            }
            else
            {
                var p = @"../test.txt";
                Assert.That(Path.IsPathRooted(p), Is.False);
                Console.WriteLine(Path.GetFullPath(p));

                p = @"/users/a/test.txt";
                Assert.That(Path.IsPathRooted(p), Is.True);
                Assert.That(Path.GetFullPath(p), Is.EqualTo("/users/a/test.txt"));

                p = @"/users/a/b/c/../test.txt";
                Assert.That(Path.IsPathRooted(p), Is.True);
                Assert.That(Path.GetFullPath(p), Is.EqualTo("/users/a/b/test.txt"));
            }
        }

        [TestCase]
        public void TestReadFilePerf()
        {
            ReadLines(TestHelper.GetResourceFilePath("dict.txt"));
            ReadStreamReader(TestHelper.GetResourceFilePath("dict.txt"));
        }

        private void ReadLines(string filePath)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var lines = File.ReadAllLines(filePath, Encoding.UTF8);
            foreach (var line in lines)
            {
                var tokens = line.Split(' ');
                if (tokens.Length < 2)
                {
                    continue;
                }

                var word = tokens[0];
                var freq = int.Parse(tokens[1]);

                foreach (var ch in Enumerable.Range(0, word.Length))
                {
                    var wfrag = word.Sub(0, ch + 1);
                }
            }

            stopWatch.Stop();
            Console.WriteLine(stopWatch.ElapsedMilliseconds);
        }

        private void ReadStreamReader(string filePath)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            using (var sr = new StreamReader(filePath, Encoding.UTF8))
            {
                string line = null;
                while ((line = sr.ReadLine()) != null)
                {
                    var tokens = line.Split(' ');
                    if (tokens.Length < 2)
                    {
                        continue;
                    }

                    var word = tokens[0];
                    var freq = int.Parse(tokens[1]);

                    foreach (var ch in Enumerable.Range(0, word.Length))
                    {
                        var wfrag = word.Sub(0, ch + 1);
                    }
                }
            }

            stopWatch.Stop();
            Console.WriteLine(stopWatch.ElapsedMilliseconds);
        }
    }
}