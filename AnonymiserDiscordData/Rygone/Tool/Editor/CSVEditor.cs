using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Rygone.Tool.Editor
{
    public static class CSVEditor
    {
        public delegate string[] ForLine(IReadOnlyDictionary<string, string> line, int nb);
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
                string[] ColumnTitle = null;
                Dictionary<string, string> LineData = null;
                List<string> line = new List<string>();
                int nb = 0;
                while (editor.HasNext())
                {
                    line.Clear();
                    {
                        int i = 0;
                        line.Add("");
                        bool guimet = false;
                        bool cont = true;
                        while (cont && editor.HasNext())
                        {
                            char c = editor.Read();
                            if (c == '"')
                            {
                                line[i] += c;
                                guimet = !guimet;
                            }
                            else if (guimet)
                                line[i] += c;
                            else if (c == ',')
                            {
                                line.Add("");
                                ++i;
                            }
                            else if (c == '\n')
                            {
                                cont = false;
                                line[i] = line[i][0..^1];
                            }
                            else
                                line[i] += c;
                        }
                        if (ColumnTitle != null && ColumnTitle.Length != line.Count)
                            return;
                    }

                    if (ColumnTitle == null)
                    {
                        ColumnTitle = line.ToArray();
                        LineData = new Dictionary<string, string>();
                        foreach (string key in ColumnTitle)
                            LineData.Add(key, key);
                        Write(editor, edit(LineData, nb++));
                    }
                    else
                    {
                        int i = 0;
                        foreach (string key in ColumnTitle)
                            LineData[key] = line[i++];
                        Write(editor, edit(LineData, nb++));
                    }
                }
                editor.Close();
            });
            res.Start();
            return res;
        }
        private static void Write(IEditor editor, string[] datas)
        {
            int i = 0;
            foreach (string data in datas)
            {
                if (i++ != 0)
                    editor.Write(",");
                editor.Write(data);
            }
            editor.Write("\r\n");
        }
    }
}