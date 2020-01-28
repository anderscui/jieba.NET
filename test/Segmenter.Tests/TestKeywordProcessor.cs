using System;
using System.Linq;
using JiebaNet.Analyser;
using NUnit.Framework;

namespace JiebaNet.Segmenter.Tests
{
    [TestFixture]
    public class TestKeywordProcessor
    {
        [TestCase]
        public void TestCreateProcessor()
        {
            var kp = new KeywordProcessor();
            kp.AddKeyword("Big Apple", cleanName: "New York");
            Assert.That(kp.Length, Is.EqualTo(1));
            Assert.That(kp.Contains("Big"), Is.False);
            Assert.That(kp.Contains("Big Apple"), Is.True);
            
            kp.AddKeyword("Bay Area");
            Assert.That(kp.Length, Is.EqualTo(2));
        }

        // [TestCase]
        // public void TestExtract()
        // {
        //     var kp = new KeywordProcessor();
        //     kp.AddKeywords(new []{"Big Apple", "Bay Area"});
        //     var keywordsFound = kp.ExtractKeywords("I love Big Apple and Bay Area.");
        //     Assert.That(keywordsFound.Count(), Is.EqualTo(2));
        // }
        
        [TestCase]
        public void TestExtract2()
        {
            var kp = new KeywordProcessor();
            kp.AddKeywords(new []{"Big Apple", "Big"});
            var keywordsFound = kp.ExtractKeywords("Big");
        }
    }
}