using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace JiebaNet.Segmenter
{
    public static class Extensions
    {
        public static readonly Regex RegexDigits = new Regex(@"\d+", RegexOptions.Compiled);

        public static int ToInt32(this char ch)
        {
            return ch;
        }

        public static char ToChar(this int i)
        {
            return (char) i;
        }

        public static string Sub(this string s, int startIndex, int endIndex)
        {
            return s.Substring(startIndex, endIndex - startIndex);
        }

        public static bool IsInt32(this string s)
        {
            return RegexDigits.IsMatch(s);
        }

        public static TValue GetDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue defaultValue)
        {
            if (dict.ContainsKey(key))
            {
                return dict[key];
            }
            return defaultValue;
        }

        public static void Update<TKey, TValue>(this IDictionary<TKey, TValue> dict, IDictionary<TKey, TValue> other)
        {
            foreach (var key in other.Keys)
            {
                dict[key] = other[key];
            }
        }

        public static string Join(this IEnumerable<string> inputs, string separator = ", ")
        {
            return string.Join(separator, inputs);
        }

        public static bool IsEmpty<T>(this IEnumerable<T> enumerable)
        {
            return (enumerable == null) || !enumerable.Any();
        }

        public static bool IsNotEmpty<T>(this IEnumerable<T> enumerable)
        {
            return (enumerable != null) && enumerable.Any();
        }

        public static IEnumerable<string> SubGroupValues(this GroupCollection groups)
        {
            var result = from Group g in groups
                         select g.Value;
            return result.Skip(1);
        }
    }
}