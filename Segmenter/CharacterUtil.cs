using System;
using System.Text.RegularExpressions;

namespace JiebaNet.Segmenter
{
    public class CharacterUtil
    {
        public static readonly Regex RegexSkip = new Regex("(\\d+\\.\\d+|[a-zA-Z0-9]+)", RegexOptions.Compiled);
        private static readonly char[] connectors = new char[] { '+', '#', '&', '.', '_', '-' };

        public static Boolean isChineseLetter(char ch)
        {
            if (ch >= 0x4E00 && ch <= 0x9FA5)
                return true;
            return false;
        }

        public static Boolean isEnglishLetter(char ch)
        {
            if ((ch >= 0x0041 && ch <= 0x005A) || (ch >= 0x0061 && ch <= 0x007A))
                return true;
            return false;
        }

        public static Boolean isDigit(char ch)
        {
            if (ch >= 0x0030 && ch <= 0x0039)
                return true;
            return false;
        }

        public static Boolean isConnector(char ch)
        {
            foreach (var connector in connectors)
                if (ch == connector)
                    return true;
            return false;
        }

        public static Boolean ccFind(char ch)
        {
            if (isChineseLetter(ch))
                return true;
            if (isEnglishLetter(ch))
                return true;
            if (isDigit(ch))
                return true;
            if (isConnector(ch))
                return true;
            return false;
        }

        /**
         * 全角 to 半角,大写 to 小写
         * 
         * @param input
         *            输入字符
         * @return 转换后的字符
         */
        public static char regularize(char input)
        {
            if (input == 12288)
            {
                return (char)32;
            }

            if (input > 65280 && input < 65375)
            {
                return (char)(input - 65248);
            }

            if (input >= 'A' && input <= 'Z')
            {
                return (char)(input.ToInt32() + 32);
            }

            return input;
        }
    }
}