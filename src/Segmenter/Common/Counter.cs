using System;
using System.Collections.Generic;
using System.Linq;

namespace JiebaNet.Segmenter.Common
{
    public interface ICounter<T>
    {
        int Count { get; }
        int Total { get; }
        int this[T key] { get; set; }
        IEnumerable<KeyValuePair<T, int>> Elements { get; }

        IEnumerable<KeyValuePair<T, int>> MostCommon(int n = -1);
        void Subtract(IEnumerable<T> items);
        void Subtract(ICounter<T> other);
        void Update(IEnumerable<T> items);
        void Update(ICounter<T> other);

        void Remove(T key);
        bool Contains(T key);
    }

    public class Counter<T>: ICounter<T>
    {
        private Dictionary<T, int> data = new Dictionary<T, int>();

        public Counter() {}

        public Counter(IEnumerable<T> items)
        {
            CountItems(items);
        }

        private void CountItems(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                data[item] = data.GetDefault(item, 0) + 1;
            }
        }

        private void CountPairs(IEnumerable<KeyValuePair<T, int>> pairs)
        {
            foreach (var pair in pairs)
            {
                this[pair.Key] += pair.Value;
            }
        }

        private void SubtractItems(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                data[item] = data.GetDefault(item, 0) - 1;
            }
        }

        private void SubtractPairs(IEnumerable<KeyValuePair<T, int>> pairs)
        {
            foreach (var pair in pairs)
            {
                this[pair.Key] -= pair.Value;
            }
        }

        public int Count => data.Count;
        public int Total => data.Values.Sum();
        public IEnumerable<KeyValuePair<T, int>> Elements => data;

        public int this[T key]
        {
            get => data.ContainsKey(key) ? data[key] : 0;
            set => data[key] = value;
        }

        public IEnumerable<KeyValuePair<T, int>> MostCommon(int n = -1)
        {
            var pairs = data.Where(pair => pair.Value > 0).OrderByDescending(pair => pair.Value);
            return n < 0 ? pairs : pairs.Take(n);
        }

        public void Subtract(IEnumerable<T> items)
        {
            SubtractItems(items);
        }

        public void Subtract(ICounter<T> other)
        {
            SubtractPairs(other.Elements);
        }

        public void Update(IEnumerable<T> items)
        {
            CountItems(items);
        }

        public void Update(ICounter<T> other)
        {
            CountPairs(other.Elements);
        }

        public void Remove(T key)
        {
            if (data.ContainsKey(key))
            {
                data.Remove(key);
            }
        }

        public bool Contains(T key)
        {
            return data.ContainsKey(key);
        }
    }
}
