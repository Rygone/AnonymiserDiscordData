using System;
using System.IO.Compression;
using System.IO;

namespace Rygone.Tool
{
    public static class Ziper
    {
        private static string tempPath = null;
        public static string TempPath
        {
            get
            {
                if (tempPath == null)
                {
                    tempPath = Path.GetTempPath();
                    if (!Directory.Exists(tempPath))
                    {
                        throw new DirectoryNotFoundException("TempPath dosn't exists");
                    }
                    string temp;
                    Random rdm = new Random();
                    while (Directory.Exists(tempPath + (temp = rdm.Next().ToString()))) { }
                    tempPath += temp;
                    Directory.CreateDirectory(tempPath);
                }
                return tempPath; 
            }
        }
        public static bool UnZip(string from, string to = null)
        {
            if(from == null)
            {
                return false;
            }
            if(to == null)
            {
                to = TempPath;
            }
            ZipFile.ExtractToDirectory(from, to);
            return true;
        }
        public static bool Zip(string to, string from = null)
        {
            if (to == null)
            {
                return false;
            }
            if (from == null)
            {
                from = TempPath;
            }
            ZipFile.CreateFromDirectory(from, to);
            return true;
        }

    }
}
