using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JiebaNet.Segmenter.Common;

namespace JiebaNet.Analyser
{
    public class KeywordProcessor
    {
        // java in javascript
        // c语言
        // 语法 tree
        private readonly string _keyword = "_keyword_";
        private readonly ISet<char> _whiteSpaceChars = new HashSet<char>(".\t\n\a ,");
        private readonly bool _caseSensitive;
        private readonly IDictionary<string, string> _keywordTrieDict = new Dictionary<string, string>();

        private readonly ISet<char> _nonWordBoundries =
            new HashSet<char>(
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_");

        private int _termsInTrie = 0;

        public KeywordProcessor(bool caseSensitive = false)
        {
            _caseSensitive = caseSensitive;
        }

        public int Length => this._termsInTrie;

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

        public string GetKeyword(string keyword, string cleanName = null)
        {
            return GetItem(keyword);
        }

        public bool Contains(string word)
        {
            return GetItem(word).IsNotNull();
        }

        public IEnumerable<string> ExtractKeywords(string sentence)
        {
            var keywords_extracted = new List<string>();
            if (sentence.IsEmpty())
            {
                return keywords_extracted;
            }

            if (!_caseSensitive)
            {
                sentence = sentence.ToLower();
            }

            var start = 0;
            var end = 0;
            var idx = 0;
            var idy = 0;
            var sent_len = sentence.Length;
            var reset_current_dict = false;

            while (idx < sent_len)
            {
                var ch = sentence[idx];
                var curSub = sentence.Sub(start, idx);
                // when reaching a char that denote word end
                if (!_nonWordBoundries.Contains(ch))
                {
                    // if current prefix is in trie
                    if (_keywordTrieDict.ContainsKey(curSub))
                    {
                        string seq_found = null;
                        string longest_found = null;
                        var is_longer_found = false;
                        
                        if (Contains(curSub))
                        {
                            seq_found = _keywordTrieDict[curSub];
                            longest_found = _keywordTrieDict[curSub];
                            end = idx;
                        }

                        // re look for longest seq from this position
                        if (_keywordTrieDict.ContainsKey(curSub))
                        {
                            idy = idx + 1;
                            while (idy < sent_len)
                            {
                                curSub = sentence.Sub(start, idy);
                                var inner_ch = sentence[idy];
                                if (!_nonWordBoundries.Contains(inner_ch) && Contains(curSub))
                                {
                                    longest_found = _keywordTrieDict[curSub];
                                    end = idy;
                                    is_longer_found = true;
                                }

                                curSub = sentence.Sub(start, idy + 1);
                                if(!_keywordTrieDict.ContainsKey(curSub))
                                {
                                    break;
                                }

                                idy += 1;
                            }

                            if (idy == sent_len && Contains(curSub))
                            {
                                longest_found = _keywordTrieDict[curSub];
                                end = idy;
                                is_longer_found = true;
                            }

                            if (is_longer_found)
                            {
                                idx = end;
                                start = idx;
                            }
                        }
                        
                        if (longest_found.IsNotEmpty())
                        {
                            keywords_extracted.Add(longest_found);
                        }

                        reset_current_dict = true;
                    }
                    else
                    {
                        reset_current_dict = true;
                    }
                }
                else if (_keywordTrieDict.ContainsKey(curSub))
                {
                    // in a word and in trie, just continue
                }
                else
                {
                    // in a word and not in trie, reset 
                    reset_current_dict = true;
                    
                    // skip to end of word
                    idy = idx + 1;
                    while (idy < sent_len && _nonWordBoundries.Contains(sentence[idy]))
                    {
                        idy += 1;
                    }

                    idx = idy;
                    
                    // idy = idx;
                    // while (idy < sent_len && _nonWordBoundries.Contains(sentence[idy]) && _keywordTrieDict.ContainsKey(sentence.Sub(start, idy + 1)))
                    // {
                    //     idy += 1;
                    // }
                    //
                    // Console.WriteLine(idy);
                    //
                    // if (idy == sent_len)
                    // {
                    //     if (Contains(sentence.Sub(start, idy))))
                    //     {
                    //         keywords_extracted.Add(sentence.Sub(start, idy));
                    //         //Console.WriteLine(sentence.Sub(start, idy));                            
                    //     }
                    // }
                    // else if (!_keywordTrieDict.ContainsKey(sentence.Sub(start, idy + 1)))
                    // {
                    //     
                    // }

                    // in a word and in trie, just continue
                }

                if (idx + 1 >= sent_len)
                {
                    curSub = sentence.Sub(start, idx);
                    if (Contains(curSub))
                    {
                        keywords_extracted.Add(_keywordTrieDict[curSub]);
                    }
                }

                idx += 1;
                if (reset_current_dict)
                {
                    reset_current_dict = false;
                    start = idx;
                }
            }

            return keywords_extracted;
        }

        #region Private methods

        // TODO: C# idioms
        private bool SetItem(string keyword, string cleanName)
        {
            var result = false;
            if (cleanName.IsEmpty() && keyword.IsNotEmpty())
            {
                cleanName = keyword;
            }

            if (keyword.IsNotEmpty() && cleanName.IsNotEmpty())
            {
                if (!_caseSensitive)
                {
                    keyword = keyword.ToLower();
                }

                var existing = GetItem(keyword);
                if (existing.IsNull())
                {
                    _keywordTrieDict[keyword] = cleanName;
                    for (var i = 0; i < keyword.Length; i++)
                    {
                        var wfrag = keyword.Substring(0, i + 1);
                        if (!_keywordTrieDict.ContainsKey(wfrag))
                        {
                            _keywordTrieDict[wfrag] = null;
                        }
                    }

                    result = true;
                    _termsInTrie += 1;
                }
            }

            return result;
        }

        private string GetItem(string word)
        {
            if (!_caseSensitive)
            {
                word = word.ToLower();
            }

            var result = _keywordTrieDict.GetDefault(word, null);
            return result;
        }

        #endregion
    }
}