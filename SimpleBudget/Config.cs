using System;
using System.IO;

namespace SimpleBudget
{
    static class Config
    {
        private const string CONFIG = "app.config";
        private const string APPDIR = "SimpleBudget";
        private static string path = string.Empty;

        static Config()
        {
            try
            {
                string dirPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), APPDIR);
                Directory.CreateDirectory(dirPath);
                path = Path.Combine(dirPath, CONFIG);
            }
            catch (Exception) { }
        }

        public static string RetrieveFileName()
        {
            try
            {
                return File.ReadAllLines(path)[0];
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static void StoreFileName(string fileName)
        {
            try
            {
                File.WriteAllText(path, fileName);
            }
            catch (Exception) { }
        }
    }
}
