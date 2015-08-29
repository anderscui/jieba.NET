using System.Configuration;

namespace JiebaNet.Segmenter
{
    public class ConfigManager
    {
        public static string MainDictFile
        {
            get { return ConfigurationManager.AppSettings["MainDictFile"]; }
        }

        public static string ProbEmitFile
        {
            get { return ConfigurationManager.AppSettings["ProbEmitFile"]; }
        }
    }
}