using System.Configuration;
using System.IO;

namespace JiebaNet.Analyser
{
    public class ConfigManager
    {
        // TODO: duplicate codes.
        public static string ConfigFileBaseDir
        {
            get
            {
                return ConfigurationManager.AppSettings["JiebaConfigFileDir"] ?? "Resources";
            }
        }

        public static string IdfFile
        {
            get { return Path.Combine(ConfigFileBaseDir, "idf.txt"); }
        }

        public static string StopWordsFile
        {
            get { return Path.Combine(ConfigFileBaseDir, "stopwords.txt"); }
        }
    }
}