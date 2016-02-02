using System;
using JiebaNet.Segmenter.Common;
using NUnit.Framework;

namespace JiebaNet.Segmenter.Tests.Common
{
    [TestFixture]
    public class TestTrie
    {
        private Trie GetTestTrie()
        {
            var trie = new Trie();
            trie.Insert("ann");
            trie.Insert("anders", 5);
            trie.Insert("andy", 10);

            trie.Insert("bill");
            trie.Insert("candy");
            trie.Insert("dove");

            return trie;
        }

        [Test]
        public void TestInsert()
        {
            var trie = GetTestTrie();
            Console.WriteLine(trie);
        }

        [Test]
        public void TestContains()
        {
            var trie = GetTestTrie();
            Assert.That(trie.Contains("anders"));
            Assert.That(trie.Contains("anderslly"), Is.False);
            Assert.That(trie.Contains("and"), Is.False);
            Assert.That(trie.Contains("bill"));
        }

        [Test]
        public void TestContainsPrefix()
        {
            var trie = GetTestTrie();
            Assert.That(trie.ContainsPrefix("anders"));
            Assert.That(trie.ContainsPrefix("anderslly"), Is.False);
            Assert.That(trie.ContainsPrefix("and")); // although the word is not in the dict.
            Assert.That(trie.ContainsPrefix("bill"));
        }

        [Test]
        public void TestFrequency()
        {
            var trie = GetTestTrie();
            Assert.That(trie.Frequency("anders"), Is.EqualTo(5));
            Assert.That(trie.Frequency("andy"), Is.EqualTo(10));
            Assert.That(trie.Frequency("anderslly"), Is.EqualTo(0));
            Assert.That(trie.Frequency("and"), Is.EqualTo(0));
            Assert.That(trie.Frequency("bill"), Is.EqualTo(1));
        }

        [Test]
        public void TestCountAndTotalFrequency()
        {
            var trie = GetTestTrie();
            Assert.That(trie.Count, Is.EqualTo(6));
            Assert.That(trie.TotalFrequency, Is.EqualTo(19));
        }
    }
}
