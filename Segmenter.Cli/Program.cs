using System;
using System.Collections.Generic;
using System.IO;

namespace JiebaNet.Segmenter.Cli
{
    public class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("No file specified");
                return;
            }

            var result = new List<string>();

            var filename = Path.GetFullPath(args[0]);
            var lines = File.ReadAllLines(filename);

            var segmenter = new JiebaSegmenter();
            foreach (var line in lines)
            {
                result.Add(string.Join("/ ", segmenter.Cut(line)));
            }
            Console.WriteLine(string.Join(Environment.NewLine, result));
        }
    }
}
