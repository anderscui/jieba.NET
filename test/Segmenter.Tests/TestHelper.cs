using System.IO;

namespace JiebaNet.Segmenter.Tests
{
    public class TestHelper
    {
        public static string GetCaseFilePath(string fileName)
        {
            var path = Path.Combine("Cases", fileName); 
            return path;
        }
        
        public static string GetResourceFilePath(string fileName)
        {
            return Path.Combine("Resources", fileName);
        }
    }
}