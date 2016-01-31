using System;
using System.Text.RegularExpressions;
using JiebaNet.Segmenter.Common;
using NUnit.Framework;

namespace JiebaNet.Segmenter.Tests.FCL
{
    [TestFixture]
    public class TestRegex
    {
        [TestCase]
        public void TestMatches()
        {
            var re = new Regex(@"\d+");
            var input = "12 34 567";
            var mat = re.Matches(input);
            foreach (Match m in mat)
            {
                Console.WriteLine("{0} {1}", m.Index, m.Length);
            }
        }

        [TestCase]
        public void TestNumbers()
        {
            var re = new Regex("[.0-9]+");
            Assert.That(re.IsMatch("..."));
            Assert.That(re.IsMatch("123"));
            Assert.That(re.IsMatch("1.23"));
            Assert.That(re.IsMatch("1.2.3"));
            Assert.That(re.IsMatch("a.1.2.3"));
        }

        [TestCase]
        public void TestFloatNumbers()
        {
            Console.WriteLine(double.Parse("-4.762305214596967"));
            Console.WriteLine(double.Parse("-3.14e+100"));
        }

        [TestCase]
        public void TestUserDictRegex()
        {
            var reUserDict = new Regex("^(?<word>.+?)(?<freq> [0-9]+)?(?<tag> [a-z]+)?$", RegexOptions.Compiled);
            var s = "Steve Jobs 1000 nr";
            var mat = reUserDict.Match(s);
            Assert.That(mat.Groups.Count, Is.EqualTo(4));
            Assert.That(mat.Groups["word"].Value.Trim(), Is.EqualTo("Steve Jobs"));
            Assert.That(mat.Groups["freq"].Value.Trim(), Is.EqualTo("1000"));
            Assert.That(mat.Groups["tag"].Value.Trim(), Is.EqualTo("nr"));

            s = "Steve Jobs 1000";
            mat = reUserDict.Match(s);
            Assert.That(mat.Groups.Count, Is.EqualTo(4));
            Assert.That(mat.Groups["word"].Value.Trim(), Is.EqualTo("Steve Jobs"));
            Assert.That(mat.Groups["freq"].Value.Trim(), Is.EqualTo("1000"));
            Assert.That(mat.Groups["tag"].Value.Trim(), Is.EqualTo(""));

            s = "Steve Jobs";
            mat = reUserDict.Match(s);
            Assert.That(mat.Groups.Count, Is.EqualTo(4));
            Assert.That(mat.Groups["word"].Value.Trim(), Is.EqualTo("Steve Jobs"));
            Assert.That(mat.Groups["freq"].Value.Trim(), Is.EqualTo(""));
            Assert.That(mat.Groups["tag"].Value.Trim(), Is.EqualTo(""));
            CollectionAssert.AreEqual(mat.Groups.SubGroupValues(), new [] { "Steve Jobs", "", "" });
        }

        [TestCase]
        public void TestSplitLines_EmptyString()
        {
            var s = string.Empty;
            var lines = s.SplitLines();
            Assert.That(lines.Length, Is.EqualTo(1));
            Assert.That(lines[0], Is.EqualTo(string.Empty));
        }

        [TestCase]
        public void TestSplitLines_Consistent_Newline()
        {
            var s = "a\r\nb\r\nc";
            var lines = s.SplitLines();
            Assert.That(lines.Length, Is.EqualTo(5));

            s = "a\nb\nc";
            lines = s.SplitLines();
            Assert.That(lines.Length, Is.EqualTo(5));

            s = "a\rb\rc";
            lines = s.SplitLines();
            Assert.That(lines.Length, Is.EqualTo(5));
        }

        [TestCase]
        public void TestSplitLines_Inconsistent_Newline()
        {
            var s = "a\r\nb\nc\rd";
            var lines = s.SplitLines();
            Assert.That(lines.Length, Is.EqualTo(7));

            s = "a\nb\rc";
            lines = s.SplitLines();
            Assert.That(lines.Length, Is.EqualTo(5));

            s = "a\rb\nc";
            lines = s.SplitLines();
            Assert.That(lines.Length, Is.EqualTo(5));
        }
    }
}