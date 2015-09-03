using System;
using System.Collections.Generic;
using System.IO;
using JiebaNet.Segmenter.PosSeg;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Segmenter.Tests.FCL
{
    [TestFixture]
    public class TestJson
    {
        [TestCase]
        public void TestDeserializeObject()
        {
            var p = @"Resources\pos_prob_start.json";
            var jsonContent = File.ReadAllText(p);
            var probs = JsonConvert.DeserializeObject<List<PosStateProb>>(jsonContent);
            Assert.That(probs, Is.Not.Null);
            Assert.That(probs.Count, Is.EqualTo(256));
            Console.WriteLine("key: {0}, val: {1}", probs[0].PosState,
                probs[0].Prob);

            var p2 = @"Resources\pos_prob_trans.json";
            jsonContent = File.ReadAllText(p2);
            var transProbs = JsonConvert.DeserializeObject<List<PosStateTransProb>>(jsonContent);
            Assert.That(transProbs, Is.Not.Null);
            Assert.That(transProbs[0].PosStateFrom, Is.EqualTo("B-a"));
            Assert.That(transProbs[0].PosStatesTo[0].PosState, Is.EqualTo("E-a"));
            Assert.That(transProbs[0].PosStatesTo[0].Prob, Is.EqualTo(-0.0050648453069648755));
        }
    }
}