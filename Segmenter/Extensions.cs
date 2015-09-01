using System.Collections.Generic;
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

        public static string Join(this IEnumerable<string> inputs, string separator = ", ")
        {
            return string.Join(separator, inputs);
        }
    }
}