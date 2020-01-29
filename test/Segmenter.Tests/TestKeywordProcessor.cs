using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using JiebaNet.Segmenter.Common;

namespace JiebaNet.Segmenter.Tests
{
    [TestFixture]
    public class TestKeywordProcessor
    {
        private KeywordProcessor GetSimpleProcessor()
        {
            var kp = new KeywordProcessor();
            kp.AddKeywords(new []{".NET Core", "Java", "C语言", "字典 tree", "CET-4", "网络 编程"});
            return kp;
        }
        
        [TestCase]
        public void TestCreateProcessor()
        {
            var kp = new KeywordProcessor();
            kp.AddKeyword("Big Apple", cleanName: "New York");
            
            Assert.That(kp.CaseSensitive, Is.False);
            Assert.That(kp.Contains("Big"), Is.False);
            Assert.That(kp.Contains("Big Apple"), Is.True);
        }
        
        [TestCase]
        public void TestRemoveKeyword()
        {
            var kp = new KeywordProcessor();
            kp.AddKeyword(".net core");
            kp.AddKeyword("C# 8.0");
            kp.AddKeywords(new []{"C# 7.0", "C# 8.0"});
            
            var keywords = kp.ExtractKeywords("I am learning .net core and c# 8.0");
            var expected = new List<string> { ".net core", "C# 8.0"};
            CollectionAssert.AreEqual(expected, keywords);
            
            Assert.That(kp.Contains("C# 8.0"), Is.True);
            kp.RemoveKeyword("C# 8.0");
            Assert.That(kp.Contains("C# 8.0"), Is.False);
            keywords = kp.ExtractKeywords("I am learning .net core and c# 8.0");
            expected = new List<string> { ".net core"};
            CollectionAssert.AreEqual(expected, keywords);
        }

        [TestCase]
        public void TestExtract()
        {
            var kp = new KeywordProcessor();
            kp.AddKeywords(new []{"Big Apple", "Bay Area"});
            var keywords = kp.ExtractKeywords("I love Big Apple and Bay Area.");
            var expected = new List<string> { "Big Apple", "Bay Area"};
            CollectionAssert.AreEqual(expected, keywords);
        }
        
        [TestCase]
        public void TestExtractSpans()
        {
            var kp = new KeywordProcessor();
            kp.AddKeywords(new []{"Big Apple", "Bay Area"});
            var keywords = kp.ExtractKeywordSpans("I love Big Apple and Bay Area.");
            var expected = new List<TextSpan> { new TextSpan("Big Apple", 7, 16), new TextSpan("Bay Area", 21, 29)};
            CollectionAssert.AreEqual(expected, keywords);
        }
        
        [TestCase]
        public void TestExtractNone()
        {
            var kp = GetSimpleProcessor();
            
            var keywords = kp.ExtractKeywords("疾风知劲草。");
            Assert.That(keywords.IsEmpty(), Is.True);
        }
        
        [TestCase]
        public void TestExtractBounds()
        {
            var kp = GetSimpleProcessor();
            
            var keywords = kp.ExtractKeywords(".net core.");
            var expected = new List<string> { ".NET Core"};
            CollectionAssert.AreEqual(expected, keywords);
            
            keywords = kp.ExtractKeywords(".net core");
            expected = new List<string> { ".NET Core"};
            CollectionAssert.AreEqual(expected, keywords);
        }
        
        [TestCase]
        public void TestExtractMixed()
        {
            var kp = GetSimpleProcessor();
            
            var keywords = kp.ExtractKeywords("你需要通过cet-4考试，学习c语言、.NET core、网络 编程、JavaScript，掌握字典 tree的用法");
            // Java is not extracted.
            var expected = new List<string> { "CET-4", "C语言", ".NET Core", "网络 编程", "字典 tree"};
            CollectionAssert.AreEqual(expected, keywords);
        }
        
        [TestCase]
        public void TestExtractMixedRaw()
        {
            var kp = GetSimpleProcessor();
            
            var keywords = 
                kp.ExtractKeywords("你需要通过cet-4考试，学习c语言、.NET core、网络 编程、JavaScript，掌握字典 tree的用法", raw: true);
            // Java is not extracted.
            var expected = new List<string> { "cet-4", "c语言", ".NET core", "网络 编程", "字典 tree"};
            CollectionAssert.AreEqual(expected, keywords);
        }
    }
}