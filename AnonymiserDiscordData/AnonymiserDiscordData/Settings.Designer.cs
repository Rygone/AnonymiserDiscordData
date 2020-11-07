namespace AnonymiserDiscordData
{
    partial class formSettings
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.next = new System.Windows.Forms.Button();
            this.info = new System.Windows.Forms.Panel();
            this.info1 = new System.Windows.Forms.Label();
            this.settings = new System.Windows.Forms.Panel();
            this.info2 = new System.Windows.Forms.Label();
            this.info3 = new System.Windows.Forms.Label();
            this.info5 = new System.Windows.Forms.Label();
            this.info4 = new System.Windows.Forms.LinkLabel();
            this.info.SuspendLayout();
            this.SuspendLayout();
            // 
            // next
            // 
            this.next.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.next.BackColor = System.Drawing.SystemColors.ControlDark;
            this.next.Location = new System.Drawing.Point(1021, 518);
            this.next.Margin = new System.Windows.Forms.Padding(4);
            this.next.Name = "next";
            this.next.Size = new System.Drawing.Size(150, 30);
            this.next.TabIndex = 0;
            this.next.UseVisualStyleBackColor = false;
            // 
            // info
            // 
            this.info.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.info.BackColor = System.Drawing.SystemColors.ControlDark;
            this.info.Controls.Add(this.info4);
            this.info.Controls.Add(this.info5);
            this.info.Controls.Add(this.info3);
            this.info.Controls.Add(this.info2);
            this.info.Controls.Add(this.info1);
            this.info.Location = new System.Drawing.Point(12, 12);
            this.info.Name = "info";
            this.info.Size = new System.Drawing.Size(1159, 499);
            this.info.TabIndex = 1;
            // 
            // info1
            // 
            this.info1.AutoSize = true;
            this.info1.Location = new System.Drawing.Point(40, 40);
            this.info1.Name = "info1";
            this.info1.Size = new System.Drawing.Size(49, 21);
            this.info1.TabIndex = 0;
            this.info1.Text = "Hello,";
            // 
            // settings
            // 
            this.settings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.settings.BackColor = System.Drawing.SystemColors.ControlDark;
            this.settings.Location = new System.Drawing.Point(1177, 226);
            this.settings.Name = "settings";
            this.settings.Size = new System.Drawing.Size(1159, 499);
            this.settings.TabIndex = 1;
            // 
            // info2
            // 
            this.info2.AutoSize = true;
            this.info2.Location = new System.Drawing.Point(40, 70);
            this.info2.Name = "info2";
            this.info2.Size = new System.Drawing.Size(538, 21);
            this.info2.TabIndex = 0;
            this.info2.Text = "J’ai décidé de faire un petit programme pour anonymisée le donnée Discord.";
            // 
            // info3
            // 
            this.info3.AutoSize = true;
            this.info3.Location = new System.Drawing.Point(40, 110);
            this.info3.Name = "info3";
            this.info3.Size = new System.Drawing.Size(275, 21);
            this.info3.TabIndex = 0;
            this.info3.Text = "Je le l’ai publié avec le code source ici :";
            // 
            // info5
            // 
            this.info5.AutoSize = true;
            this.info5.Location = new System.Drawing.Point(40, 190);
            this.info5.Name = "info5";
            this.info5.Size = new System.Drawing.Size(448, 21);
            this.info5.TabIndex = 0;
            this.info5.Text = "Si vous avez des questions MP moi sur Discord. (NEYGO#5944)";
            // 
            // info4
            // 
            this.info4.AutoSize = true;
            this.info4.Location = new System.Drawing.Point(90, 150);
            this.info4.Name = "info4";
            this.info4.Size = new System.Drawing.Size(81, 21);
            this.info4.TabIndex = 1;
            this.info4.TabStop = true;
            this.info4.Text = "linkLabel1";
            // 
            // formSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(1184, 561);
            this.Controls.Add(this.settings);
            this.Controls.Add(this.info);
            this.Controls.Add(this.next);
            this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "formSettings";
            this.info.ResumeLayout(false);
            this.info.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button next;
        private System.Windows.Forms.Panel info;
        private System.Windows.Forms.Label info1;
        private System.Windows.Forms.Panel settings;
        private System.Windows.Forms.LinkLabel info4;
        private System.Windows.Forms.Label info5;
        private System.Windows.Forms.Label info3;
        private System.Windows.Forms.Label info2;
    }
}

