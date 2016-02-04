using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CommandLine;
using JiebaNet.Segmenter.PosSeg;

namespace JiebaNet.Segmenter.Cli
{
    class Options
    {
        [Option('h', "help")]
        public bool ShowHelp { get; set; }

        [Option('v', "version")]
        public bool ShowVersion { get; set; }

        [Option('f', "file", Required = true)]
        public string FileName { get; set; }

        [Option('d', "delimiter", DefaultValue = "/ ")]
        public string Delimiter { get; set; }

        [Option('p', "pos")]
        public bool POS { get; set; }

        //[Option('u', "user-dict")]
        //public string UserDict { get; set; }

        [Option('a', "cut-all")]
        public bool CutAll { get; set; }

        [Option('n', "no-hmm")]
        public bool NoHmm { get; set; }

        //[Option('q', "quiet")]
        //public bool Quiet { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            var usage = new StringBuilder();
            usage.AppendLine();
            usage.AppendLine("jieba.NET options and parameters: ");
            usage.AppendLine();
            usage.AppendLine("-f \t --file \t the file name, required.");
            usage.AppendLine();
            usage.AppendLine("-d \t --delimiter \t the delimiter between tokens, default: / .");
            usage.AppendLine("-a \t --cut-all \t use cut_all mode.");
            usage.AppendLine("-n \t --no-hmm \t don't use HMM.");
            usage.AppendLine("-p \t --pos \t enable POS tagging.");


            usage.AppendLine("-v \t --version \t show version info.");
            usage.AppendLine("-h \t --help \t show help details.");
            usage.AppendLine();
            usage.AppendLine("sample usages: ");
            usage.AppendLine("$ jiebanet -f input.txt > output.txt");
            usage.AppendLine("$ jiebanet -d | -f input.txt > output.txt");
            usage.AppendLine("$ jiebanet -p -f input.txt > output.txt");

            return usage.ToString();
        }
    }

    public class Program
    {
        static void Main(string[] args)
        {
            var options = new Options();
            if (Parser.Default.ParseArguments(args, options))
            {
                if (options.ShowHelp)
                {
                    Console.WriteLine(options.GetUsage());
                    return;
                }

                if (options.ShowVersion)
                {
                    Console.WriteLine("jieba.NET 0.38");
                    return;
                }

                if (string.IsNullOrWhiteSpace(options.FileName))
                {
                    Console.WriteLine("No file specified");
                    return;
                }

                SegmentFile(options);
            }
        }

        private static void SegmentFile(Options options)
        {
            var result = new List<string>();

            var fileName = Path.GetFullPath(options.FileName);
            var lines = File.ReadAllLines(fileName);

            Func<string, bool, bool, IEnumerable<string>> cutMethod = null;
            var segmenter = new JiebaSegmenter();
            if (options.POS)
            {
                cutMethod = (text, cutAll, hmm) =>
                {
                    var posSeg = new PosSegmenter(segmenter);
                    return posSeg.Cut(text, hmm).Select(token => string.Format("{0}/{1}", token.Word, token.Flag));
                };
            }
            else
            {
                cutMethod = segmenter.Cut;
            }

            var delimiter = string.IsNullOrWhiteSpace(options.Delimiter) ? "/ " : options.Delimiter;
            foreach (var line in lines)
            {
                result.Add(string.Join(delimiter, cutMethod(line, options.CutAll, options.NoHmm)));
            }
            Console.WriteLine(string.Join(Environment.NewLine, result));
        }
    }
}
