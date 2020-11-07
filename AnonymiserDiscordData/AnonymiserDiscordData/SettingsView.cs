using System;
using System.Drawing;
using System.Windows.Forms;
using static AnonymiserDiscordData.Util.Settings;
using static AnonymiserDiscordData.Util.Settings.LanguageText;
using static AnonymiserDiscordData.Util.Settings.LanguageText.LanguageExtensions;

namespace AnonymiserDiscordData
{
    public partial class formSettings : Form
    {
        private int Step = 0;
        public formSettings()
        {
            InitializeComponent();
        }

        private void formSettings_Load(object sender, EventArgs e)
        {
            Point P = new Point(12, 12);
            info.Location = P;
            settings.Location = P;
            Update();

            Seter(language);
            AddControl(next);

            AddControl(info1);
            AddControl(info2);
            AddControl(info3);
            AddControl(info4);
            AddControl(info5);

            AddControl(lblPath);
            AddControl(path);
            AddControl(found);
            AddControl(settings1);
            AddControl(settings2);
            AddControl(settings3);
            AddControl(settings4);
            AddControl(settings5);
            AddControl(settings6);
            AddControl(settings7);
        }

        private void next_Click(object sender, EventArgs e)
        {
            ++Step;
            Update();
        }
        private void info4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            const string url = "https://github.com/Rygone/AnonymiserDiscordData";
            try
            {
                System.Diagnostics.Process.Start(url);
            }
            catch
            {
                //"Impossible de lancer navigateur internet,\r\nURL copier dans le presse papier"
                MessageBox.Show(GetText("UrlError"));
                Clipboard.SetText(url);
            }
        }
        private void found_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "zip files (*.zip)|*.zip";
                openFileDialog.FilterIndex = 1;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                    path.Text = openFileDialog.FileName;
            }
        }

        private void Update()
        {
            switch (Step)
            {
                case 0:
                    {
                        info.Visible = true;
                    }
                    break;
                case 1:
                    {
                        info.Visible = false;
                        settings.Visible = true;
                    }
                    break;
                case 2:
                    {
                        if (path.Text.EndsWith(".zip") && System.IO.File.Exists(path.Text))
                        {
                            settings.Visible = false;
                        }
                    }
                    break;
            }
        }
    }
}
