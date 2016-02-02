using System;
using System.IO;
using System.Linq;
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

        [Test]
        public void TestWordDictToTrie()
        {
            var trie = GetWordDictTrie();
            //Console.WriteLine(wordDict.Trie.Sum(p => p.Value > 0 ? 1 : 0));
            Console.WriteLine(trie.Count);
            Console.WriteLine(trie.TotalFrequency);

            Assert.That(trie.Contains("不列颠"));

            Assert.That(trie.ContainsPrefix("不列"));
            Assert.That(trie.Contains("不列"), Is.False);
        }

        [Test]
        public void TestNextCharsOf()
        {
            var trie = GetWordDictTrie();

            var chars = trie.NextCharsOf("天地");
            Console.WriteLine(chars.Count());
            foreach (var c in chars)
            {
                Console.WriteLine(c);
            }
        }

        private Trie GetWordDictTrie()
        {
            var wordDict = WordDictionary.Instance;
            var trie = new Trie();
            foreach (var wd in wordDict.Trie)
            {
                trie.Insert(wd.Key, wd.Value);
            }
            return trie;
        }
    }
}