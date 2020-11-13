using AnonymiserDiscordData.Tool;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static Rygone.Tool.Settings;

namespace AnonymiserDiscordData
{
    public partial class FormSettings : Form
    {
        private int Step = 0;
        public FormSettings()
        {
            InitializeComponent();
        }

        private CheckBox[] cbSettings;
        private Dictionary<CheckBox, CheckBox> cbDependency;

        private void FormSettings_Load(object sender, EventArgs e)
        {
            Point P = new Point(12, 12);
            Size S = new Size(1159, 499);
            info.Location = P;
            info.Size = S;
            settings.Location = P;
            settings.Size = S;
            loading.Location = P;
            loading.Size = S;
            end.Location = P;
            end.Size = S;
            err.Location = P;
            err.Size = S;
            UpdateView();

            AddControl(this);

            LanguageSeter(language);
            AddControl(next);

            AddControl(info1);
            AddControl(info2);
            AddControl(info3);
            AddControl(info4);
            AddControl(info5);

            AddControl(lblPath);
            path.Text = "";
            AddControl(found);
            cbSettings = new CheckBox[] {
                HideMessages,
                DeleteMessages,
                HideNicknames,
                HideServerIDs,
                HideServerNames,
                HideChannelIDs,
                HideChannelNames,
                HideApplication,
                DeleteApplication,
                HideOS,
                HideIPs,
                HideLocations,
                DeleteActivities,
            };
            foreach (CheckBox cb in cbSettings)
            {
                AddControl(cb);
                cb.CheckedChanged += SettingsEventHandler;
            }

            cbDependency = new Dictionary<CheckBox, CheckBox>
            {
                { cbSettings[0], cbSettings[1] },
                { cbSettings[3], cbSettings[4] },
                { cbSettings[5], cbSettings[6] },
                { cbSettings[7], cbSettings[8] },
                { cbSettings[9], cbSettings[12] },
                { cbSettings[10], cbSettings[12] },
                { cbSettings[11], cbSettings[12] }
            };

            AddControl(lblLoading);

            AddControl(end1);
            AddControl(end2);
            AddControl(end4);

            AddControl(err1);
            AddControl(err3);
        }
        private void SettingsEventHandler(object sender, EventArgs e)
        {
            if (!HasDefaultValue((CheckBox)sender) && GetDefaultChecked((CheckBox)sender))
            {
                if (MessageBox.Show(GetText("SureToChange"), GetText("PersonalData"),
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2)
                    != DialogResult.Yes)
                    ((CheckBox)sender).Checked = true;
            }
            if (((CheckBox)sender).Checked)
            {
                foreach (CheckBox key in cbDependency.Keys)
                    if (cbDependency[key] == sender)
                        key.Checked = true;
            }
            else if (cbDependency.ContainsKey((CheckBox)sender))
                cbDependency[(CheckBox)sender].Checked = false;
        }

        private void Next_Click(object sender, EventArgs e)
        {
            ++Step;
            UpdateView();
        }

        private void Info4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            const string url = "https://github.com/Rygone/AnonymiserDiscordData";
            try
            {
                System.Diagnostics.Process.Start(url);
            }
            catch
            {
                MessageBox.Show(GetText("UrlError"));
                Clipboard.SetText(url);
            }
        }

        private void End3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string url = ((Control)sender).Text;
            try
            {
                System.Diagnostics.Process.Start(url);
            }
            catch
            {
                MessageBox.Show(GetText("FileError"));
                Clipboard.SetText(url);
            }
        }
        private void Found_Click(object sender, EventArgs e)
        {
            next.Enabled = false;
            found.Enabled = false;

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "zip files (*.zip)|*.zip",
                FilterIndex = 1
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
                path.Text = openFileDialog.FileName;

            next.Enabled = true;
            found.Enabled = true;
        }

        private string value = null;

        private void UpdateView()
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
                        err.Visible = false;
                        settings.Visible = true;
                    }
                    break;
                case 2:
                    {
                        if (path.Text.EndsWith(".zip") && System.IO.File.Exists(path.Text))
                        {
                            settings.Visible = false;
                            loading.Visible = true;
                            next.Enabled = false;
                            language.Enabled = false;
                            Data.Start(
                                path.Text,
                                this,
                                (value) => { pbLoading.Maximum = value; },
                                (value) => { pbLoading.Value = value; },
                                (value) => { lblLoading.Text = GetText(value); },
                                (value) => { Step += 1; this.value = value; UpdateView(); },
                                (value) => { Step += 3; this.value = value;  UpdateView(); },
                                HideMessages.Checked,
                                DeleteMessages.Checked,
                                HideNicknames.Checked,
                                HideServerIDs.Checked,
                                HideServerNames.Checked,
                                HideChannelIDs.Checked,
                                HideChannelNames.Checked,
                                HideApplication.Checked,
                                DeleteApplication.Checked,
                                HideOS.Checked,
                                HideIPs.Checked,
                                HideLocations.Checked,
                                DeleteActivities.Checked
                            );
                        }
                        else
                        {
                            --Step;
                            Found_Click(null, null);
                        }
                    }
                    break;
                case 3:
                    {
                        next.Enabled = true;
                        language.Enabled = true;
                        loading.Visible = false;
                        end3.Text = value;
                        end.Visible = true;
                    }
                    break;
                case 4:
                    Close();
                    break;
                case 5:
                    {
                        next.Enabled = true;
                        language.Enabled = true;
                        loading.Visible = false;
                        err.Visible = true;
                        err2.Text = GetText(value);
                        Step = 0;
                    }
                    break;
            }
        }

    }
}
