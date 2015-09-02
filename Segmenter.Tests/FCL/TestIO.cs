using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace Segmenter.Tests.FCL
{
    [TestFixture]
    public class TestIO
    {
        private static readonly string ProbFilePath = "../../prob.txt";

        [TestCase]
        public void TestReadFile()
        {
            var lines = File.ReadAllLines(ProbFilePath, Encoding.UTF8);

            IDictionary<char, IDictionary<char, double>> emit = new Dictionary<char, IDictionary<char, double>>();
            IDictionary<char, double> values = null;
            foreach (var line in lines)
            {
                var tokens = line.Split('\t');
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

            Console.WriteLine(emit.Count);
            Console.WriteLine(values.Count);
        }

        [TestCase]
        public void TestNormalizePath()
        {
            var p = @"..\test.txt";
            Console.WriteLine(Path.IsPathRooted(p));
            Console.WriteLine(Path.GetFullPath(p));

            p = @"C:\test.txt";
            Console.WriteLine(Path.IsPathRooted(p));
            Console.WriteLine(Path.GetFullPath(p));
        }
    }
}