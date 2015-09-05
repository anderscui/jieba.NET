using System;
using System.Collections.Generic;
using System.Linq;
using JiebaNet.Segmenter;
using JiebaNet.Segmenter.PosSeg;

namespace JiebaNet.Analyser
{
    //public 
    public class Tfidf
    {
        private static readonly string DefaultIdfFile = ConfigManager.IdfFile;

        private static readonly List<string> StopWords = new List<string>()
        {
            "the", "of", "is", "and", "to", "in", "that", "we", "for", "an", "are",
            "by", "be", "as", "on", "with", "can", "if", "from", "which", "you", "it",
            "this", "then", "at", "have", "all", "not", "one", "has", "or", "that"
        };

        public JiebaSegmenter Segmenter { get; set; }
        public PosSegmenter PosSegmenter { get; set; }
        public IdfLoader Loader { get; set; }

        internal IDictionary<string, double> IdfFreq { get; set; }
        internal double MedianIdf { get; set; }

        public Tfidf(string idfPath = null)
        {
            Segmenter = new JiebaSegmenter();
            PosSegmenter = new PosSegmenter(Segmenter);
            Loader = new IdfLoader();

            var idf = Loader.Idf;
            IdfFreq = idf.Item1;
            MedianIdf = idf.Item2;
        }

        public void SetIdfPath(string idfPath)
        {
            Loader.SetNewPath(idfPath);
            var idf = Loader.Idf;
            IdfFreq = idf.Item1;
            MedianIdf = idf.Item2;
        }

        public IEnumerable<string> ExtractTags(string text, int count = 10, bool withWeight = false, IEnumerable<string> allowPos = null)
        {
            IEnumerable<string> words = null;
            IEnumerable<Pair> wordTags = null;
            if (allowPos.IsNotEmpty())
            {
                wordTags = PosSegmenter.Cut(text);
            }
            else
            {
                words = Segmenter.Cut(text);
            }

            // Calculate TF
            var freq = new Dictionary<string, double>();
            foreach (var w in words)
            {
                if (string.IsNullOrEmpty(w) || w.Trim().Length < 2 || StopWords.Contains(w.ToLower()))
                {
                    continue;
                }
                freq[w] = freq.GetDefault(w, 0.0) + 1.0;
            }
            var total = freq.Values.Sum();
            foreach (var k in freq.Keys)
            {
                freq[k] *= IdfFreq.GetDefault(k, MedianIdf)/total;
            }

            if (count <= 0)
            {
                count = 10;
            }

            return freq.OrderByDescending(p => p.Value).Select(p => p.Key).Take(count);
        }
    }

    public class IdfLoader
    {
        internal string Path { get; set; }
        internal IDictionary<string, double> IdfFreq { get; set; }
        internal double MedianIdf { get; set; }

        public void SetNewPath(string newIdfPath)
        {
            // TODO:
        }

        public Tuple<IDictionary<string, double>, double> Idf
        {
            get { return new Tuple<IDictionary<string, double>, double>(IdfFreq, MedianIdf); }
        }
    }
}
