using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace JiebaNet.Segmenter.Tests
{
    [TestFixture]
    public class TestSegmenter
    {
        private string[] GetTestSentences()
        {
            return File.ReadAllLines(@"Cases\jieba_test.txt");
        }

        [TestCase]
        public void TestGetDAG()
        {
            JiebaSegmenter seg = new JiebaSegmenter();
            var dag = seg.GetDAG("语言学家参加学术会议");
            foreach (var key in dag.Keys.ToList().OrderBy(k => k))
            {
                Console.Write("{0}: ", key);
                foreach (var i in dag[key])
                {
                    Console.Write("{0} ", i);
                }
                Console.WriteLine();
            }
        }

        [TestCase]
        public void TestCalc()
        {
            var s = "语言学家参加学术会议";
            var seg = new JiebaSegmenter();
            var dag = seg.GetDAG(s);
            var route = seg.Calc(s, dag);
            foreach (var key in route.Keys.ToList().OrderBy(k => k))
            {
                Console.Write("{0}: ", key);
                var pair = route[key];
                Console.WriteLine("({0}, {1})", pair.freq, pair.key);
            }
        }

        [TestCase]
        public void TestCutDAG()
        {
            var s = "语言学家去参加了那个学术会议";
            var seg = new JiebaSegmenter();
            var words = seg.CutDAG(s);
            foreach (var w in words)
            {
                Console.WriteLine(w);
            }
        }

        [TestCase]
        public void TestCutDAGWithoutHmm()
        {
            var s = "语言学家去参加了那个学术会议";
            var seg = new JiebaSegmenter();
            var words = seg.CutDAGWithoutHmm(s);
            foreach (var w in words)
            {
                Console.WriteLine(w);
            }
        }

        [TestCase]
        public void TestCut()
        {
            TestCutFunction((new JiebaSegmenter()).Cut, false, true, @"Cases\accurate_hmm.txt");
        }

        [TestCase]
        public void TestCutAll()
        {
            TestCutFunction((new JiebaSegmenter()).Cut, true, false, @"Cases\cut_all.txt");
        }

        [TestCase]
        public void TestCutWithoutHmm()
        {
            TestCutFunction((new JiebaSegmenter()).Cut, false, false, @"Cases\accurate_no_hmm.txt");
        }

        [TestCase]
        public void TestCutForSearch()
        {
            TestCutSearchFunction((new JiebaSegmenter()).CutForSearch, true, @"Cases\cut_search_hmm.txt");
        }

        [TestCase]
        public void TestCutForSearchWithoutHmm()
        {
            TestCutSearchFunction((new JiebaSegmenter()).CutForSearch, false, @"Cases\cut_search_no_hmm.txt");
        }

        [TestCase]
        public void TestTokenize()
        {
            var seg = new JiebaSegmenter();
            seg.AddWord("机器学习");
            seg.AddWord("自然语言处理");
            foreach (var token in seg.Tokenize("小明最近在学习机器学习、自然语言处理、云计算和大数据"))
            {
                Console.WriteLine(token);
            }

            foreach (var token in seg.Tokenize("小明最近在学习机器学习、自然语言处理、云计算和大数据", TokenizerMode.Search))
            {
                Console.WriteLine(token);
            }
        }

        private static void TestCutThenPrint(JiebaSegmenter segmenter, string s)
        {
            Console.WriteLine(string.Join("/ ", segmenter.Cut(s)));
        }

        [TestCase]
        public void TestAddWord()
        {
            var seg = new JiebaSegmenter();
            TestCutThenPrint(seg, "小明最近在学习机器学习和自然语言处理");
            seg.AddWord("机器学习");
            TestCutThenPrint(seg, "小明最近在学习机器学习和自然语言处理");
        }

        [TestCase]
        public void TestCutTraditionalChinese()
        {
            var seg = new JiebaSegmenter();
            TestCutThenPrint(seg, "小明最近在學習機器學習和自然語言處理");
            //seg.AddWord("机器学习");
            //TestCutThenPrint(seg, "小明最近在学习机器学习和自然语言处理");
        }

        [TestCase]
        public void TestUserDict()
        {
            var dict = @"Resources\user_dict.txt";
            var seg = new JiebaSegmenter();

            TestCutThenPrint(seg, "小明最近在学习机器学习、自然语言处理、云计算和大数据");
            seg.LoadUserDict(dict);
            TestCutThenPrint(seg, "小明最近在学习机器学习、自然语言处理、云计算和大数据");
        }

        [TestCase]
        public void TestSplit_Han_Default()
        {
            var s = "IBM是一家不错的公司，给你发offer了吗？";
            foreach (var part in JiebaSegmenter.RegexChineseDefault.Split(s))
            {
                Console.WriteLine(part);
            }

            foreach (var part in JiebaSegmenter.RegexChineseCutAll.Split(s))
            {
                Console.WriteLine(part);
            }
        }

        #region Private Helpers

        private void TestCutFunction(Func<string, bool, bool, IEnumerable<string>> method,
                                     bool cutAll, bool useHmm,
                                     string testResultFile)
        {
            var testCases = GetTestSentences();
            var testResults = File.ReadAllLines(testResultFile);
            Assert.That(testCases.Length, Is.EqualTo(testResults.Length));
            for (int i = 0; i < testCases.Length; i++)
            {
                var testCase = testCases[i];
                var testResult = testResults[i];
                Assert.That(method(testCase, cutAll, useHmm).Join("/ "), Is.EqualTo(testResult));
            }
        }

        private void TestCutSearchFunction(Func<string, bool, IEnumerable<string>> method,
                                     bool useHmm,
                                     string testResultFile)
        {
            var testCases = GetTestSentences();
            var testResults = File.ReadAllLines(testResultFile);
            Assert.That(testCases.Length, Is.EqualTo(testResults.Length));
            for (int i = 0; i < testCases.Length; i++)
            {
                var testCase = testCases[i];
                var testResult = testResults[i];
                Assert.That(method(testCase, useHmm).Join("/ "), Is.EqualTo(testResult));
            }
        }

        #endregion
    }
}