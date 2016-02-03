using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JiebaNet.Analyser;
using NUnit.Framework;

namespace JiebaNet.Segmenter.Tests
{
    [TestFixture]
    public class TestDictStats
    {
        [TestCase]
        public void TestExtractTags()
        {
            var dict = WordDictionary.Instance;

            // TODO: add custom default dict.
            var dict1 = new Dictionary<char, int>();
            
            var dict2 = new Dictionary<char, int>();
            foreach (var s in dict.Trie)
            {
                if (!dict1.ContainsKey(s.Key[0]))
                {
                    dict1[s.Key[0]] = 0;
                }

                dict1[s.Key[0]] += 1;

                if (s.Key.Length >= 2)
                {
                    if (!dict2.ContainsKey(s.Key[1]))
                    {
                        dict2[s.Key[1]] = 0;
                    }

                    dict2[s.Key[1]] += 1;
                }
            }

            var total = dict.Trie.Count;
            var n1 = dict1.Count;
            var avg1 = ((double) total)/n1;
            var top500 = dict1.OrderByDescending(d => d.Value).Take(500);

            Console.WriteLine(n1);
            Console.WriteLine(avg1);

            var topCount = 0;
            foreach (var pair in top500)
            {
                topCount += pair.Value;
                Console.WriteLine("{0}: {1}", pair.Key, pair.Value);
            }
            Console.WriteLine(topCount);
        }
    }
}