using System;
using System.Configuration;
using System.IO;

namespace JiebaNet.Segmenter
{
    public class ConfigManager
    {
        public static string ConfigFileBaseDir
        {
            get
            {
#if !NETSTANDARD2_0
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

        public static string MainDictFile
        {
            get { return Path.Combine(ConfigFileBaseDir, "dict.txt"); }
        }

        public static string ProbTransFile
        {
            get { return Path.Combine(ConfigFileBaseDir, "prob_trans.json"); }
        }

        public static string ProbEmitFile
        {
            get { return Path.Combine(ConfigFileBaseDir, "prob_emit.json"); }
        }

        public static string PosProbStartFile
        {
            get { return Path.Combine(ConfigFileBaseDir, "pos_prob_start.json"); }
        }

        public static string PosProbTransFile
        {
            get { return Path.Combine(ConfigFileBaseDir, "pos_prob_trans.json"); }
        }

        public static string PosProbEmitFile
        {
            get { return Path.Combine(ConfigFileBaseDir, "pos_prob_emit.json"); }
        }

        public static string CharStateTabFile
        {
            get { return Path.Combine(ConfigFileBaseDir, "char_state_tab.json"); }
        }
    }
}