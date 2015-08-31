using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using NUnit.Framework;

namespace JiebaNet.Segmenter.Tests
{
    [TestFixture]
    public class TestSegmenter
    {
        private List<string> GetTestSentences()
        {
            return new List<string>()
            {
                "我需要廉租房",
                "据说这位语言学家去参加神马学术会议了",
                "小明硕士毕业于中国科学院计算所，后在日本京都大学深造",
                "他来到了网易杭研大厦",
                "他说的确实在理",
                "我购买了道具和服装，为了出演罗密欧与朱丽叶",
                "工信处女干事每月经过下属科室都要亲口交代24口交换机等技术性器件的安装工作",

                "我他我买演件了为不知所云",
                "小明不知何许人也，最近才学习机器学习知识",
                "通过结巴分词这个library，我们可以看到概率论在自然语言处理中的大量应用。"
            };
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
            var seg = new JiebaSegmenter();
            foreach (var sentence in GetTestSentences())
            {
                TestCutThenPrint(seg, sentence);
            }
        }

        [TestCase]
        public void TestCutAll()
        {
            var seg = new JiebaSegmenter();
            foreach (var sentence in GetTestSentences())
            {
                TestCutAllThenPrint(seg, sentence);
            }
        }

        [TestCase]
        public void TestCutWithoutHmm()
        {
            var seg = new JiebaSegmenter();
            foreach (var sentence in GetTestSentences())
            {
                TestCutWithoutHmm(seg, sentence);
            }
        }

        [TestCase]
        public void TestCutForSearch()
        {
            var seg = new JiebaSegmenter();
            foreach (var sentence in GetTestSentences())
            {
                TestCutForSearch(seg, sentence);
            }
        }

        private static void TestCutThenPrint(JiebaSegmenter segmenter, string s)
        {
            Console.WriteLine(string.Join("/ ", segmenter.Cut(s)));
        }

        private static void TestCutAllThenPrint(JiebaSegmenter segmenter, string s)
        {
            Console.WriteLine(string.Join("/ ", segmenter.Cut(s, true)));
        }

        private static void TestCutWithoutHmm(JiebaSegmenter segmenter, string s)
        {
            Console.WriteLine(string.Join("/ ", segmenter.Cut(s, hmm: false)));
        }

        private static void TestCutForSearch(JiebaSegmenter segmenter, string s)
        {
            Console.WriteLine(string.Join("/ ", segmenter.CutForSearch(s)));
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
        public void TestSuggestFreq()
        {
            var seg = new JiebaSegmenter();
            TestCutThenPrint(seg, "小明最近在学习机器学习、自然语言处理、云计算和大数据");
            seg.AddWord("机器学习");
            seg.SuggestFreq("自然语言处理", true);
            seg.AddWord("云计算");
            seg.SuggestFreq("大数据", true);
            TestCutThenPrint(seg, "小明最近在学习机器学习、自然语言处理、云计算和大数据");
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
    }
}