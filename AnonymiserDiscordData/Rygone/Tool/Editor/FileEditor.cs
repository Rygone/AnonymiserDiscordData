using System.IO;

namespace Rygone.Tool.Editor
{
    public class FileEditor : IEditor
    {
        private readonly string path;
        private readonly StreamReader sr;
        private readonly StreamWriter sw;
        public FileEditor(string path)
        {
            this.path = path;
            sr = new StreamReader(path);
            sw = new StreamWriter($"{path}.temp");
        }

        public void Close()
        {
            sr.Close();
            sw.Close();
            File.Delete(path);
            File.Move($"{path}.temp", path);
        }
        public bool HasNext() => sr.Peek() >= 0;
        public char Read() => (char)sr.Read();
        public string ReadLine() => sr.ReadLine();
        public void Write(string data) => sw.Write(data);
    }
}
