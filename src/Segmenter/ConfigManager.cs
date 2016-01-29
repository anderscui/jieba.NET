using System.Configuration;

namespace JiebaNet.Segmenter
{
    public class ConfigManager
    {
        public static string MainDictFile
        {
            get { return ConfigurationManager.AppSettings["MainDictFile"] ?? @"Resources\dict.txt"; }
        }

        public static string ProbTransFile
        {
            get { return ConfigurationManager.AppSettings["ProbTransFile"] ?? @"Resources\prob_trans.json"; }
        }

        public static string ProbEmitFile
        {
            get { return ConfigurationManager.AppSettings["ProbEmitFile"] ?? @"Resources\prob_emit.json"; }
        }

        public static string PosProbStartFile
        {
            get { return ConfigurationManager.AppSettings["PosProbStartFile"] ?? @"Resources\pos_prob_start.json"; }
        }

        public static string PosProbTransFile
        {
            get { return ConfigurationManager.AppSettings["PosProbTransFile"] ?? @"Resources\pos_prob_trans.json"; }
        }

        public static string PosProbEmitFile
        {
            get { return ConfigurationManager.AppSettings["PosProbEmitFile"] ?? @"Resources\pos_prob_emit.json"; }
        }

        public static string CharStateTabFile
        {
            get { return ConfigurationManager.AppSettings["CharStateTabFile"] ?? @"Resources\char_state_tab.json"; }
        }
    }
}