using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CommandLine;

namespace JiebaNet.Segmenter.Cli
{
    class Options
    {
        [Option('h', "help")]
        public bool ShowHelp { get; set; }

        [Option('v', "version")]
        public bool ShowVersion { get; set; }

        [Option('f', "file")]
        public string FileName { get; set; }

        [Option('l', "delimiter", DefaultValue = "/")]
        public string Delimiter { get; set; }

        [Option('p', "pos")]
        public string POS { get; set; }

        [Option('D', "dict")]
        public string Dict { get; set; }

        [Option('u', "user-dict")]
        public string UserDict { get; set; }

        [Option('a', "cut-all")]
        public bool CutAll { get; set; }

        [Option('n', "no-hmm")]
        public bool NoHmm { get; set; }

        [Option('q', "quiet")]
        public bool Quiet { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            var usage = new StringBuilder();
            usage.AppendLine("jieba.NET command line tool.");
            usage.AppendLine("-f \t --file \t specify the file name.");
            usage.AppendLine("-v \t --version \t show version info.");
            usage.AppendLine("-h \t --help \t show help details.");

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
            //else
            //{
            //    Console.WriteLine(options.GetUsage());
            //}
        }

        private static void SegmentFile(Options options)
        {
            var result = new List<string>();

            var fileName = Path.GetFullPath(options.FileName);
            var lines = File.ReadAllLines(fileName);

            var segmenter = new JiebaSegmenter();
            var delimiter = string.IsNullOrWhiteSpace(options.Delimiter) ? "/" : options.Delimiter;
            foreach (var line in lines)
            {
                result.Add(string.Join(delimiter, segmenter.Cut(line)));
            }
            Console.WriteLine(string.Join(Environment.NewLine, result));
        }
    }
}
