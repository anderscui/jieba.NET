using System;
using System.IO;

namespace JiebaNet.Segmenter.Tests
{
    public class TestHelper
    {
        public static bool IsOnWindows()
        {
            return Environment.OSVersion.Platform.ToString().StartsWith("Win");
        }
        
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