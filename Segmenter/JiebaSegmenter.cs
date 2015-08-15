using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JiebaNet.Segmenter.Viterbi;

namespace JiebaNet.Segmenter
{
    public class JiebaSegmenter
    {
        private static WordDictionary wordDict = WordDictionary.getInstance();
        private static FinalSeg finalSeg = FinalSeg.getInstance();

        public enum SegMode
        {
            INDEX,
            SEARCH
        }

        private IDictionary<int, List<int>> createDAG(string sentence)
        {
            IDictionary<int, List<int>> dag = new Dictionary<int, List<int>>();
            DictSegment trie = wordDict.getTrie();
            char[] chars = sentence.ToCharArray();
            int N = chars.Length;
            int i = 0, j = 0;
            while (i < N)
            {
                Hit hit = trie.match(chars, i, j - i + 1);
                if (hit.isPrefix() || hit.isMatch())
                {
                    if (hit.isMatch())
                    {
                        if (!dag.ContainsKey(i))
                        {
                            List<int> value = new List<int>();
                            dag[i] = value;
                            value.Add(j);
                        }
                        else
                            dag[i].Add(j);
                    }
                    j += 1;
                    if (j >= N)
                    {
                        i += 1;
                        j = i;
                    }
                }
                else
                {
                    i += 1;
                    j = i;
                }
            }

            for (i = 0; i < N; ++i)
            {
                if (!dag.ContainsKey(i))
                {
                    List<int> value = new List<int>();
                    value.Add(i);
                    dag[i] = value;
                }
            }
            return dag;
        }

        private IDictionary<int, Pair<int>> calc(string sentence, IDictionary<int, List<int>> dag)
        {
            int N = sentence.Length;
            Dictionary<int, Pair<int>> route = new Dictionary<int, Pair<int>>();
            route[N] = new Pair<int>(0, 0.0);
            for (int i = N - 1; i > -1; i--)
            {
                Pair<int> candidate = null;
                foreach (int x in dag[i])
                {
                    double freq = wordDict.getFreq(sentence.Sub(i, x + 1)) + route[x + 1].freq;
                    if (null == candidate)
                    {
                        candidate = new Pair<int>(x, freq);
                    }
                    else if (candidate.freq < freq)
                    {
                        candidate.freq = freq;
                        candidate.key = x;
                    }
                }
                route[i] = candidate;
            }
            return route;
        }

        public List<SegToken> process(string paragraph, SegMode mode)
        {
            List<SegToken> tokens = new List<SegToken>();
            StringBuilder sb = new StringBuilder();
            int offset = 0;
            for (int i = 0; i < paragraph.Length; ++i)
            {
                char ch = CharacterUtil.regularize(paragraph[i]);
                if (CharacterUtil.ccFind(ch))
                    sb.Append(ch);
                else
                {
                    if (sb.Length > 0)
                    {
                        // process
                        if (mode == SegMode.SEARCH)
                        {
                            foreach (string word in sentenceProcess(sb.ToString()))
                            {
                                tokens.Add(new SegToken(word, offset, offset += word.Length));
                            }
                        }
                        else
                        {
                            foreach (string token in sentenceProcess(sb.ToString()))
                            {
                                if (token.Length > 2)
                                {
                                    string gram2;
                                    int j = 0;
                                    for (; j < token.Length - 1; ++j)
                                    {
                                        gram2 = token.Sub(j, j + 2);
                                        if (wordDict.containsWord(gram2))
                                            tokens.Add(new SegToken(gram2, offset + j, offset + j + 2));
                                    }
                                }
                                if (token.Length > 3)
                                {
                                    string gram3;
                                    int j = 0;
                                    for (; j < token.Length - 2; ++j)
                                    {
                                        gram3 = token.Sub(j, j + 3);
                                        if (wordDict.containsWord(gram3))
                                            tokens.Add(new SegToken(gram3, offset + j, offset + j + 3));
                                    }
                                }
                                tokens.Add(new SegToken(token, offset, offset += token.Length));
                            }
                        }
                        sb = new StringBuilder();
                        offset = i;
                    }
                    if (wordDict.containsWord(paragraph.Sub(i, i + 1)))
                        tokens.Add(new SegToken(paragraph.Sub(i, i + 1), offset, ++offset));
                    else
                        tokens.Add(new SegToken(paragraph.Sub(i, i + 1), offset, ++offset));
                }
            }
            if (sb.Length > 0)
                if (mode == SegMode.SEARCH)
                {
                    foreach (string token in sentenceProcess(sb.ToString()))
                    {
                        tokens.Add(new SegToken(token, offset, offset += token.Length));
                    }
                }
                else
                {
                    foreach (string token in sentenceProcess(sb.ToString()))
                    {
                        if (token.Length > 2)
                        {
                            string gram2;
                            int j = 0;
                            for (; j < token.Length - 1; ++j)
                            {
                                gram2 = token.Sub(j, j + 2);
                                if (wordDict.containsWord(gram2))
                                    tokens.Add(new SegToken(gram2, offset + j, offset + j + 2));
                            }
                        }
                        if (token.Length > 3)
                        {
                            string gram3;
                            int j = 0;
                            for (; j < token.Length - 2; ++j)
                            {
                                gram3 = token.Sub(j, j + 3);
                                if (wordDict.containsWord(gram3))
                                    tokens.Add(new SegToken(gram3, offset + j, offset + j + 3));
                            }
                        }
                        tokens.Add(new SegToken(token, offset, offset += token.Length));
                    }
                }

            return tokens;
        }

        public List<string> sentenceProcess(String sentence)
        {
            List<String> tokens = new List<String>();
            int N = sentence.Length;
            IDictionary<int, List<int>> dag = createDAG(sentence);
            IDictionary<int, Pair<int>> route = calc(sentence, dag);

            int x = 0;
            int y = 0;
            String buf;
            StringBuilder sb = new StringBuilder();
            while (x < N)
            {
                y = route[x].key + 1;
                string lWord = sentence.Sub(x, y);
                if (y - x == 1)
                    sb.Append(lWord);
                else
                {
                    if (sb.Length > 0)
                    {
                        buf = sb.ToString();
                        sb = new StringBuilder();
                        if (buf.Length == 1)
                        {
                            tokens.Add(buf);
                        }
                        else
                        {
                            if (wordDict.containsWord(buf))
                            {
                                tokens.Add(buf);
                            }
                            else
                            {
                                finalSeg.cut(buf, tokens);
                            }
                        }
                    }
                    tokens.Add(lWord);
                }
                x = y;
            }
            buf = sb.ToString();
            if (buf.Length > 0)
            {
                if (buf.Length == 1)
                {
                    tokens.Add(buf);
                }
                else
                {
                    if (wordDict.containsWord(buf))
                    {
                        tokens.Add(buf);
                    }
                    else
                    {
                        finalSeg.cut(buf, tokens);
                    }
                }
            }
            return tokens;
        }

        public List<string> cut(string paragraph, SegMode mode = SegMode.SEARCH)
        {
            var tokens = process(paragraph, mode);
            return tokens.Select(t => paragraph.Sub(t.startOffset, t.endOffset)).ToList();
        }
    }
}