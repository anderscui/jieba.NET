using System.Collections.Generic;
using System.Linq;
using JiebaNet.Segmenter.Common;

namespace JiebaNet.Segmenter
{
    public class KeywordProcessor
    {
        // private readonly string _keyword = "_keyword_";
        // private readonly ISet<char> _whiteSpaceChars = new HashSet<char>(".\t\n\a ,");
        // private readonly bool CaseSensitive;
        private readonly KeywordTrie KeywordTrie = new KeywordTrie();

        private readonly ISet<char> NonWordBoundries =
            new HashSet<char>("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_");

        public bool CaseSensitive { get; }

        public KeywordProcessor(bool caseSensitive = false)
        {
            CaseSensitive = caseSensitive;
        }
        
        public void AddKeyword(string keyword, string cleanName = null)
        {
            SetItem(keyword, cleanName);
        }

        public void AddKeywords(IEnumerable<string> keywords)
        {
            foreach (var keyword in keywords)
            {
                AddKeyword(keyword);
            }
        }

        public void RemoveKeyword(string keyword)
        {
            if (!CaseSensitive)
            {
                keyword = keyword.ToLower();
            }
            KeywordTrie.Remove(keyword);
        }
        
        public void RemoveKeywords(IEnumerable<string> keywords)
        {
            foreach (var keyword in keywords)
            {
                RemoveKeyword(keyword);
            }
        }

        public bool Contains(string word)
        {
            return GetItem(word).IsNotNull();
        }

        public IEnumerable<TextSpan> ExtractKeywordSpans(string sentence)
        {
            var keywordsExtracted = new List<TextSpan>();
            if (sentence.IsEmpty())
            {
                return keywordsExtracted;
            }

            if (!CaseSensitive)
            {
                sentence = sentence.ToLower();
            }

            KeywordTrieNode currentState = KeywordTrie;
            var seqStartPos = 0;
            var seqEndPos = 0;
            var resetCurrentDict = false;
            var idx = 0;
            var sentLen = sentence.Length;
            while (idx < sentLen)
            {
                var ch = sentence[idx];
                // when reaching a char that denote word end
                if (!NonWordBoundries.Contains(ch))
                {
                    // if current prefix is in trie
                    if (currentState.HasValue || currentState.HasChild(ch))
                    {
                        //string seqFound = null;
                        string longestFound = null;
                        var isLongerFound = false;
                        
                        if (currentState.HasValue)
                        {
                            //seqFound = currentState.Value;
                            longestFound = currentState.Value;
                            seqEndPos = idx;
                        }

                        // re look for longest seq from this position
                        if (currentState.HasChild(ch))
                        {
                            var curStateContinued = currentState.GetChild(ch);
                            var idy = idx + 1;
                            while (idy < sentLen)
                            {
                                var innerCh = sentence[idy];
                                if (!NonWordBoundries.Contains(innerCh) && curStateContinued.HasValue)
                                {
                                    longestFound = curStateContinued.Value;
                                    seqEndPos = idy;
                                    isLongerFound = true;
                                }

                                if(curStateContinued.HasChild(innerCh))
                                {
                                    curStateContinued = curStateContinued.GetChild(innerCh);
                                }
                                else
                                {
                                    break;
                                }

                                idy += 1;
                            }

                            if (idy == sentLen && curStateContinued.HasValue)
                            {
                                // end of sentence reached.
                                longestFound = curStateContinued.Value;
                                seqEndPos = idy;
                                isLongerFound = true;
                            }

                            if (isLongerFound)
                            {
                                idx = seqEndPos;
                            }
                        }
                        
                        if (longestFound.IsNotEmpty())
                        {
                            keywordsExtracted.Add(new TextSpan(text: longestFound, start: seqStartPos, end: idx));
                        }

                        currentState = KeywordTrie;
                        resetCurrentDict = true;
                    }
                    else
                    {
                        currentState = KeywordTrie;
                        resetCurrentDict = true;
                    }
                }
                else if (currentState.HasChild(ch))
                {
                    currentState = currentState.GetChild(ch);
                }
                else
                {
                    currentState = KeywordTrie;
                    resetCurrentDict = true;
                    
                    // skip to end of word
                    var idy = idx + 1;
                    while (idy < sentLen)
                    {
                        if (!NonWordBoundries.Contains(sentence[idy]))
                        {
                            break;
                        }
                        idy += 1;
                    }

                    idx = idy;
                }

                if (idx + 1 >= sentLen)
                {
                    if (currentState.HasValue)
                    {
                        var seqFound = currentState.Value;
                        keywordsExtracted.Add(new TextSpan(text: seqFound, start: seqStartPos, end: sentLen));
                    }
                }

                idx += 1;
                if (resetCurrentDict)
                {
                    resetCurrentDict = false;
                    seqStartPos = idx;
                }
            }

            return keywordsExtracted;
        }

        public IEnumerable<string> ExtractKeywords(string sentence, bool raw = false)
        {
            if (raw)
            {
                return ExtractKeywordSpans(sentence).Select(span => sentence.Sub(span.Start, span.End));
            }
            
            return ExtractKeywordSpans(sentence).Select(span => span.Text);
        }

        #region Private methods

        private void SetItem(string keyword, string cleanName)
        {
            if (cleanName.IsEmpty() && keyword.IsNotEmpty())
            {
                cleanName = keyword;
            }

            if (keyword.IsNotEmpty() && cleanName.IsNotEmpty())
            {
                if (!CaseSensitive)
                {
                    keyword = keyword.ToLower();
                }

                KeywordTrie[keyword] = cleanName;
            }
        }
        
        private string GetItem(string word)
        {
            if (!CaseSensitive)
            {
                word = word.ToLower();
            }

            return KeywordTrie[word];
        }

        #endregion
    }
}