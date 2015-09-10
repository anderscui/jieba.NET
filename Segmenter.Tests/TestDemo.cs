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

        [TestCase]
        public void ExtractTagsDemo2()
        {
            var text = @"在数学和计算机科学/算学之中，算法/算则法（Algorithm）为一个计算的具体步骤，常用于计算、数据处理和自动推理。精确而言，算法是一个表示为有限长列表的有效方法。算法应包含清晰定义的指令用于计算函数。
                         算法中的指令描述的是一个计算，当其运行时能从一个初始状态和初始输入（可能为空）开始，经过一系列有限而清晰定义的状态最终产生输出并停止于一个终态。一个状态到另一个状态的转移不一定是确定的。随机化算法在内的一些算法，包含了一些随机输入。
                         形式化算法的概念部分源自尝试解决希尔伯特提出的判定问题，并在其后尝试定义有效计算性或者有效方法中成形。这些尝试包括库尔特·哥德尔、雅克·埃尔布朗和斯蒂芬·科尔·克莱尼分别于1930年、1934年和1935年提出的递归函数，阿隆佐·邱奇于1936年提出的λ演算，1936年Emil Leon Post的Formulation 1和艾伦·图灵1937年提出的图灵机。即使在当前，依然常有直觉想法难以定义为形式化算法的情况。";

            var extractor = new TfidfExtractor();
            var keywords = extractor.ExtractTags(text, 10, Constants.NounAndVerbPos);
            foreach (var keyword in keywords)
            {
                Console.WriteLine(keyword);
            }
        }
    }
}