using System;
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
#if !(NETSTANDARD1_0 || NETSTANDARD2_0)
                var configFileDir = ConfigurationManager.AppSettings["JiebaConfigFileDir"] ?? "Resources";
                if (!Path.IsPathRooted(configFileDir))
                {
                    var domainDir = AppDomain.CurrentDomain.BaseDirectory;
                    configFileDir = Path.GetFullPath(Path.Combine(domainDir, configFileDir));
                }
#else
                var configFileDir = "Resources";
#endif
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
