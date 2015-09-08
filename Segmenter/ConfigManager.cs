using System.Configuration;

namespace JiebaNet.Segmenter
{
    public class ConfigManager
    {
        public static string MainDictFile
        {
            get { return ConfigurationManager.AppSettings["MainDictFile"]; }
        }

        public static string ProbTransFile
        {
            get { return ConfigurationManager.AppSettings["ProbTransFile"]; }
        }

        public static string ProbEmitFile
        {
            get { return ConfigurationManager.AppSettings["ProbEmitFile"]; }
        }

        public static string PosProbStartFile
        {
            get { return ConfigurationManager.AppSettings["PosProbStartFile"]; }
        }

        public static string PosProbTransFile
        {
            get { return ConfigurationManager.AppSettings["PosProbTransFile"]; }
        }

        public static string PosProbEmitFile
        {
            get { return ConfigurationManager.AppSettings["PosProbEmitFile"]; }
        }

        public static string CharStateTabFile
        {
            get { return ConfigurationManager.AppSettings["CharStateTabFile"]; }
        }
    }
}