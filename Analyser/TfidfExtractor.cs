using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using JiebaNet.Segmenter;
using JiebaNet.Segmenter.PosSeg;

namespace JiebaNet.Analyser
{
    public abstract class KeywordExtractor
    {
        protected static readonly List<string> DefaultStopWords = new List<string>()
        {
            "the", "of", "is", "and", "to", "in", "that", "we", "for", "an", "are",
            "by", "be", "as", "on", "with", "can", "if", "from", "which", "you", "it",
            "this", "then", "at", "have", "all", "not", "one", "has", "or", "that"
        };

        protected virtual ISet<string> StopWords { get; set; }

        public void SetStopWords(string stopWordsFile)
        {
            var path = Path.GetFullPath(stopWordsFile);
            if (File.Exists(path))
            {
                var lines = File.ReadAllLines(path);
                StopWords = new HashSet<string>();
                foreach (var line in lines)
                {
                    StopWords.Add(line.Trim());
                }
            }
        }

        public abstract IEnumerable<string> ExtractTags(string text, int count = 20, IEnumerable<string> allowPos = null);
        public abstract IEnumerable<Tuple<string, double>> ExtractTagsWithWeight(string text, int count = 10, IEnumerable<string> allowPos = null);
    }

    public class TfidfExtractor : KeywordExtractor
    {
        private static readonly string DefaultIdfFile = ConfigManager.IdfFile;

        private JiebaSegmenter Segmenter { get; set; }
        private PosSegmenter PosSegmenter { get; set; }
        private IdfLoader Loader { get; set; }

        private IDictionary<string, double> IdfFreq { get; set; }
        private double MedianIdf { get; set; }

        public TfidfExtractor()
        {
            Segmenter = new JiebaSegmenter();
            PosSegmenter = new PosSegmenter(Segmenter);
            SetStopWords(ConfigManager.StopWordsFile);
            if (StopWords.IsEmpty())
            {
                StopWords.UnionWith(DefaultStopWords);
            }

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

        public override IEnumerable<string> ExtractTags(string text, int count = 20, IEnumerable<string> allowPos = null)
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

        public override IEnumerable<Tuple<string, double>> ExtractTagsWithWeight(string text, int count = 10, IEnumerable<string> allowPos = null)
        {
            throw new NotImplementedException();
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

                MedianIdf = IdfFreq.Values.OrderBy(v => v).ToList()[IdfFreq.Count / 2];
            }
        }
    }
}
