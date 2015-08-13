using System;

namespace JiebaNet.Segmenter
{
    public static class Extensions
    {
        public static int ToInt32(this char ch)
        {
            return (int) Char.GetNumericValue(ch);
        }

        public static char ToChar(this int i)
        {
            return (char) i;
        }
    }
}