using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace JiebaNet.Segmenter.FinalSeg
{
    public class Viterbi : IFinalSeg
    {
        private static readonly Lazy<Viterbi> Lazy = new Lazy<Viterbi>(() => new Viterbi());
        private static readonly char[] States = {'B', 'M', 'E', 'S'};

        private static readonly Regex RegexChinese = new Regex(@"([\u4E00-\u9FA5]+)", RegexOptions.Compiled);
        private static readonly Regex RegexSkip = new Regex(@"(\d+\.\d+|[a-zA-Z0-9]+)", RegexOptions.Compiled);

        private static IDictionary<char, IDictionary<char, Double>> _emitProbs;
        private static IDictionary<char, Double> _startProbs;
        private static IDictionary<char, IDictionary<char, Double>> _transProbs;
        private static IDictionary<char, char[]> _prevStatus;

        private Viterbi()
        {
            LoadModel();
        }

        // TODO: synchronized
        public static Viterbi Instance
        {
            get { return Lazy.Value; }
        }

        public IEnumerable<string> Cut(String sentence)
        {
            var tokens = new List<string>();
            foreach (var blk in RegexChinese.Split(sentence))
            {
                if (RegexChinese.IsMatch(blk))
                {
                    tokens.AddRange(ViterbiCut(blk));
                }
                else
                {
                    var segments = RegexSkip.Split(blk).Where(seg => !string.IsNullOrEmpty(seg));
                    tokens.AddRange(segments);
                }
            }
            return tokens;
        }

        #region Private Helpers
        
        private void LoadModel()
        {
            long s = DateTime.Now.Millisecond;
            _prevStatus = new Dictionary<char, char[]>();
            _prevStatus['B'] = new char[] {'E', 'S'};
            _prevStatus['M'] = new char[] {'M', 'B'};
            _prevStatus['S'] = new char[] {'S', 'E'};
            _prevStatus['E'] = new char[] {'B', 'M'};

            _startProbs = new Dictionary<char, Double>();
            _startProbs['B'] = -0.26268660809250016;
            _startProbs['E'] = -3.14e+100;
            _startProbs['M'] = -3.14e+100;
            _startProbs['S'] = -1.4652633398537678;

            _transProbs = new Dictionary<char, IDictionary<char, Double>>();
            IDictionary<char, Double> transB = new Dictionary<char, Double>();
            transB['E'] = -0.510825623765990;
            transB['M'] = -0.916290731874155;
            _transProbs['B'] = transB;

            IDictionary<char, Double> transE = new Dictionary<char, Double>();
            transE['B'] = -0.5897149736854513;
            transE['S'] = -0.8085250474669937;
            _transProbs['E'] = transE;

            IDictionary<char, Double> transM = new Dictionary<char, Double>();
            transM['E'] = -0.33344856811948514;
            transM['M'] = -1.2603623820268226;
            _transProbs['M'] = transM;

            IDictionary<char, Double> transS = new Dictionary<char, Double>();
            transS['B'] = -0.7211965654669841;
            transS['S'] = -0.6658631448798212;
            _transProbs['S'] = transS;

            var probEmitPath = ConfigManager.ProbEmitFile;
            _emitProbs = new Dictionary<char, IDictionary<char, double>>();

            try
            {
                var lines = File.ReadAllLines(probEmitPath, Encoding.UTF8);

                IDictionary<char, double> values = null;
                foreach (var line in lines)
                {
                    var tokens = line.Split('\t');
                    // If a new state starts.
                    if (tokens.Length == 1)
                    {
                        values = new Dictionary<char, double>();
                        _emitProbs[tokens[0][0]] = values;
                    }
                    else
                    {
                        values[tokens[0][0]] = double.Parse(tokens[1]);
                    }
                }
            }
            catch (IOException ex)
            {
                Console.Error.WriteLine("{0}: loading model from file {1} failure!", ex.Message, probEmitPath);
            }

            Console.WriteLine("model loading finished, time elapsed {0} ms.", DateTime.Now.Millisecond - s);
        }

        private IEnumerable<string> ViterbiCut(string sentence)
        {
            var v = new List<IDictionary<char, Double>>();
            IDictionary<char, Node> path = new Dictionary<char, Node>();

            // Init weights and paths.
            v.Add(new Dictionary<char, Double>());
            foreach (var state in States)
            {
                var emP = _emitProbs[state].GetDefault(sentence[0], Constants.MinProb);
                v[0][state] = _startProbs[state] + emP;
                path[state] = new Node(state, null);
            }

            // For each remaining char
            for (var i = 1; i < sentence.Length; ++i)
            {
                IDictionary<char, Double> vv = new Dictionary<char, Double>();
                v.Add(vv);
                IDictionary<char, Node> newPath = new Dictionary<char, Node>();
                foreach (var y in States)
                {
                    var emp = _emitProbs[y].GetDefault(sentence[i], Constants.MinProb);

                    Pair<char> candidate = new Pair<char>('\0', Constants.MinProb);
                    foreach (var y0 in _prevStatus[y])
                    {
                        var tranp = _transProbs[y0].GetDefault(y, Constants.MinProb);
                        tranp = v[i - 1][y0] + tranp + emp;
                        if (candidate.Freq <= tranp)
                        {
                            candidate.Freq = tranp;
                            candidate.Key = y0;
                        }
                    }
                    vv[y] = candidate.Freq;
                    newPath[y] = new Node(y, path[candidate.Key]);
                }
                path = newPath;
            }

            var probE = v[sentence.Length - 1]['E'];
            var probS = v[sentence.Length - 1]['S'];
            var finalPath = probE < probS ? path['S'] : path['E'];

            var posList = new List<char>(sentence.Length);
            while (finalPath != null)
            {
                posList.Add(finalPath.Value);
                finalPath = finalPath.Parent;
            }
            posList.Reverse();

            var tokens = new List<string>();
            int begin = 0, next = 0;
            for (var i = 0; i < sentence.Length; i++)
            {
                var pos = posList[i];
                if (pos == 'B')
                    begin = i;
                else if (pos == 'E')
                {
                    tokens.Add(sentence.Sub(begin, i + 1));
                    next = i + 1;
                }
                else if (pos == 'S')
                {
                    tokens.Add(sentence.Sub(i, i + 1));
                    next = i + 1;
                }
            }
            if (next < sentence.Length)
            {
                tokens.Add(sentence.Substring(next));
            }

            return tokens;
        }

        #endregion
    }
}