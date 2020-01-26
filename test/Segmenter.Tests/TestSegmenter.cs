using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using JiebaNet.Segmenter.Common;
using NUnit.Framework;

namespace JiebaNet.Segmenter.Tests
{
    [TestFixture]
    public class TestSegmenter
    {
        private string[] GetTestSentences()
        {
            return File.ReadAllLines(TestHelper.GetCaseFilePath("jieba_test.txt"));
        }

        [TestCase]
        public void TestGetDag()
        {
            var seg = new JiebaSegmenter();
            var dag = seg.GetDag("语言学家参加学术会议");
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
            var dag = seg.GetDag(s);
            var route = seg.Calc(s, dag);
            foreach (var key in route.Keys.ToList().OrderBy(k => k))
            {
                Console.Write("{0}: ", key);
                var pair = route[key];
                Console.WriteLine("({0}, {1})", pair.Freq, pair.Key);
            }
        }

        [TestCase]
        public void TestCutDag()
        {
            var s = "语言学家去参加了那个学术会议";
            var seg = new JiebaSegmenter();
            var words = seg.CutDag(s);
            foreach (var w in words)
            {
                Console.WriteLine(w);
            }
        }

        [TestCase]
        public void TestCutDagWithoutHmm()
        {
            var s = "语言学家去参加了那个学术会议";
            var seg = new JiebaSegmenter();
            var words = seg.CutDagWithoutHmm(s);
            foreach (var w in words)
            {
                Console.WriteLine(w);
            }
        }

        #region Jieba Python Test Cases

        [TestCase]
        public void TestCut()
        {
            TestCutFunction((new JiebaSegmenter()).Cut, false, true, TestHelper.GetCaseFilePath("accurate_hmm.txt"));
        }

        [TestCase]
        public void TestCutAll()
        {
            TestCutFunction((new JiebaSegmenter()).Cut, true, false, TestHelper.GetCaseFilePath("cut_all.txt"));
        }

        [TestCase]
        public void TestCutWithoutHmm()
        {
            TestCutFunction((new JiebaSegmenter()).Cut, false, false, TestHelper.GetCaseFilePath("accurate_no_hmm.txt"));
        }

        [TestCase]
        public void TestCutForSearch()
        {
            TestCutSearchFunction((new JiebaSegmenter()).CutForSearch, true, TestHelper.GetCaseFilePath("cut_search_hmm.txt"));
        }

        [TestCase]
        public void TestCutForSearchWithoutHmm()
        {
            TestCutSearchFunction((new JiebaSegmenter()).CutForSearch, false, TestHelper.GetCaseFilePath("cut_search_no_hmm.txt"));
        }

        #endregion

        [TestCase]
        public void TestTokenize()
        {
            var seg = new JiebaSegmenter();
            foreach (var token in seg.Tokenize("小明最近在学习机器学习、自然语言处理、云计算和大数据"))
            {
                Console.WriteLine(token);
            }
            Console.WriteLine();

            foreach (var token in seg.Tokenize("小明最近在学习机器学习、自然语言处理、云计算和大数据", TokenizerMode.Search))
            {
                Console.WriteLine(token);
            }
        }

        [TestCase]
        public void TestTokenizeWithSpace()
        {
            var seg = new JiebaSegmenter();

            var s = "永和服装饰品有限公司";
            var tokens = seg.Tokenize(s).ToList();
            Assert.That(tokens.Count, Is.EqualTo(4));
            Assert.That(tokens.Last().EndIndex, Is.EqualTo(s.Length));

            s = "永和服装饰品 有限公司";
            tokens = seg.Tokenize(s).ToList();
            Assert.That(tokens.Count, Is.EqualTo(5));
            Assert.That(tokens.Last().EndIndex, Is.EqualTo(s.Length));
        }

        private static void TestCutThenPrint(JiebaSegmenter segmenter, string s)
        {
            Console.WriteLine(string.Join("/ ", segmenter.Cut(s)));
        }

        [TestCase]
        public void TestAddWord()
        {
            var seg = new JiebaSegmenter();
            var s = "小明最近在学习机器学习和自然语言处理";

            var segments = seg.Cut(s);
            Assert.That(segments, Contains.Item("机器"));
            Assert.That(segments, Contains.Item("学习"));

            seg.AddWord("机器学习");
            segments = seg.Cut(s);
            Assert.That(segments, Contains.Item("机器学习"));
            Assert.That(segments, Is.Not.Contains("机器"));

            // reset dict otherwise other test cases would be affected.
            seg.DeleteWord("机器学习");
        }

        [TestCase]
        public void TestDeleteWord()
        {
            var seg = new JiebaSegmenter();
            var s = "小明最近在学习机器学习和自然语言处理";

            var segments = seg.Cut(s);
            Assert.That(segments, Contains.Item("机器"));
            Assert.That(segments, Is.Not.Contains("机器学习"));

            seg.AddWord("机器学习");
            segments = seg.Cut(s);
            Assert.That(segments, Contains.Item("机器学习"));
            Assert.That(segments, Is.Not.Contains("机器"));

            seg.DeleteWord("机器学习");
            segments = seg.Cut(s);
            Assert.That(segments, Contains.Item("机器"));
            Assert.That(segments, Is.Not.Contains("机器学习"));
        }

        [TestCase]
        public void TestCutSpecialWords()
        {
            var seg = new JiebaSegmenter();
            seg.AddWord(".NET");
            seg.AddWord("U.S.A.");
            
            var s = ".NET平台是微软推出的, U.S.A.是美国的简写";

            var segments = seg.Cut(s);
            foreach (var segment in segments)
            {
                Console.WriteLine(segment);
            }

            seg.LoadUserDict(TestHelper.GetResourceFilePath("user_dict.txt"));
            s = "Steve Jobs重新定义了手机";
            segments = seg.Cut(s);
            foreach (var segment in segments)
            {
                Console.WriteLine(segment);
            }

            s = "我们所熟悉的一个版本是Mac OS X 10.11 EI Capitan，在2015年推出。";
            segments = seg.Cut(s);
            foreach (var segment in segments)
            {
                Console.WriteLine(segment);
            }
        }

        [TestCase]
        public void TestCutAllSpecialWords()
        {
            var seg = new JiebaSegmenter();
            seg.AddWord(".NET");
            seg.AddWord("U.S.A.");
            seg.AddWord("Steve Jobs");

            var s = ".NET平台是微软推出的, U.S.A.是美国的简写";
            var segments = seg.Cut(s).ToList();
            Assert.That(segments,   Contains.Item(".NET"));
            Assert.That(segments,   Contains.Item("U.S.A."));

            s = "Steve Jobs重新定义了手机";
            segments = seg.Cut(s).ToList();
            Assert.That(segments,   Has.No.Member("Steve Jobs"));
        }

        [TestCase]
        public void TestCutTraditionalChinese()
        {
            var seg = new JiebaSegmenter();
            TestCutThenPrint(seg, "小明最近在學習機器學習和自然語言處理");
        }

        [TestCase]
        public void TestUserDict()
        {
            var dict = TestHelper.GetResourceFilePath("user_dict.txt");
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

        [TestCase]
        public void TestPercentages()
        {
            var seg = new JiebaSegmenter();
            
            var s = "看上去iphone8手机样式很赞,售价699美元,销量涨了5%么？";
            var segments = seg.Cut(s);
            Assert.That(segments, Contains.Item("5%"));
            foreach (var sm in segments)
            {
                Console.WriteLine(sm);
            }

            s = "pi的值是3.14，这是99.99%的人都知道的。";
            segments = seg.Cut(s);
            Assert.That(segments, Contains.Item("3.14"));
            Assert.That(segments, Contains.Item("99.99%"));
        }

        [TestCase]
        public void TestHyphen()
        {
            var seg = new JiebaSegmenter();
            seg.AddWord("cet-4");

            var s = "你一定也考过cet-4了。";
            var segments = seg.Cut(s).ToList();
            Assert.That(segments, Contains.Item("cet-4"));
            Console.WriteLine(segments);
            foreach (var sm in segments)
            {
                Console.WriteLine(sm);
            }
        }

        [TestCase]
        [Category("Issue")]
        public void TestChineseDot()
        {
            // for #42, #43
            var seg = new JiebaSegmenter();
            seg.AddWord("艾尔肯·吐尼亚孜");
            seg.AddWord("短P-R间期");

            var s = "艾尔肯·吐尼亚孜新疆阿克苏人。 在短P-R间期。";
            var segments = seg.Cut(s).ToList();
            Assert.That(segments, Contains.Item("艾尔肯·吐尼亚孜"));
            Assert.That(segments, Contains.Item("短P-R间期"));
        }

        [TestCase]
        [Category("Issue")]
        public void TestIssue49()
        {
            // for #49
            var seg = new JiebaSegmenter();

            var s = "简历名称 JAVA后端";
            var segments = seg.Cut(s);
            Assert.That(segments.Count(), Is.EqualTo(5));

            s = "简历名称JAVA后端";
            segments = seg.Cut(s);
            Assert.That(segments.Count(), Is.EqualTo(4));
        }

        [TestCase]
        public void TestCutAllMixedZhEn()
        {
            var seg = new JiebaSegmenter();
            seg.AddWord("超敏C反应蛋白");

            var s = "很多人的第一门语言是C语言。超敏C反应蛋白是什么？";
            var segments = seg.CutAll(s).ToList();
            Assert.That(segments, Contains.Item("C语言"));
            Console.WriteLine(segments);
            foreach (var sm in segments)
            {
                Console.WriteLine(sm);
            }
        }

        [TestCase]
        [Category("Issue")]
        public void TestIssue46()
        {
            var seg = new JiebaSegmenter();
            seg.DeleteWord("天半");
            
            var segments = seg.CutAll("2天半").ToList();
            Assert.That(segments, Contains.Item("天"));
            Assert.That(segments, Contains.Item("半"));
        }

        [TestCase]
        [Category("Issue")]
        public void TestEnglishWordsCut()
        {
            var seg = new JiebaSegmenter();
            var text = "HighestDegree";
            CollectionAssert.AreEqual(new[] { text }, seg.Cut(text));
            text = "HelloWorld";
            CollectionAssert.AreEqual(new[] { text }, seg.Cut(text));
            text = "HelloWorldle";
            CollectionAssert.AreEqual(new[] { text }, seg.Cut(text));
            text = "HelloWorldlee";
            CollectionAssert.AreEqual(new[] { text }, seg.Cut(text));
        }

        [Test]
        public void TestWordFreq()
        {
            var s = "在数学和计算机科学之中，算法（algorithm）为任何良定义的具体计算步骤的一个序列，常用于计算、数据处理和自动推理。精确而言，算法是一个表示为有限长列表的有效方法。算法应包含清晰定义的指令用于计算函数。";
            var seg = new JiebaSegmenter();
            var freqs = new Counter<string>(seg.Cut(s));
            // TODO: use stopwords.
            foreach (var pair in freqs.MostCommon(5))
            {
                Console.WriteLine($"{pair.Key}: {pair.Value}");
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