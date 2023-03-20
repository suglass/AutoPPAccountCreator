namespace WebAuto
{
    partial class frmDBSetting
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.chkSSH = new System.Windows.Forms.CheckBox();
            this.txtSSHHostName = new System.Windows.Forms.TextBox();
            this.txtSSHPassword = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtSSHPort = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.txtSSHUserName = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txtDBPassword = new System.Windows.Forms.TextBox();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.txtDBName = new System.Windows.Forms.TextBox();
            this.txtDBUserName = new System.Windows.Forms.TextBox();
            this.txtHostName = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnSetDB = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.btnBrowseSSHKey = new System.Windows.Forms.Button();
            this.txtSSHKeyFile = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.pictureBox2);
            this.groupBox2.Controls.Add(this.groupBox3);
            this.groupBox2.Controls.Add(this.txtDBPassword);
            this.groupBox2.Controls.Add(this.txtPort);
            this.groupBox2.Controls.Add(this.txtDBName);
            this.groupBox2.Controls.Add(this.txtDBUserName);
            this.groupBox2.Controls.Add(this.txtHostName);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.btnSetDB);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.ForeColor = System.Drawing.SystemColors.Info;
            this.groupBox2.Location = new System.Drawing.Point(8, 8);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(660, 334);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "SQL DB Settings";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnBrowseSSHKey);
            this.groupBox3.Controls.Add(this.txtSSHKeyFile);
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Controls.Add(this.chkSSH);
            this.groupBox3.Controls.Add(this.txtSSHHostName);
            this.groupBox3.Controls.Add(this.txtSSHPassword);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.txtSSHPort);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.txtSSHUserName);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Location = new System.Drawing.Point(21, 30);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(609, 151);
            this.groupBox3.TabIndex = 10;
            this.groupBox3.TabStop = false;
            // 
            // chkSSH
            // 
            this.chkSSH.AutoSize = true;
            this.chkSSH.Location = new System.Drawing.Point(16, -1);
            this.chkSSH.Name = "chkSSH";
            this.chkSSH.Size = new System.Drawing.Size(83, 20);
            this.chkSSH.TabIndex = 0;
            this.chkSSH.Text = "Use SSH";
            this.chkSSH.UseVisualStyleBackColor = true;
            this.chkSSH.CheckedChanged += new System.EventHandler(this.chkSSH_CheckedChanged);
            // 
            // txtSSHHostName
            // 
            this.txtSSHHostName.Enabled = false;
            this.txtSSHHostName.Location = new System.Drawing.Point(119, 34);
            this.txtSSHHostName.Name = "txtSSHHostName";
            this.txtSSHHostName.Size = new System.Drawing.Size(348, 22);
            this.txtSSHHostName.TabIndex = 0;
            // 
            // txtSSHPassword
            // 
            this.txtSSHPassword.Enabled = false;
            this.txtSSHPassword.Location = new System.Drawing.Point(380, 71);
            this.txtSSHPassword.Name = "txtSSHPassword";
            this.txtSSHPassword.Size = new System.Drawing.Size(165, 22);
            this.txtSSHPassword.TabIndex = 3;
            this.txtSSHPassword.UseSystemPasswordChar = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.label3.Location = new System.Drawing.Point(23, 37);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(76, 16);
            this.label3.TabIndex = 6;
            this.label3.Text = "Host Name";
            // 
            // txtSSHPort
            // 
            this.txtSSHPort.Enabled = false;
            this.txtSSHPort.Location = new System.Drawing.Point(490, 34);
            this.txtSSHPort.Name = "txtSSHPort";
            this.txtSSHPort.Size = new System.Drawing.Size(55, 22);
            this.txtSSHPort.TabIndex = 1;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.label9.Location = new System.Drawing.Point(23, 73);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(77, 16);
            this.label9.TabIndex = 5;
            this.label9.Text = "User Name";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.label10.Location = new System.Drawing.Point(299, 72);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(68, 16);
            this.label10.TabIndex = 4;
            this.label10.Text = "Password";
            // 
            // txtSSHUserName
            // 
            this.txtSSHUserName.Enabled = false;
            this.txtSSHUserName.Location = new System.Drawing.Point(119, 71);
            this.txtSSHUserName.Name = "txtSSHUserName";
            this.txtSSHUserName.Size = new System.Drawing.Size(165, 22);
            this.txtSSHUserName.TabIndex = 2;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.label11.Location = new System.Drawing.Point(473, 37);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(11, 16);
            this.label11.TabIndex = 4;
            this.label11.Text = ":";
            // 
            // txtDBPassword
            // 
            this.txtDBPassword.Location = new System.Drawing.Point(368, 234);
            this.txtDBPassword.Name = "txtDBPassword";
            this.txtDBPassword.Size = new System.Drawing.Size(165, 22);
            this.txtDBPassword.TabIndex = 3;
            this.txtDBPassword.UseSystemPasswordChar = true;
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(478, 197);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(55, 22);
            this.txtPort.TabIndex = 1;
            // 
            // txtDBName
            // 
            this.txtDBName.Location = new System.Drawing.Point(107, 287);
            this.txtDBName.Name = "txtDBName";
            this.txtDBName.Size = new System.Drawing.Size(165, 22);
            this.txtDBName.TabIndex = 4;
            // 
            // txtDBUserName
            // 
            this.txtDBUserName.Location = new System.Drawing.Point(107, 234);
            this.txtDBUserName.Name = "txtDBUserName";
            this.txtDBUserName.Size = new System.Drawing.Size(165, 22);
            this.txtDBUserName.TabIndex = 2;
            // 
            // txtHostName
            // 
            this.txtHostName.Location = new System.Drawing.Point(107, 197);
            this.txtHostName.Name = "txtHostName";
            this.txtHostName.Size = new System.Drawing.Size(348, 22);
            this.txtHostName.TabIndex = 0;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.label7.Location = new System.Drawing.Point(461, 200);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(11, 16);
            this.label7.TabIndex = 4;
            this.label7.Text = ":";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.label12.Location = new System.Drawing.Point(18, 287);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(67, 16);
            this.label12.TabIndex = 5;
            this.label12.Text = "DB Name";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.label6.Location = new System.Drawing.Point(287, 235);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(68, 16);
            this.label6.TabIndex = 4;
            this.label6.Text = "Password";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.label5.Location = new System.Drawing.Point(18, 235);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 16);
            this.label5.TabIndex = 5;
            this.label5.Text = "User Name";
            // 
            // btnSetDB
            // 
            this.btnSetDB.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnSetDB.Location = new System.Drawing.Point(561, 279);
            this.btnSetDB.Name = "btnSetDB";
            this.btnSetDB.Size = new System.Drawing.Size(69, 39);
            this.btnSetDB.TabIndex = 5;
            this.btnSetDB.Text = "Set";
            this.btnSetDB.UseVisualStyleBackColor = true;
            this.btnSetDB.Click += new System.EventHandler(this.btnSetDB_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.label4.Location = new System.Drawing.Point(18, 199);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(76, 16);
            this.label4.TabIndex = 6;
            this.label4.Text = "Host Name";
            // 
            // pictureBox2
            // 
            this.pictureBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox2.Location = new System.Drawing.Point(15, 270);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(530, 1);
            this.pictureBox2.TabIndex = 11;
            this.pictureBox2.TabStop = false;
            // 
            // btnBrowseSSHKey
            // 
            this.btnBrowseSSHKey.Enabled = false;
            this.btnBrowseSSHKey.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnBrowseSSHKey.Location = new System.Drawing.Point(549, 106);
            this.btnBrowseSSHKey.Name = "btnBrowseSSHKey";
            this.btnBrowseSSHKey.Size = new System.Drawing.Size(26, 26);
            this.btnBrowseSSHKey.TabIndex = 8;
            this.btnBrowseSSHKey.Text = "...";
            this.btnBrowseSSHKey.UseVisualStyleBackColor = true;
            this.btnBrowseSSHKey.Click += new System.EventHandler(this.btnBrowseSSHKey_Click);
            // 
            // txtSSHKeyFile
            // 
            this.txtSSHKeyFile.Enabled = false;
            this.txtSSHKeyFile.Location = new System.Drawing.Point(117, 108);
            this.txtSSHKeyFile.Name = "txtSSHKeyFile";
            this.txtSSHKeyFile.Size = new System.Drawing.Size(426, 22);
            this.txtSSHKeyFile.TabIndex = 7;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.label13.Location = new System.Drawing.Point(20, 110);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(87, 16);
            this.label13.TabIndex = 9;
            this.label13.Text = "SSH Key File";
            // 
            // frmDBSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.ClientSize = new System.Drawing.Size(676, 350);
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmDBSetting";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MySQL Database Settings";
            this.Load += new System.EventHandler(this.frmDBSetting_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox chkSSH;
        private System.Windows.Forms.TextBox txtSSHHostName;
        private System.Windows.Forms.TextBox txtSSHPassword;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtSSHPort;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtSSHUserName;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtDBPassword;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.TextBox txtDBName;
        private System.Windows.Forms.TextBox txtDBUserName;
        private System.Windows.Forms.TextBox txtHostName;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnSetDB;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnBrowseSSHKey;
        private System.Windows.Forms.TextBox txtSSHKeyFile;
        private System.Windows.Forms.Label label13;
    }
}