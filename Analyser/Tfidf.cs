using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using JiebaNet.Segmenter;
using JiebaNet.Segmenter.PosSeg;

namespace JiebaNet.Analyser
{
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

        public Tfidf()
        {
            Segmenter = new JiebaSegmenter();
            PosSegmenter = new PosSegmenter(Segmenter);
            Loader = new IdfLoader(DefaultIdfFile);

            IdfFreq = Loader.IdfFreq;
            MedianIdf = Loader.MedianIdf;
        }

        public void SetIdfPath(string idfPath)
        {
            Loader.SetNewPath(idfPath);
            IdfFreq = Loader.IdfFreq;
            MedianIdf = Loader.MedianIdf;
        }

        // TODO:
        public IEnumerable<string> FilterCutByPos(string text, IEnumerable<string> allowPos)
        {
            return Segmenter.Cut(text);
        } 

        public IEnumerable<string> ExtractTags(string text, int count = 10, bool withWeight = false, IEnumerable<string> allowPos = null)
        {
            IEnumerable<string> words = null;
            if (allowPos.IsNotEmpty())
            {
                words = FilterCutByPos(text, allowPos);
            }
            else
            {
                words = Segmenter.Cut(text);
            }

            // Calculate TF
            var freq = new Dictionary<string, double>();
            foreach (var word in words)
            {
                var w = word;
                if (string.IsNullOrEmpty(w) || w.Trim().Length < 2 || StopWords.Contains(w.ToLower()))
                {
                    continue;
                }
                freq[w] = freq.GetDefault(w, 0.0) + 1.0;
            }
            var total = freq.Values.Sum();
            foreach (var k in freq.Keys.ToList())
            {
                freq[k] *= IdfFreq.GetDefault(k, MedianIdf) / total;
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
        internal string IdfFilePath { get; set; }
        internal IDictionary<string, double> IdfFreq { get; set; }
        internal double MedianIdf { get; set; }

        public IdfLoader(string idfPath = null)
        {
            IdfFilePath = string.Empty;
            IdfFreq = new Dictionary<string, double>();
            MedianIdf = 0.0;
            if (!string.IsNullOrWhiteSpace(idfPath))
            {
                SetNewPath(idfPath);
            }
        }

        public void SetNewPath(string newIdfPath)
        {
            var idfPath = Path.GetFullPath(newIdfPath);
            if (IdfFilePath != idfPath)
            {
                IdfFilePath = idfPath;
                var lines = File.ReadAllLines(idfPath, Encoding.UTF8);
                IdfFreq = new Dictionary<string, double>();
                foreach (var line in lines)
                {
                    var parts = line.Trim().Split(' ');
                    var word = parts[0];
                    var freq = double.Parse(parts[1]);
                    IdfFreq[word] = freq;
                }

                MedianIdf = IdfFreq.Values.OrderBy(v => v).ToList()[IdfFreq.Count/2];
            }
        }
    }
}
