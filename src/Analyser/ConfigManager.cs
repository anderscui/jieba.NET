using System;
using System.Configuration;
using System.IO;

namespace JiebaNet.Analyser
{
    public class ConfigManager
    {
        public static string ConfigFileBaseDir
        {
            get
            {
                var configFileDir = ConfigurationManager.AppSettings["JiebaConfigFileDir"] ?? "Resources";
                if (!Path.IsPathRooted(configFileDir))
                {
                    var domainDir = AppDomain.CurrentDomain.BaseDirectory;
                    configFileDir = Path.GetFullPath(Path.Combine(domainDir, configFileDir));
                }
                return configFileDir;
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