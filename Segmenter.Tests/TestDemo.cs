using System;
using System.Linq;
using JiebaNet.Analyser;
using JiebaNet.Segmenter.PosSeg;
using NUnit.Framework;

namespace JiebaNet.Segmenter.Tests
{
    [TestFixture]
    public class TestDemo
    {
        [TestCase]
        public void CutDemo()
        {
            var segmenter = new JiebaSegmenter();
            var segments = segmenter.Cut("我来到北京清华大学", cutAll: true);
            Console.WriteLine("【全模式】：{0}", string.Join("/ ", segments));

            segments = segmenter.Cut("我来到北京清华大学");  // 默认为精确模式
            Console.WriteLine("【精确模式】：{0}", string.Join("/ ", segments));

            segments = segmenter.Cut("他来到了网易杭研大厦");  // 默认为精确模式，同时也使用HMM模型
            Console.WriteLine("【新词识别】：{0}", string.Join("/ ", segments));

            segments = segmenter.CutForSearch("小明硕士毕业于中国科学院计算所，后在日本京都大学深造"); // 搜索引擎模式
            Console.WriteLine("【搜索引擎模式】：{0}", string.Join("/ ", segments));

            segments = segmenter.Cut("结过婚的和尚未结过婚的");
            Console.WriteLine("【歧义消除】：{0}", string.Join("/ ", segments));

            segments = segmenter.Cut("北京大学生喝进口红酒");
            Console.WriteLine("【歧义消除】：{0}", string.Join("/ ", segments));

            segments = segmenter.Cut("在北京大学生活区喝进口红酒");
            Console.WriteLine("【歧义消除】：{0}", string.Join("/ ", segments));
        }

        [TestCase]
        public void TokenizeDemo()
        {
            var segmenter = new JiebaSegmenter();
            var s = "永和服装饰品有限公司";
            var tokens = segmenter.Tokenize(s);
            foreach (var token in tokens)
            {
                Console.WriteLine("word {0,-12} start: {1,-3} end: {2,-3}", token.Word, token.StartIndex, token.EndIndex);
            }
        }

        [TestCase]
        public void TokenizeSearchDemo()
        {
            var segmenter = new JiebaSegmenter();
            var s = "永和服装饰品有限公司";
            var tokens = segmenter.Tokenize(s, TokenizerMode.Search);
            foreach (var token in tokens)
            {
                Console.WriteLine("word {0,-12} start: {1,-3} end: {2,-3}", token.Word, token.StartIndex, token.EndIndex);
            }
        }

        [TestCase]
        public void PosCutDemo()
        {
            var posSeg = new PosSegmenter();
            var s = "一团硕大无朋的高能离子云，在遥远而神秘的太空中迅疾地飘移";

            var tokens = posSeg.Cut(s);
            Console.WriteLine(string.Join(" ", tokens.Select(token => string.Format("{0}/{1}", token.Word, token.Flag))));
        }

        [TestCase]
        public void ExtractTagsDemo()
        {
            var text =
                "程序员(英文Programmer)是从事程序开发、维护的专业人员。一般将程序员分为程序设计人员和程序编码人员，但两者的界限并不非常清楚，特别是在中国。软件从业人员分为初级程序员、高级程序员、系统分析员和项目经理四大类。";
            var extractor = new TfidfExtractor();
            var keywords = extractor.ExtractTags(text);
            foreach (var keyword in keywords)
            {
                Console.WriteLine(keyword);
            }
        }
    }
}