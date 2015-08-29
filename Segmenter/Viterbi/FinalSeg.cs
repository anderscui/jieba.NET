using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace JiebaNet.Segmenter.Viterbi
{
    public class FinalSeg
    {
        private static readonly Lazy<FinalSeg> lazy = new Lazy<FinalSeg>(() => new FinalSeg());
        private static char[] states = new char[] {'B', 'M', 'E', 'S'};
        private static IDictionary<char, IDictionary<char, Double>> emit;
        private static IDictionary<char, Double> start;
        private static IDictionary<char, IDictionary<char, Double>> trans;
        private static IDictionary<char, char[]> prevStatus;
        private static Double MIN_FLOAT = -3.14e100;

        private FinalSeg()
        {
            LoadModel();

            Console.WriteLine(emit.Count);
        }

        // TODO: synchronized
        public static FinalSeg Instance
        {
            get { return lazy.Value; }
        }

        private void LoadModel()
        {
            long s = DateTime.Now.Millisecond;
            prevStatus = new Dictionary<char, char[]>();
            prevStatus['B'] = new char[] {'E', 'S'};
            prevStatus['M'] = new char[] {'M', 'B'};
            prevStatus['S'] = new char[] {'S', 'E'};
            prevStatus['E'] = new char[] {'B', 'M'};

            start = new Dictionary<char, Double>();
            start['B'] = -0.26268660809250016;
            start['E'] = -3.14e+100;
            start['M'] = -3.14e+100;
            start['S'] = -1.4652633398537678;

            trans = new Dictionary<char, IDictionary<char, Double>>();
            IDictionary<char, Double> transB = new Dictionary<char, Double>();
            transB['E'] = -0.510825623765990;
            transB['M'] = -0.916290731874155;
            trans['B'] = transB;

            IDictionary<char, Double> transE = new Dictionary<char, Double>();
            transE['B'] = -0.5897149736854513;
            transE['S'] = -0.8085250474669937;
            trans['E'] = transE;

            IDictionary<char, Double> transM = new Dictionary<char, Double>();
            transM['E'] = -0.33344856811948514;
            transM['M'] = -1.2603623820268226;
            trans['M'] = transM;

            IDictionary<char, Double> transS = new Dictionary<char, Double>();
            transS['B'] = -0.7211965654669841;
            transS['S'] = -0.6658631448798212;
            trans['S'] = transS;

            var probEmitPath = ConfigManager.ProbEmitFile;
            emit = new Dictionary<char, IDictionary<char, double>>();

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
                        emit[tokens[0][0]] = values;
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

        public void cut(String sentence, List<String> tokens)
        {
            StringBuilder chinese = new StringBuilder();
            StringBuilder other = new StringBuilder();
            for (int i = 0; i < sentence.Length; ++i)
            {
                char ch = sentence[i];
                if (CharacterUtil.isChineseLetter(ch))
                {
                    if (other.Length > 0)
                    {
                        processOtherUnknownWords(other.ToString(), tokens);
                        other = new StringBuilder();
                    }
                    chinese.Append(ch);
                }
                else
                {
                    if (chinese.Length > 0)
                    {
                        viterbi(chinese.ToString(), tokens);
                        chinese = new StringBuilder();
                    }
                    other.Append(ch);
                }
            }
            if (chinese.Length > 0)
                viterbi(chinese.ToString(), tokens);
            else
            {
                processOtherUnknownWords(other.ToString(), tokens);
            }
        }

        public void viterbi(String sentence, List<String> tokens)
        {
            var v = new List<IDictionary<char, Double>>();
            IDictionary<char, Node> path = new Dictionary<char, Node>();

            v.Add(new Dictionary<char, Double>());
            foreach (var state in states)
            {
                var sd = emit[state];
                double emP = double.MinValue;
                if (sd.ContainsKey(sentence[0]))
                    emP = sd[sentence[0]];
                v[0][state] = start[state] + emP;
                path[state] = new Node(state, null);
            }

            for (int i = 1; i < sentence.Length; ++i)
            {
                IDictionary<char, Double> vv = new Dictionary<char, Double>();
                v.Add(vv);
                IDictionary<char, Node> newPath = new Dictionary<char, Node>();
                foreach (var y in states)
                {
                    var sd = emit[y];
                    double emp = double.MinValue;
                    if (sd.ContainsKey(sentence[i]))
                        emp = sd[sentence[i]];

                    Pair<char> candidate = null;
                    foreach (char y0 in prevStatus[y])
                    {
                        double tranp = double.MinValue;
                        if (trans[y0].ContainsKey(y))
                        {
                            tranp = trans[y0][y];
                        }
                        
                        tranp += (emp + v[i - 1][y0]);
                        if (null == candidate)
                        {
                            candidate = new Pair<char>(y0, tranp);
                        }
                        else if (candidate.freq <= tranp)
                        {
                            candidate.freq = tranp;
                            candidate.key = y0;
                        }
                    }
                    vv[y] = candidate.freq;
                    newPath[y] = new Node(y, path[candidate.key]);
                }
                path = newPath;
            }

            double probE = v[sentence.Length - 1]['E'];
            double probS = v[sentence.Length - 1]['S'];
            List<char> posList = new List<char>(sentence.Length);
            
            Node win;
            if (probE < probS)
                win = path['S'];
            else
                win = path['E'];

            while (win != null)
            {
                posList.Add(win.value);
                win = win.parent;
            }
            posList.Reverse();

            int begin = 0, next = 0;
            for (int i = 0; i < sentence.Length; ++i)
            {
                char pos = posList[i];
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
                tokens.Add(sentence.Substring(next));
        }

        private void processOtherUnknownWords(String other, List<String> tokens)
        {
            var mat = CharacterUtil.reSkip.Matches(other);
            int offset = 0;
            foreach (Match m in mat)
            {
                if (m.Index > offset)
                {
                    tokens.Add(other.Sub(offset, m.Index));
                }
                tokens.Add(m.Value);
                offset = m.Length;
            }
            if (offset < other.Length)
            {
                tokens.Add(other.Substring(offset));
            }
        }
    }
}