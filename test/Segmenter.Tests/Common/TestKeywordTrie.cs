using System;

using NUnit.Framework;

using JiebaNet.Segmenter.Common;

namespace JiebaNet.Segmenter.Tests.Common
{
    [TestFixture]
    public class TestKeywordTrie
    {
        [TestCase]
        public void TestAdd()
        {
            var trie = new KeywordTrie();
            Assert.That(trie.HasValue, Is.False);
            
            // add
            trie["自然"] = "nature";
            trie["自然语言"] = "natural language";
            
            Assert.That(trie.Contains("自然"), Is.True);
            Assert.That(trie.Contains("自然语"), Is.False);

            // remove
            trie["自然"] = null;
            Assert.That(trie.Contains("自然"), Is.False);
            
            // retrieve
            Assert.That(trie["自然语言"], Is.EqualTo("natural language"));
            
            // update
            trie["自然语言"] = "human language";
            Assert.That(trie["自然语言"], Is.EqualTo("human language"));
        }
    }
}
