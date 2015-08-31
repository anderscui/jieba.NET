using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace JiebaNet.Segmenter
{
    public class WordDictionary
    {
        private static readonly Lazy<WordDictionary> lazy = new Lazy<WordDictionary>(() => new WordDictionary());
        private static readonly string MainDict = ConfigManager.MainDictFile;

        public IDictionary<string, int> Trie = new Dictionary<string, int>();
        public ISet<string> LoadedPath = new HashSet<string>();

        /// <summary>
        /// total occurrence of all words.
        /// </summary>
        public double Total { get; set; }

        private static readonly object locker = new object();

        private WordDictionary()
        {
            LoadDict();

            Console.WriteLine("{0} words", Trie.Count);
            Console.WriteLine("total freq: {0}", Total);
        }

        public static WordDictionary Instance
        {
            get { return lazy.Value; }
        }

        // TODO: synchronized
        /// <summary>
        /// Loads user dictionaries.
        /// </summary>
        /// <param name="userDictFile"></param>
        public void LoadUserDict(string userDictFile)
        {
            var dictFullPath = Path.GetFullPath(userDictFile);
            Console.WriteLine("Initializing user dictionary: " + userDictFile);

            lock(locker)
            {
                if (LoadedPath.Contains(dictFullPath))
                    return;

                try
                {
                    var startTime = DateTime.Now.Millisecond;

                    var lines = File.ReadAllLines(dictFullPath, Encoding.UTF8);
                    foreach (var line in lines)
                    {
                        if (string.IsNullOrWhiteSpace(line))
                        {
                            continue;
                        }

                        var tokens = line.Trim().Split('\t', ' ');
                        var word = tokens[0];
                        // TODO: calc freq;
                        var freq = 5;
                        var tag = string.Empty;
                        if (tokens.Length == 2)
                        {
                            if (tokens[1].IsInt32())
                            {
                                freq = int.Parse(tokens[1]);
                            }
                            else
                            {
                                tag = tokens[1];
                            }
                        }
                        else if (tokens.Length > 2)
                        {
                            freq = int.Parse(tokens[1]);
                            tag = tokens[2];
                        }
                        
                        AddWord(word, freq, tag);
                    }

                    Console.WriteLine("user dict '{0}' load finished, time elapsed {1} ms", 
                        dictFullPath, DateTime.Now.Millisecond - startTime);
                }
                catch (IOException e)
                {
                    Console.Error.WriteLine("'{0}' load failure, reason: {1}", dictFullPath, e.Message);
                }
                catch (FormatException fe)
                {
                    Console.Error.WriteLine(fe.Message);
                }
            }
        }

        private void LoadDict()
        {
            try
            {
                var startTime = DateTime.Now.Millisecond;

                var lines = File.ReadAllLines(MainDict, Encoding.UTF8);
                foreach (var line in lines)
                {
                    var tokens = line.Split('\t', ' ');
                    if (tokens.Length < 2)
                    {
                        Console.Error.WriteLine("Invalid line: {0}", line);
                        continue;
                    }

                    var word = tokens[0];
                    var freq = int.Parse(tokens[1]);

                    Trie[word] = freq;
                    Total += freq;

                    foreach (var ch in Enumerable.Range(0, word.Length))
                    {
                        var wfrag = word.Sub(0, ch + 1);
                        if (!Trie.ContainsKey(wfrag))
                        {
                            Trie[wfrag] = 0;
                        }
                    }
                }

                Console.WriteLine("main dict load finished, time elapsed {0} ms", DateTime.Now.Millisecond - startTime);
            }
            catch (IOException e)
            {
                Console.Error.WriteLine("{0} load failure, reason: {1}", MainDict, e.Message);
            }
            catch (FormatException fe)
            {
                Console.Error.WriteLine(fe.Message);
            }
        }

        public bool ContainsWord(string word)
        {
            return Trie.ContainsKey(word) && Trie[word] > 0;
        }

        public int GetFreqOrDefault(string key)
        {
            if (ContainsWord(key))
                return Trie[key];
            else
                return 1;
        }

        public void AddWord(string word, int freq = 0, string tag = null)
        {
            if (ContainsWord(word))
            {
                return;
            }

            Trie[word] = freq;
            Total += freq;
            for (var i = 0; i < word.Length; i++)
            {
                var wfrag = word.Substring(0, i + 1);
                if (!Trie.ContainsKey(wfrag))
                {
                    Trie[wfrag] = 0;
                }
            }
        }

        public void DeleteWord(string word)
        {
            AddWord(word, 0);
        }

        public int SuggestFreq(string word, IEnumerable<string> segments, bool tune = false)
        {
            double freq = 1;
            foreach (var seg in segments)
            {
                freq *= GetFreqOrDefault(seg) / Total;
            }

            return Math.Max((int)(freq * Total) + 1, GetFreqOrDefault(word));
        }
    }
}