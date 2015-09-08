using System.Configuration;

namespace JiebaNet.Analyser
{
    public class ConfigManager
    {
        public static string IdfFile
        {
            get { return ConfigurationManager.AppSettings["IdfFile"] ?? @"Resources\idf.txt"; }
        }

        public static string StopWordsFile
        {
            get { return ConfigurationManager.AppSettings["StopWordsFile"] ?? @"Resources\stopwords.txt"; }
        }
    }
}