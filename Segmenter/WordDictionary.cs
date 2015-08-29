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

        private static readonly string MAIN_DICT = ConfigManager.MainDictFile;
        private static string USER_DICT_SUFFIX = ".dict";

        public IDictionary<string, int> Trie = new Dictionary<string, int>();
        public ISet<string> loadedPath = new HashSet<string>();
        private double minFreq = double.MaxValue;

        /// <summary>
        /// total occurrence of all words.
        /// </summary>
        public double Total { get; set; }

        private DictSegment _dict;

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
        /// <param name="configFile"></param>
        private void init(string configFile)
        {
            // TODO: normalize/configurable path
            string abspath = configFile;
            Console.WriteLine("initialize user dictionary: " + abspath);

            lock(locker)
            {
                if (loadedPath.Contains(abspath))
                    return;

                try
                {
                    var dictFiles = Directory.GetFiles(abspath, "*" + USER_DICT_SUFFIX);
                    foreach (var dictFile in dictFiles)
                    {
                        //_instance.loadUserDict(dictFile);
                    }
                    loadedPath.Add(abspath);
                }
                catch (IOException e)
                {
                    Console.Error.WriteLine("{0}: load user dict failure!", configFile);
                }
            }
        }

        private void LoadDict()
        {
            try
            {
                var startTime = DateTime.Now.Millisecond;

                var lines = File.ReadAllLines(MAIN_DICT, Encoding.UTF8);
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
                Console.Error.WriteLine("{0} load failure, reason: {1}", MAIN_DICT, e.Message);
            }
            catch (FormatException fe)
            {
                Console.Error.WriteLine(fe.Message);
            }
        }

        //public void loadUserDict(string userDict)
        //{
        //    loadUserDict(userDict, Encoding.UTF8);
        //}

        //public void loadUserDict(string userDict, Encoding charset)
        //{
        //    try
        //    {
        //        var lines = File.ReadAllLines(userDict, charset);
        //        long s = DateTime.Now.Millisecond;
        //        int count = 0;
        //        foreach (var line in lines)
        //        {
        //            string[] tokens = line.Split("\t ".ToCharArray());

        //            if (tokens.Length < 2)
        //                continue;

        //            string word = tokens[0];
        //            double freq = double.Parse(tokens[1]);
        //            word = addWord(word);
        //            Freq[word] = Math.Log(freq/Total);
        //            count++;
        //        }
        //        Console.WriteLine("user dict {0} load finished, total words :{1}, time elapsed: {2} ms",
        //            userDict, count, DateTime.Now.Millisecond - s);
        //    }
        //    catch (IOException e)
        //    {
        //        Console.Error.WriteLine("{0}: load user dict failure!", userDict);
        //    }
        //}

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
            
        }

        public int SuggestFreq(string segment, bool tune = false)
        {
            var freq = 1;
            return 0;
        }
    }
}