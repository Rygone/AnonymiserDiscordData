using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace AnonymiserDiscordData.Util
{
    public static class Settings
    {
        public static HashSet<Control> Controls = new HashSet<Control>();
        public static class LanguageText
        {
            public enum Language
            {
                NULL, FR, EN
            }
            public static class LanguageExtensions
            {
                public static Language[] values()
                {
                    return new Language[] { Language.FR, Language.EN };
                }
                public static string File(Language l)
                {
                    return @$"Language\{Abbreviation(l)}";
                }
                public static string FullText(Language language)
                {
                    switch (language)
                    {
                        case Language.FR: return "Français";
                        case Language.EN: return "English";
                    }
                    return null;
                }
                public static Language FullText(string language)
                {
                    switch (language)
                    {
                        case "Français": return Language.FR;
                        case "English": return Language.EN;
                    }
                    return Language.NULL;
                }

                public static string Abbreviation(Language language)
                {
                    switch (language)
                    {
                        case Language.FR: return "FR";
                        case Language.EN: return "EN";
                    }
                    return null;
                }
                public static Language Abbreviation(string language)
                {
                    if (language != null)
                        switch (language)
                        {
                            case "FR": return Language.FR;
                            case "EN": return Language.EN;
                        }
                    return Language.NULL;
                }
            }

            private static Language currentLanguage = Language.NULL;
            public static Language CurrentLanguage
            {
                get
                {
                    if (currentLanguage == Language.NULL)
                    {
                        string language = Settings.Get("Current Language");
                        CurrentLanguage = language != null ? LanguageExtensions.Abbreviation(language) : Language.FR;
                    }
                    return currentLanguage;
                }
                set
                {
                    if (value != Language.NULL)
                    {
                        currentLanguage = value;
                        foreach (Control control in Settings.Controls)
                        {
                            SetText(control);
                        }
                        Settings.Set("Current Language", LanguageExtensions.Abbreviation(value));
                    }
                }
            }
            public static void SetText(Control control) => control.Text = GetText($"{control.Name}.Text");
            public static string GetText(string name)
            {
                string text = Settings.Get(name, LanguageExtensions.File(CurrentLanguage));
                if (text == null)
                    Settings.Set(name, $"!!!{name} don't exist!!!", LanguageExtensions.File(CurrentLanguage));
                return text != null ? text : $"!!!{name} don't exist!!!";
            }
            public static void Seter(ComboBox comboBox)
            {
                var Items = comboBox.Items;
                comboBox.Font = font;
                foreach (Language l in LanguageExtensions.values())
                {
                    Items.Add(LanguageExtensions.FullText(l));
                }
                comboBox.SelectedItem = LanguageExtensions.FullText(CurrentLanguage);
                comboBox.SelectedIndexChanged += EventHandler;
            }
            private static void EventHandler(object sender, EventArgs e)
            {
                CurrentLanguage = LanguageExtensions.FullText((string)((ComboBox)sender).SelectedItem);
            }
        }
        private static Font font = new Font("Arial ", 14F, FontStyle.Bold, GraphicsUnit.Point);
        public static void AddControl(Control control)
        {
            if (Controls.Add(control))
            {
                if (control is CheckBox)
                {
                    CheckBox CheckBox = (CheckBox)control;
                    string check = Get($"{CheckBox.Name}.Checked");
                    if (check == null)
                        Set($"{CheckBox.Name}.Checked", "false");
                    CheckBox.Checked = check == "true";
                    CheckBox.CheckedChanged += EventHandler;
                }
                LanguageText.SetText(control);
                control.Font = font;
            }
        }

        private static void EventHandler(object sender, EventArgs e)
        {
            if (sender is CheckBox)
            {
                Set($"{((CheckBox)sender).Name}.Checked", ((CheckBox)sender).Checked ? "true" : "false");
            }
        }

        private static string Replace(string data) => data.Replace(">", "/</</").Replace("\r", @"\r").Replace("\n", @"\n").Replace("\t", @"\t");
        private static string Restore(string data) => data.Replace("/</</", ">").Replace(@"\r", "\r").Replace(@"\n", "\n").Replace(@"\t", "\t");
        private static bool FileExist(string file = "settings")
        {
            return File.Exists($@"Settings\{file}");
        }
        private static bool FileWriteAllLines(List<string> lines, string file = "settings")
        {
            try
            {
                if (!FileExist(file))
                {
                    string[] path = file.Split('\\');
                    string tempfile = "";
                    for(int i = 0; i < path.Length - 1; ++i)
                    {
                        tempfile += path[i] + '\\';
                    }
                    if (!Directory.Exists($@"Settings\{tempfile}"))
                        Directory.CreateDirectory($@"Settings\{tempfile}");
                }
                File.WriteAllLines($@"Settings\{file}", lines.ToArray());
                return true;
            }
            catch
            {
                return false;
            }
        }
        private static List<string> FileReadAllLines(string file = "settings")
        {
            if (FileExist(file))
                return new List<string>(File.ReadAllLines($@"Settings\{file}"));
            return new List<string>();
        }
        public static string Get(string data, string file = "settings")
        {
            if (data == null)
                return null;
            data = Replace(data);
            List<string> lines = FileReadAllLines(file);
            foreach (string line in lines.ToArray())
            {
                if (line.StartsWith($"{data}>"))
                {
                    string[] res = line.Substring($"{data}>".Length).Split('>');
                    return res.Length == 1 ? Restore(res[0]) : Restore(res[1]);
                }
            }
            return null;
        }
        public static string GetDefault(string data, string file = "settings")
        {
            if (data == null)
                return null;
            data = Replace(data);
            List<string> lines = FileReadAllLines(file);
            foreach (string line in lines.ToArray())
            {
                if (line.StartsWith($"{data}>"))
                {
                    string[] res = line.Substring($"{data}>".Length).Split('>');
                    return Restore(res[0]);
                }
            }
            return null;
        }
        public static bool SetDefault(string data, string value = null, string file = "settings")
        {
            if (data == null)
                return false;
            data = Replace(data);
            int i = 0;
            List<string> lines = FileReadAllLines(file);
            foreach (string line in lines.ToArray())
            {
                if (line.StartsWith($"{data}>"))
                {
                    string[] res = line.Substring($"{data}>".Length).Split('>');
                    if (value != null)
                        lines[i] = res.Length == 1 ? $"{data}>{Replace(value)}" : $"{data}>{Replace(value)}>{res[1]}";
                    else
                        lines.RemoveAt(i);
                    return FileWriteAllLines(lines, file);
                }
                ++i;
            }
            if (value != null) {
                lines.Add($"{data}>{Replace(value)}");
                return FileWriteAllLines(lines, file);
            }
            return false;
        }
        public static bool Set(string data, string value=null, string file = "settings")
        {
            if (data == null)
                return false;
            data = Replace(data);
            int i = 0;
            List<string> lines = FileReadAllLines(file);
            foreach (string line in lines.ToArray())
            {
                if (line.StartsWith($"{data}>"))
                {
                    string[] res = line.Substring($"{data}>".Length).Split('>');
                    if (value != null && res[0] != Replace(value))
                        lines[i] = $"{data}>{res[0]}>{Replace(value)}";
                    else
                        lines[i] = $"{data}>{res[0]}";
                    return FileWriteAllLines(lines, file);
                }
                ++i;
            }
            if (value != null)
            {
                lines.Add($"{data}>{Replace(value)}");
                return FileWriteAllLines(lines, file);
            }
            return false;
        }


        public static int GetInt(string data, string file = "settings") => Convert.ToInt32(Get(data, file), 16);
        public static bool SetInt(string data, int value, string file = "settings") => Set(data, Convert.ToString(value, 16), file);

        public static double GetDouble(string data, string file = "settings") => Convert.ToDouble(Get(data, file));
        public static bool SetDouble(string data, double value, string file = "settings") => Set(data, Convert.ToString(value), file);

        public static bool GetBool(string data, string file = "settings") => (Get(data, file) == "true");
        public static bool SetBool(string data, bool value, string file = "settings") => Set(data, value ? "true" : "false", file);
    }
}
