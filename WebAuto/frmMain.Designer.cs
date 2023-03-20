using CustomControl;

namespace WebAuto
{
    partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.panel1 = new System.Windows.Forms.Panel();
            this.panelMain = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lblInstantLog = new System.Windows.Forms.Label();
            this.btnDB = new CustomControl.PushImageButton();
            this.btnAuto = new CustomControl.PushImageButton();
            this.btnLog = new CustomControl.PushImageButton();
            this.btnSetting = new CustomControl.PushImageButton();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnDB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnAuto)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnLog)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSetting)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.btnDB);
            this.panel1.Controls.Add(this.btnAuto);
            this.panel1.Controls.Add(this.btnLog);
            this.panel1.Controls.Add(this.btnSetting);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(187, 556);
            this.panel1.TabIndex = 0;
            // 
            // panelMain
            // 
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(187, 0);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(690, 556);
            this.panelMain.TabIndex = 1;
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.lblInstantLog);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 556);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(5);
            this.panel2.Size = new System.Drawing.Size(877, 33);
            this.panel2.TabIndex = 0;
            // 
            // lblInstantLog
            // 
            this.lblInstantLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblInstantLog.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInstantLog.ForeColor = System.Drawing.SystemColors.Info;
            this.lblInstantLog.Location = new System.Drawing.Point(5, 5);
            this.lblInstantLog.Margin = new System.Windows.Forms.Padding(3);
            this.lblInstantLog.Name = "lblInstantLog";
            this.lblInstantLog.Size = new System.Drawing.Size(865, 21);
            this.lblInstantLog.TabIndex = 0;
            this.lblInstantLog.Text = "Ready...";
            // 
            // btnDB
            // 
            this.btnDB.Image = global::WebAuto.Properties.Resources.db;
            this.btnDB.Location = new System.Drawing.Point(49, 171);
            this.btnDB.Name = "btnDB";
            this.btnDB.Pushed = false;
            this.btnDB.Size = new System.Drawing.Size(90, 90);
            this.btnDB.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.btnDB.TabIndex = 0;
            this.btnDB.TabStop = false;
            this.btnDB.Click += new System.EventHandler(this.btnDB_Click);
            // 
            // btnAuto
            // 
            this.btnAuto.Image = global::WebAuto.Properties.Resources.auto;
            this.btnAuto.Location = new System.Drawing.Point(49, 42);
            this.btnAuto.Name = "btnAuto";
            this.btnAuto.Pushed = true;
            this.btnAuto.Size = new System.Drawing.Size(90, 90);
            this.btnAuto.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.btnAuto.TabIndex = 0;
            this.btnAuto.TabStop = false;
            this.btnAuto.Click += new System.EventHandler(this.btnAuto_Click);
            // 
            // btnLog
            // 
            this.btnLog.Image = global::WebAuto.Properties.Resources.log;
            this.btnLog.Location = new System.Drawing.Point(49, 416);
            this.btnLog.Name = "btnLog";
            this.btnLog.Pushed = false;
            this.btnLog.Size = new System.Drawing.Size(90, 90);
            this.btnLog.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.btnLog.TabIndex = 1;
            this.btnLog.TabStop = false;
            this.btnLog.Click += new System.EventHandler(this.btnLog_Click);
            // 
            // btnSetting
            // 
            this.btnSetting.Image = global::WebAuto.Properties.Resources.setting;
            this.btnSetting.Location = new System.Drawing.Point(49, 295);
            this.btnSetting.Name = "btnSetting";
            this.btnSetting.Pushed = false;
            this.btnSetting.Size = new System.Drawing.Size(90, 90);
            this.btnSetting.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.btnSetting.TabIndex = 1;
            this.btnSetting.TabStop = false;
            this.btnSetting.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.ClientSize = new System.Drawing.Size(877, 589);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(893, 595);
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Web Automation Tool";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.btnDB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnAuto)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnLog)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSetting)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lblInstantLog;
        private PushImageButton btnSetting;
        private PushImageButton btnAuto;
        private PushImageButton btnDB;
        private PushImageButton btnLog;
    }
}

