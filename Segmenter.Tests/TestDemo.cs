using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace JiebaNet.Segmenter.Tests
{
    [TestFixture]
    public class TestDemo
    {

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
        public void Demo()
        {
        var segmenter = new JiebaSegmenter();
        var segments = segmenter.Cut("我来到北京清华大学", cutAll: true);
        Console.WriteLine("【全模式】：{0}", string.Join("/ ", segments));

        segments = segmenter.Cut("我来到北京清华大学");  // 默认为精确模式
        Console.WriteLine("【精确模式】：{0}", string.Join("/ ", segments));

        segments = segmenter.Cut("他来到了网易杭研大厦");  // 默认为精确模式，同时也使用HMM模型
        Console.WriteLine("【新词识别】：{0}", string.Join(", ", segments));

        segments = segmenter.CutForSearch("小明硕士毕业于中国科学院计算所，后在日本京都大学深造"); // 搜索引擎模式
        Console.WriteLine("【搜索引擎模式】：{0}", string.Join(", ", segments));
        }
    }
}