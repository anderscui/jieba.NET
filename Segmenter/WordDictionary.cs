using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JiebaNet.Segmenter
{
    public class WordDictionary
    {
        private static WordDictionary singleton;
        private static readonly string MAIN_DICT = "/dict.txt";
        private static string USER_DICT_SUFFIX = ".dict";

        // TODO: 2 final fields
        public  IDictionary<string, double> freqs = new Dictionary<string, double>();
        public ISet<string> loadedPath = new HashSet<string>();
        private double minFreq = double.MaxValue;
        private double total = 0.0;
        private DictSegment _dict;

        private static readonly object locker = new object();

        private WordDictionary()
        {
            this.loadDict();
        }

        // TODO: synchronized
        public static WordDictionary getInstance()
        {
            if (singleton == null)
            {
                lock(locker)
                {
                    if (singleton == null)
                    {
                        singleton = new WordDictionary();
                        return singleton;
                    }
                }
            }
            return singleton;
        }

        // TODO: synchronized
        /**
         * for ES to initialize the user dictionary.
         * 
         * @param configFile
         */
        public void init(string configFile)
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
                        singleton.loadUserDict(dictFile);
                    }
                    loadedPath.Add(abspath);
                }
                catch (IOException e)
                {
                    // TODO: Auto-generated catch block
                    // e.printStackTrace();
                    Console.Error.WriteLine("{0}: load user dict failure!", configFile);
                }
            }
        }

        public void loadDict()
        {
            _dict = new DictSegment((char)0);
            
            try
            {
                var lines = File.ReadAllLines(MAIN_DICT, Encoding.UTF8);

                long s = DateTime.Now.Millisecond;
                foreach (var line in lines)
                {
                    string[] tokens = line.Split("[\t ]+".ToCharArray());
                    if (tokens.Length < 2)
                        continue;

                    string word = tokens[0];
                    double freq = double.Parse(tokens[1]);
                    total += freq;
                    word = addWord(word);
                    freqs[word] = freq;
                }

                // normalize
                foreach (var freq in freqs)
                {
                    freqs[freq.Key] = Math.Log(freq.Value/total);
                    minFreq = Math.Min(freq.Value, minFreq);
                }

                Console.WriteLine("main dict load finished, time elapsed {0} ms", DateTime.Now.Millisecond - s);
            }
            catch (IOException e)
            {
                Console.Error.WriteLine("{0} load failure!", MAIN_DICT);
            }
        }

        private string addWord(string word)
        {
            if (!string.IsNullOrWhiteSpace(word))
            {
                string key = word.Trim().ToLower();
                _dict.fillSegment(key.ToCharArray());
                return key;
            }
            else
                return null;
        }

        public void loadUserDict(string userDict)
        {
            loadUserDict(userDict, Encoding.UTF8);
        }

        public void loadUserDict(string userDict, Encoding charset)
        {
            try
            {
                var lines = File.ReadAllLines(userDict, charset);
                long s = DateTime.Now.Millisecond;
                int count = 0;
                foreach (var line in lines)
                {
                    string[] tokens = line.Split("[\t ]+".ToCharArray());

                    if (tokens.Length < 2)
                        continue;

                    string word = tokens[0];
                    double freq = double.Parse(tokens[1]);
                    word = addWord(word);
                    freqs[word] = Math.Log(freq/total);
                    count++;
                }
                Console.WriteLine("user dict {0} load finished, total words :{1}, time elapsed: {2} ms",
                    userDict, count, DateTime.Now.Millisecond - s);
            }
            catch (IOException e)
            {
                Console.Error.WriteLine("{0}: load user dict failure!", userDict);
            }
        }

        public DictSegment getTrie()
        {
            return this._dict;
        }

        public bool containsWord(string word)
        {
            return freqs.ContainsKey(word);
        }

        public double getFreq(string key)
        {
            if (containsWord(key))
                return freqs[key];
            else
                return minFreq;
        }
    }
}