using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace JiebaNet.Segmenter.Tests.FCL
{
    [TestFixture]
    public class TestJson
    {
        [TestCase]
        public void TestDeserializeDict()
        {
            var p = TestHelper.GetResourceFilePath("test_dict.json");
            var jsonContent = File.ReadAllText(p);
            var probs = JsonConvert.DeserializeObject<Dictionary<char, double>>(jsonContent);
            Assert.That(probs, Is.Not.Null);
            Assert.That(probs.Count, Is.EqualTo(3));
            Assert.That(probs['上'], Is.EqualTo(-8.541143017159477));
        }

        [TestCase]
        public void TestDeserializeProbStart()
        {
            var p = TestHelper.GetResourceFilePath("pos_prob_start.json");
            var jsonContent = File.ReadAllText(p);
            var probs = JsonConvert.DeserializeObject<IDictionary<string, double>>(jsonContent);
            Assert.That(probs, Is.Not.Null);
            Assert.That(probs.Count, Is.EqualTo(256));
            Assert.That(probs["B-f"], Is.EqualTo(-5.491630418482717));
        }

        [TestCase]
        public void TestDeserializeProbTrans()
        {
            var p = TestHelper.GetResourceFilePath("pos_prob_trans.json");
            var jsonContent = File.ReadAllText(p);
            var probs = JsonConvert.DeserializeObject<IDictionary<string, IDictionary<string, double>>>(jsonContent);
            Assert.That(probs, Is.Not.Null);
            Assert.That(probs.Count, Is.EqualTo(256));
            Assert.That(probs.Select(k => k.Value.Count).Sum(), Is.EqualTo(5218));
            Assert.That(probs["E-f"]["B-f"], Is.EqualTo(-4.533974775212322));
        }

        [TestCase]
        public void TestDeserializeProbEmit()
        {
            var p = TestHelper.GetResourceFilePath("pos_prob_emit.json");
            var jsonContent = File.ReadAllText(p);
            var probs = JsonConvert.DeserializeObject<IDictionary<string, IDictionary<char, double>>>(jsonContent);
            Assert.That(probs, Is.Not.Null);
            Assert.That(probs.Count, Is.EqualTo(256));
            Assert.That(probs.Select(k => k.Value.Count).Sum(), Is.EqualTo(89290));
            Assert.That(probs["E-f"]['上'], Is.EqualTo(-2.8053792589892286));
        }

        [TestCase]
        public void TestDeserializeCharStateTab()
        {
            var p = TestHelper.GetResourceFilePath("char_state_tab.json");
            var jsonContent = File.ReadAllText(p);
            var probs = JsonConvert.DeserializeObject<IDictionary<string, List<string>>>(jsonContent);
            Assert.That(probs, Is.Not.Null);
            Assert.That(probs.Count, Is.EqualTo(6648));
            Assert.That(probs.Select(k => k.Value.Count).Sum(), Is.EqualTo(66162));
            Assert.That(probs["一"].Contains("E-r"));
            Assert.That(probs["上"].Contains("B-f"));
        }

        [TestCase]
        public void TestSerializeObject()
        {
            var d = new Dictionary<char, double>
            {
                {'B', -0.26268660809250016},
                {'E', -3.14e+100},
                {'M', -3.14e+100},
                {'S', -1.4652633398537678}
            };
            var output = JsonConvert.SerializeObject(d);
            Console.WriteLine(output);
        }
    }
}