using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using JiebaNet.Segmenter.FinalSeg;

namespace JiebaNet.Segmenter
{
    public class JiebaSegmenter
    {
        private static readonly WordDictionary WordDict = WordDictionary.Instance;
        private static readonly IFinalSeg FinalSeg = Viterbi.Instance;

        public static readonly Regex RegexChineseDefault = new Regex(@"([\u4E00-\u9FA5a-zA-Z0-9+#&\._]+)", RegexOptions.Compiled);

        /// <summary>
        /// Whitespace or new line.
        /// </summary>
        public static readonly Regex RegexSkipDefault = new Regex(@"(\r\n|\s)", RegexOptions.Compiled);

        public static readonly Regex RegexChineseCutAll = new Regex(@"([\u4E00-\u9FA5]+)", RegexOptions.Compiled);
        public static readonly Regex RegexSkipCutAll = new Regex(@"[^a-zA-Z0-9+#\n]", RegexOptions.Compiled);

        public static readonly Regex RegexEnglishChars = new Regex(@"[a-zA-Z0-9]", RegexOptions.Compiled);

        public IDictionary<int, List<int>> GetDAG(string sentence)
        {
            var dag = new Dictionary<int, List<int>>();
            var trie = WordDict.Trie;

            var N = sentence.Length;
            for (var k = 0; k < sentence.Length; k++)
            {
                var templist = new List<int>();
                var i = k;
                var frag = sentence.Substring(k, 1);
                while (i < N && trie.ContainsKey(frag))
                {
                    if (trie[frag] > 0)
                    {
                        templist.Add(i);
                    }

                    i++;
                    // TODO:
                    if (i < N)
                    {
                        frag = sentence.Sub(k, i + 1);
                    }
                }
                if (templist.Count == 0)
                {
                    templist.Add(k);
                }
                dag[k] = templist;
            }

            return dag;
        }

        public IDictionary<int, Pair<int>> Calc(string sentence, IDictionary<int, List<int>> dag)
        {
            var N = sentence.Length;

            var route = new Dictionary<int, Pair<int>>();
            route[N] = new Pair<int>(0, 0.0);

            var logtotal = Math.Log(WordDict.Total);
            for (var i = N - 1; i > -1; i--)
            {
                var candidate = new Pair<int>(-1, double.MinValue);
                foreach (int x in dag[i])
                {
                    var freq = Math.Log(WordDict.GetFreqOrDefault(sentence.Sub(i, x + 1))) - logtotal + route[x + 1].freq;
                    if (candidate.freq < freq)
                    {
                        candidate.freq = freq;
                        candidate.key = x;
                    }
                }
                route[i] = candidate;
            }
            return route;
        }

        public IEnumerable<string> CutAll(string sentence)
        {
            var dag = GetDAG(sentence);

            var words = new List<string>();
            var old_j = -1;

            foreach (var pair in dag)
            {
                var k = pair.Key;
                var L = pair.Value;
                if (L.Count == 1 && k > old_j)
                {
                    words.Add(sentence.Substring(k, L[0] + 1 - k));
                    old_j = L[0];
                }
                else
                {
                    foreach (var j in L)
                    {
                        if (j > k)
                        {
                            words.Add(sentence.Substring(k, j + 1 - k));
                            old_j = j;
                        }
                    }
                }
            }

            return words;
        }

        public IEnumerable<string> CutDAG(string sentence)
        {
            var dag = GetDAG(sentence);
            var route = Calc(sentence, dag);

            var tokens = new List<string>();

            var x = 0;
            var n = sentence.Length;
            var buf = string.Empty;
            while (x < n)
            {
                var y = route[x].key + 1;
                var w = sentence.Substring(x, y - x);
                if (y - x == 1)
                {
                    buf += w;
                }
                else
                {
                    if (buf.Length > 0)
                    {
                        AddBufferToWordList(tokens, buf);
                        buf = string.Empty;
                    }
                    tokens.Add(w);
                }
                x = y;
            }

            if (buf.Length > 0)
            {
                AddBufferToWordList(tokens, buf);
            }

            return tokens;
        }

        public IEnumerable<string> CutDAGWithoutHmm(string sentence)
        {
            var dag = GetDAG(sentence);
            var route = Calc(sentence, dag);

            var words = new List<string>();

            var x = 0;
            string buf = string.Empty;
            var N = sentence.Length;

            var y = -1;
            while (x < N)
            {
                y = route[x].key + 1;
                var l_word = sentence.Substring(x, y - x);
                if (RegexEnglishChars.IsMatch(l_word) && l_word.Length == 1)
                {
                    buf += l_word;
                    x = y;
                }
                else
                {
                    if (buf.Length > 0)
                    {
                        words.Add(buf);
                        buf = string.Empty;
                    }
                    words.Add(l_word);
                    x = y;
                }
            }

            if (buf.Length > 0)
            {
                words.Add(buf);
            }

            return words;
        }

        /// <summary>
        /// The main function that segments an entire sentence that contains 
        /// Chinese characters into seperated words.
        /// </summary>
        /// <param name="text">The string to be segmented.</param>
        /// <param name="cutAll">Model type. True for full pattern, False for accurate pattern.</param>
        /// <param name="hmm">Whether to use the Hidden Markov Model.</param>
        /// <returns></returns>
        public IEnumerable<string> Cut(string text, bool cutAll = false, bool hmm = true)
        {
            var reHan = RegexChineseDefault;
            var reSkip = RegexSkipDefault;
            Func<string, IEnumerable<string>> cutMethod = CutDAG;

            if (cutAll)
            {
                reHan = RegexChineseCutAll;
                reSkip = RegexSkipCutAll;
            }

            // TODO: ?:
            if (cutAll)
            {
                cutMethod = CutAll;
            }
            else if (hmm)
            {
                cutMethod = CutDAG;
            }
            else
            {
                cutMethod = CutDAGWithoutHmm;
            }

            var result = new List<string>();
            var blocks = reHan.Split(text);
            foreach (var blk in blocks)
            {
                if (string.IsNullOrWhiteSpace(blk))
                {
                    continue;
                }

                if (reHan.IsMatch(blk))
                {
                    foreach (var word in cutMethod(blk))
                    {
                        result.Add(word);
                    }
                }
                else
                {
                    var tmp = reSkip.Split(blk);
                    foreach (var x in tmp)
                    {
                        if (reSkip.IsMatch(x))
                        {
                            result.Add(x);
                        }
                        else if (!cutAll)
                        {
                            foreach (var ch in x)
                            {
                                result.Add(ch.ToString());
                            }
                        }
                        else
                        {
                            result.Add(x);
                        }
                    }
                }
            }

            return result;
        }

        public IEnumerable<string> CutForSearch(string text, bool hmm = true)
        {
            var result = new List<string>();

            var words = Cut(text, hmm: hmm);
            foreach (var w in words)
            {
                if (w.Length > 2)
                {
                    foreach (var i in Enumerable.Range(0, w.Length - 1))
                    {
                        var gram2 = w.Substring(i, 2);
                        if (WordDict.ContainsWord(gram2))
                        {
                            result.Add(gram2);
                        }
                    }
                }

                if (w.Length > 3)
                {
                    foreach (var i in Enumerable.Range(0, w.Length - 2))
                    {
                        var gram3 = w.Substring(i, 3);
                        if (WordDict.ContainsWord(gram3))
                        {
                            result.Add(gram3);
                        }
                    }
                }

                result.Add(w);
            }

            return result;
        }

        public IEnumerable<Token> Tokenize(string text, TokenizerMode mode = TokenizerMode.Default, bool hmm = true)
        {
            var result = new List<Token>();

            var start = 0;
            if (mode == TokenizerMode.Default)
            {
                foreach (var w in Cut(text, hmm: hmm))
                {
                    var width = w.Length;
                    result.Add(new Token(w, start, start + width));
                    start += width;
                }
            }
            else
            {
                foreach (var w in Cut(text, hmm: hmm))
                {
                    var width = w.Length;
                    if (width > 2)
                    {
                        for (int i = 0; i < width - 1; i++)
                        {
                            var gram2 = w.Substring(i, 2);
                            if (WordDict.ContainsWord(gram2))
                            {
                                result.Add(new Token(gram2, start + i, start + i + 2));
                            }
                        }
                    }
                    if (width > 3)
                    {
                        for (int i = 0; i < width - 2; i++)
                        {
                            var gram3 = w.Substring(i, 3);
                            if (WordDict.ContainsWord(gram3))
                            {
                                result.Add(new Token(gram3, start + i, start + i + 3));
                            }
                        }
                    }

                    result.Add(new Token(w, start, start + width));
                    start += width;
                }
            }

            return result;
        }

        #region Extend Main Dict

        public void LoadUserDict(string dictFile)
        {
            WordDict.LoadUserDict(dictFile);
        }

        public void AddWord(string word, int freq = 0, string tag = null)
        {
            if (freq <= 0)
            {
                freq = WordDict.SuggestFreq(word, Cut(word, hmm: false));
            }
            WordDict.AddWord(word, freq);
        }

        public void DeleteWord(string word)
        {
            WordDict.DeleteWord(word);
        }

        #endregion

        #region Private Helpers

        private void AddBufferToWordList(List<string> words, string buf)
        {
            if (buf.Length == 1)
            {
                words.Add(buf);
            }
            else
            {
                if (!WordDict.ContainsWord(buf))
                {
                    var tokens = FinalSeg.Cut(buf);
                    words.AddRange(tokens);
                }
                else
                {
                    words.AddRange(buf.Select(ch => ch.ToString()));
                }
            }
        }

        #endregion
    }

    public enum TokenizerMode
    {
        Default,
        Search
    }
}