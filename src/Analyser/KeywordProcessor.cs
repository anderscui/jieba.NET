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
        // private readonly string _keyword = "_keyword_";
        private readonly ISet<char> _whiteSpaceChars = new HashSet<char>(".\t\n\a ,");
        private readonly bool _caseSensitive;
        private readonly KeywordTrie _keywordTrieDict = new KeywordTrie();

        private readonly ISet<char> _nonWordBoundries =
            new HashSet<char>(
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_");
        
        public KeywordProcessor(bool caseSensitive = false)
        {
            _caseSensitive = caseSensitive;
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

        public string GetKeyword(string keyword)
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

            KeywordTrieNode cur_state = _keywordTrieDict;
            var seq_start_pos = 0;
            var seq_end_pos = 0;
            var reset_current_dict = false;
            var idx = 0;
            var sent_len = sentence.Length;
            while (idx < sent_len)
            {
                var ch = sentence[idx];
                // when reaching a char that denote word end
                if (!_nonWordBoundries.Contains(ch))
                {
                    // if current prefix is in trie
                    if (cur_state.HasValue || cur_state.HasChild(ch))
                    {
                        string seq_found = null;
                        string longest_found = null;
                        var is_longer_found = false;
                        
                        if (cur_state.HasValue)
                        {
                            seq_found = cur_state.Value;
                            longest_found = cur_state.Value;
                            seq_end_pos = idx;
                        }

                        // re look for longest seq from this position
                        if (cur_state.HasChild(ch))
                        {
                            var cur_state_continued = cur_state.GetChild(ch);
                            var idy = idx + 1;
                            while (idy < sent_len)
                            {
                                var inner_ch = sentence[idy];
                                if (!_nonWordBoundries.Contains(inner_ch) && cur_state_continued.HasValue)
                                {
                                    longest_found = cur_state_continued.Value;
                                    seq_end_pos = idy;
                                    is_longer_found = true;
                                }

                                if(cur_state_continued.HasChild(inner_ch))
                                {
                                    cur_state_continued = cur_state_continued.GetChild(inner_ch);
                                }
                                else
                                {
                                    break;
                                }

                                idy += 1;
                            }

                            if (idy == sent_len && cur_state_continued.HasValue)
                            {
                                // end of sentence reached.
                                longest_found = cur_state_continued.Value;
                                seq_end_pos = idy;
                                is_longer_found = true;
                            }

                            if (is_longer_found)
                            {
                                idx = seq_end_pos;
                            }
                        }
                        
                        if (longest_found.IsNotEmpty())
                        {
                            keywords_extracted.Add(longest_found);
                        }

                        cur_state = _keywordTrieDict;
                        reset_current_dict = true;
                    }
                    else
                    {
                        cur_state = _keywordTrieDict;
                        reset_current_dict = true;
                    }
                }
                else if (cur_state.HasChild(ch))
                {
                    cur_state = cur_state.GetChild(ch);
                }
                else
                {
                    cur_state = _keywordTrieDict;
                    reset_current_dict = true;
                    
                    // skip to end of word
                    var idy = idx + 1;
                    while (idy < sent_len)
                    {
                        if (!_nonWordBoundries.Contains(sentence[idy]))
                        {
                            break;
                        }
                        idy += 1;
                    }

                    idx = idy;
                }

                if (idx + 1 >= sent_len)
                {
                    if (cur_state.HasValue)
                    {
                        var seq_found = cur_state.Value;
                        keywords_extracted.Add(seq_found);
                    }
                }

                idx += 1;
                if (reset_current_dict)
                {
                    reset_current_dict = false;
                    seq_start_pos = idx;
                }
            }

            return keywords_extracted;
        }

        #region Private methods

        // TODO: C# idioms
        private void SetItem(string keyword, string cleanName)
        {
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

                _keywordTrieDict[keyword] = cleanName;
            }
        }

        private string GetItem(string word)
        {
            if (!_caseSensitive)
            {
                word = word.ToLower();
            }

            return _keywordTrieDict[word];
        }

        #endregion
    }
}