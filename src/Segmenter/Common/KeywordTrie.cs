using System;
using System.Collections.Generic;
using System.Linq;

namespace JiebaNet.Segmenter.Common
{
    public class KeywordTrieNode
    {
        private IDictionary<char, KeywordTrieNode> _children;
        // private string _value;
        
        public KeywordTrieNode(string value = null)
        {
            _children = new Dictionary<char, KeywordTrieNode>();
            Value = value;
        }

        public string Value { get; set; }

        public bool HasValue => Value.IsNotNull();

        public KeywordTrieNode AddChild(char ch, string value = null, bool overwrite = false)
        {
            var child = _children.GetOrDefault(ch);
            if (child.IsNull())
            {
                child = new KeywordTrieNode(value);
                _children[ch] = child;
            }
            else if (overwrite)
            {
                child.Value = value;
            }

            return child;
        }
        
        public KeywordTrieNode GetChild(char ch)
        {
            var child = _children.GetOrDefault(ch);
            return child;
        }

        public bool HasChild(char ch)
        {
            return _children.ContainsKey(ch);
        }
    }

    public class KeywordTrie: KeywordTrieNode
    {
        public KeywordTrie()
        {
            Count = 0;
        }
        
        public int Count { get; set; }
        
        public bool Contains(string key)
        {
            return GetItem(key).IsNotNull();
        }

        public void Remove(string key)
        {
            // TODO: impl and count
            this[key] = null;
        }

        public string this[string key]
        {
            get { return GetItem(key); }
            set { SetItem(key, value); }
        }

        #region Private Methods

        private string GetItem(string key)
        {
            KeywordTrieNode state = this;
            foreach (var ch in key)
            {
                state = state.GetChild(ch);
                if (state.IsNull())
                {
                    return null;
                }
            }

            return state.Value;
        }

        private void SetItem(string key, string value)
        {
            KeywordTrieNode state = this;
            for (int i = 0; i < key.Length; i++)
            {
                if (i < key.Length - 1)
                {
                    state = state.AddChild(key[i]);
                }
                else
                {
                    var child = state.GetChild(key[i]);
                    state = state.AddChild(key[i], value, true);
                    if (child.IsNull() || !child.HasValue)
                    {
                        Count += 1;
                    }
                }
            }
        }

        #endregion
    }
}
