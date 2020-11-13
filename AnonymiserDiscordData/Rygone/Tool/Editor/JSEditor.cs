using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Rygone.Tool.Editor
{
    public static class JSEditor
    {
        public delegate JObject ForLine(JObject line, int nb);

        public static Task Edit(string path, ForLine edit)
        {
            if (File.Exists(path))
                return Edit(new FileEditor(path), edit);
            return null;
        }
        public static Task Edit(IEditor editor, ForLine edit)
        {
            Task res = new Task(() =>
            {
                List<string> line = new List<string>();
                int nb = 0;
                while (editor.HasNext())
                    Write(editor, edit(JObject.Parse(editor.ReadLine()), nb++));
                editor.Close();
            });
            res.Start();
            return res;
        }
        private static void Write(IEditor editor, JObject datas) 
            => editor.Write(datas.ToString().Replace("\r", "").Replace("\n", "") + "\r\n");
    }
}