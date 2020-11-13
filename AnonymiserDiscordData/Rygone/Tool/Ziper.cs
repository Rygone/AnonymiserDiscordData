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
                    int i = 0;
                    while (Directory.Exists(tempPath + (temp = rdm.Next().ToString())))
                    {
                        if (i++ > 25)
                            throw new DirectoryNotFoundException("TempPath dosn't exists");
                    }
                    tempPath += temp;
                    Directory.CreateDirectory(tempPath);
                }
                return tempPath; 
            }
        }
        public static bool UnZip(string from, string to = null)
        {
            if(from == null)
                return false;
            if(to == null)
                try
                {
                    if (Directory.Exists(TempPath))
                        tempPath = null;
                    to = TempPath;
                }
                catch { return false; }
            ZipFile.ExtractToDirectory(from, to);
            return true;
        }
        public static bool Zip(string to, string from = null)
        {
            if (to == null || File.Exists(to))
                return false;
            if (from == null)
                try { from = TempPath; }
                catch { return false; }
            ZipFile.CreateFromDirectory(from, to);
            return true;
        }

    }
}
