using System;
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
    }
}