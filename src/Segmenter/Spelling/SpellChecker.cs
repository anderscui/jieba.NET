using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiebaNet.Segmenter.Common;

namespace JiebaNet.Segmenter.Spelling
{
    public interface ISpellChecker
    {
        IEnumerable<string> Suggests(string word);
    }

    public class SpellChecker : ISpellChecker
    {
        private static readonly WordDictionary WordDict = WordDictionary.Instance;
        
        internal ISet<string> GetEdits1(string word)
        {
            var splits = new List<WordSplit>();
            for (var i = 0; i <= word.Length; i++)
            {
                splits.Add(new WordSplit() { Left = word.Left(i), Right = word.Right(i) });
            }

            var deletes = splits
                .Where(s => !string.IsNullOrEmpty(s.Right))
                .Select(s => s.Left + s.Right.Substring(1));
            var transposes = splits
                .Where(s => s.Right.Length > 1)
                .Select(s => s.Left + s.Right[1] + s.Right[0] + s.Right.Substring(2));
            //var replaces; // TODO:
            //var inserts;

            var result = new HashSet<string>();
            result.UnionWith(deletes);
            result.UnionWith(transposes);

            return result;
        }

        internal ISet<string> GetKnownEdits2(string word)
        {
            var result = new HashSet<string>();
            foreach (var e1 in GetEdits1(word))
            {
                result.UnionWith(GetEdits1(e1).Where(e => WordDict.ContainsWord(e)));
            }
            return result;
        }

        internal ISet<string> GetKnownWords(IEnumerable<string> words)
        {
            return new HashSet<string>(words.Where(w => WordDict.ContainsWord(w)));
        }

        public IEnumerable<string> Suggests(string word)
        {
            var self = new List<string>() {word};

            var candicates = GetKnownWords(self);
            if (candicates.IsNotEmpty())
            {
                return candicates;
            }

            candicates.UnionWith(GetKnownWords(GetEdits1(word)));
            candicates.UnionWith(GetKnownEdits2(word));

            return candicates.OrderByDescending(c => WordDict.GetFreqOrDefault(c));
        }
    }

    internal class WordSplit
    {
        public string Left { get; set; }
        public string Right { get; set; }
    }
}
