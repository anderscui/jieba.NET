using System;
using System.Linq;
using JiebaNet.Segmenter.Common;
using NUnit.Framework;

namespace JiebaNet.Segmenter.Tests.Common
{
    [TestFixture]
    public class TestCounter
    {
        [Test]
        public void TestCreateEmpty()
        {
            var counter = new Counter<int>();
            Assert.That(counter.Count, Is.EqualTo(0));
            Assert.That(counter.Total, Is.EqualTo(0));

            counter[2] = 10;
            Assert.That(counter.Count, Is.EqualTo(1));
            Assert.That(counter.Total, Is.EqualTo(10));
            Assert.That(counter[2], Is.EqualTo(10));
        }

        [Test]
        public void TestCreateWithEnumerable()
        {
            var source = "gallahad";
            var counter = new Counter<char>(source);
            Assert.That(counter.Total, Is.EqualTo(source.Length));

            Assert.That(counter['d'], Is.EqualTo(1));
            Assert.That(counter['a'], Is.EqualTo(3));
        }

        [Test]
        public void TestElements()
        {
            var source = "gallahad";
            var counter = new Counter<char>(source);
            var elements = counter.Elements;
            Assert.That(elements.Count(), Is.EqualTo(5));
        }

        [Test]
        public void TestUpdateWithEnumerable()
        {
            var counter = new Counter<char>("which");
            Assert.That(counter['h'], Is.EqualTo(2));
            counter.Update("witch");
            Assert.That(counter['h'], Is.EqualTo(3));
        }

        [Test]
        public void TestUpdateWithCounter()
        {
            var counter = new Counter<char>("which");
            var counter2 = new Counter<char>("witch");
            counter.Update(counter2);
            Assert.That(counter['h'], Is.EqualTo(3));
        }

        [Test]
        public void TestSubtractWithEnumerable()
        {
            var counter = new Counter<char>("which");
            Assert.That(counter['h'], Is.EqualTo(2));
            counter.Subtract("witch");
            Assert.That(counter['h'], Is.EqualTo(1));
            Assert.That(counter['w'], Is.EqualTo(0));
        }

        [Test]
        public void TestSubtractWithCounter()
        {
            var counter = new Counter<char>("which");
            var counter2 = new Counter<char>("witch");
            counter.Subtract(counter2);
            Assert.That(counter['h'], Is.EqualTo(1));
            Assert.That(counter['w'], Is.EqualTo(0));
        }

        [Test]
        public void TestContains()
        {
            var counter = new Counter<char>("which");
            Assert.That(counter.Contains('w'), Is.True);
            Assert.That(counter.Contains('t'), Is.False);
        }

        [Test]
        public void TestRemove()
        {
            var counter = new Counter<char>("which");
            counter['t'] = 0;
            Assert.That(counter.Contains('t'), Is.True);

            counter.Remove('t');
            Assert.That(counter.Contains('t'), Is.False);
        }
        
        [Test]
        public void TestMostCommon()
        {
            var counter = new Counter<char>("abcdeabcdabcaba");
            var top3 = counter.MostCommon(3).ToList();
            Assert.That(top3.First().Key, Is.EqualTo('a'));
            Assert.That(top3.First().Value, Is.EqualTo(5));
            Assert.That(top3.Last().Key, Is.EqualTo('c'));
            Assert.That(top3.Last().Value, Is.EqualTo(3));

            var all = counter.MostCommon().ToList();
            Assert.That(all.First().Key, Is.EqualTo('a'));
            Assert.That(all.First().Value, Is.EqualTo(5));
            Assert.That(all.Last().Key, Is.EqualTo('e'));
            Assert.That(all.Last().Value, Is.EqualTo(1));

            var none = counter.MostCommon(0).ToList();
            Assert.That(none.Count, Is.EqualTo(0));
        }
    }
}
