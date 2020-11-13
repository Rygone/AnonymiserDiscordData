using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Rygone.Tool
{
    public static class Settings
    {
        public static HashSet<Control> Controls = new HashSet<Control>();

        #region Base 
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
                    {
                        value = Replace(value);
                        if (res[0] == value)
                            return false;
                        else
                        {
                            if (res.Length == 1)
                                lines[i] = $"{data}>{value}";
                            else if(res[1] == value)
                            {
                                lines[i] = $"{data}>{value}";
                                FileWriteAllLines(lines, file);
                                return false;
                            }
                            else
                                lines[i] = $"{data}>{value}>{res[1]}";
                        }
                    }
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
                    if (value != null)
                    {
                        value = Replace(value);
                        if (res[0] == value)
                        {
                            if (res.Length == 1)
                                return false;
                            else
                                lines[i] = $"{data}>{res[0]}";
                        }
                        else
                        {
                            if (res.Length == 1 || res[1] != value)
                                lines[i] = $"{data}>{res[0]}>{value}";
                            else
                                return false;
                        }
                    }
                    else
                    {
                        if (res.Length == 1)
                            return false;
                        else
                            lines[i] = $"{data}>{res[0]}";
                    }
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
        #endregion

        #region Dot
        private static string Value(Control control)
        {
            if (control is CheckBox)
                return Checked(control);
            else
                return Text(control);
        }
        
        private static string Text(Control control) => $"{control.Name}.Text";
        private static string Checked(Control control) => $"{control.Name}.Checked";
        #endregion

        #region Value
        public static bool HasDefaultValue(Control control)
        {
            return control is CheckBox box ? HasDefaultChecked(box) : HasDefaultText(control);
        }
        #endregion

        #region Text

        #region Language
        public static void LanguageSeter(ComboBox comboBox)
        {
            var Items = comboBox.Items;
            comboBox.Font = font;
            foreach (Language l in LanguageExtensions.Values())
            {
                Items.Add(LanguageExtensions.FullText(l));
            }
            comboBox.SelectedItem = LanguageExtensions.FullText(CurrentLanguage);
            comboBox.SelectedIndexChanged += EventHandler;
        }
        private static void EventHandler(object sender, EventArgs e)
            => CurrentLanguage = LanguageExtensions.FullText((string)((ComboBox)sender).SelectedItem);
        public enum Language
        {
            NULL, FR, EN
        }
        public static class LanguageExtensions
        {
            public static Language[] Values()
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
                    string language = Get("Current Language");
                    CurrentLanguage = language != null ? LanguageExtensions.Abbreviation(language) : Language.FR;
                }
                return currentLanguage;
            }
            set
            {
                if (value != Language.NULL)
                {
                    currentLanguage = value;
                    foreach (Control control in Controls)
                    {
                        UpdateText(control);
                    }
                    Set("Current Language", LanguageExtensions.Abbreviation(value));
                }
            }
        }
        public static void UpdateText(Control control)
            => control.Text = GetText(control);
        #endregion

        private static readonly Font font = new Font("Arial ", 14F, FontStyle.Bold, GraphicsUnit.Point);

        public static bool SetText(Control control, string text)
        {
            if (Set(Text(control), text, LanguageExtensions.File(CurrentLanguage)))
            {
                UpdateText(control);
                return true;
            }
            return false;
        }
        public static bool SetDefaultText(Control control, string text)
        {
            if (SetDefault(Text(control), text, LanguageExtensions.File(CurrentLanguage)))
            {
                UpdateText(control);
                return true;
            }
            return false;
        }
        public static string GetText(Control control)
            => GetText(Text(control));
        public static string GetText(string name)
        {
            string text = Get(name, LanguageExtensions.File(CurrentLanguage));
            if (text == null)
                Set(name, $"!!!{name}!!!", LanguageExtensions.File(CurrentLanguage));
            return text ?? $"!!!{name}-!!!";
        }
        public static string GetDefaultText(Control control)
            => GetDefaultText(Text(control));
        public static string GetDefaultText(string name)
            => GetDefault(name, LanguageExtensions.File(CurrentLanguage));

        public static bool HasDefaultText(Control control)
            => control.Text == GetDefault(Text(control), LanguageExtensions.File(CurrentLanguage));
        #endregion

        #region Checked
        public static bool SetChecked(CheckBox control, bool set)
        {
            if (SetBool(Checked(control), set))
            {
                control.Checked = set;
                return true;
            }
            return false;
        }
        public static bool SetDefaultChecked(CheckBox control, bool set)
        {
            if (SetDefaultBool(Checked(control), set))
            {
                control.Checked = set;
                return true;
            }
            return false;
        }
        public static bool GetChecked(CheckBox control)
            => GetBool(Checked(control));
        public static bool GetDefaultChecked(CheckBox control)
            => GetDefaultBool(Checked(control));

        public static bool HasDefaultChecked(CheckBox control)
            => control is CheckBox && control.Checked == GetDefaultBool(Checked(control));
        #endregion

        public static void AddControl(Control control)
        {
            if (Controls.Add(control))
            {
                if (control is CheckBox box)
                {
                    string check = Get(Checked(control));
                    if (check == null)
                        Set(Checked(control), "false");
                    box.Checked = check == "true";
                    box.CheckedChanged += CheckBoxEventHandler;
                }
                UpdateText(control);
                control.Font = font;
            }
        }
        private static void CheckBoxEventHandler(object sender, EventArgs e)
        {
            if (sender is CheckBox box)
                SetChecked(box, box.Checked);
        }

        #region Type
        public static int GetInt(string data, string file = "settings") => Convert.ToInt32(Get(data, file), 16);
        public static int GetDefaultInt(string data, string file = "settings") => Convert.ToInt32(GetDefault(data, file), 16);
        public static bool SetInt(string data, int value, string file = "settings") => Set(data, Convert.ToString(value, 16), file);
        public static bool SetDefaultInt(string data, int value, string file = "settings") => SetDefault(data, Convert.ToString(value, 16), file);

        public static double GetDouble(string data, string file = "settings") => Convert.ToDouble(Get(data, file));
        public static double GetDefaultDouble(string data, string file = "settings") => Convert.ToDouble(GetDefault(data, file));
        public static bool SetDouble(string data, double value, string file = "settings") => Set(data, Convert.ToString(value), file);
        public static bool SetDefaultDouble(string data, double value, string file = "settings") => SetDefault(data, Convert.ToString(value), file);

        public static bool GetBool(string data, string file = "settings") => (Get(data, file) == "true");
        public static bool GetDefaultBool(string data, string file = "settings") => (GetDefault(data, file) == "true");
        public static bool SetBool(string data, bool value, string file = "settings") => Set(data, value ? "true" : "false", file);
        public static bool SetDefaultBool(string data, bool value, string file = "settings") => SetDefault(data, value ? "true" : "false", file);
        #endregion
    }
}
