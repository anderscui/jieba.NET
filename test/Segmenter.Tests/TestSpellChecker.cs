using System;
using System.IO;
using JiebaNet.Analyser;
using JiebaNet.Segmenter.Common;
using JiebaNet.Segmenter.Spelling;
using NUnit.Framework;

namespace JiebaNet.Segmenter.Tests
{
    [TestFixture]
    public class TestSpellChecker
    {
        [TestCase]
        public void TestGetEdits1()
        {
            var s = "技术控";
            var checker = new SpellChecker();

            var edits1 = checker.GetEdits1(s);
            foreach (var edit in edits1)
            {
                Console.WriteLine(edit);
            }

            var edits2 = checker.GetKnownEdits2(s);
            foreach (var e2 in edits2)
            {
                Console.WriteLine(e2);
            }
            Console.WriteLine("-----");

            var sugguests = checker.Suggests(s);
            foreach (var sugguest in sugguests)
            {
                Console.WriteLine(sugguest);
            }
        }
    }
}