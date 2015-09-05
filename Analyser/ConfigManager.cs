using System.Configuration;

namespace JiebaNet.Analyser
{
    public class ConfigManager
    {
        public static string IdfFile
        {
            get { return ConfigurationManager.AppSettings["IdfFile"]; }
        }
    }
}