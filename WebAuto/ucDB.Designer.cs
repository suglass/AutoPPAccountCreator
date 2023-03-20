using CustomControl;

namespace WebAuto
{
    partial class ucDB
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnImportUseragent = new System.Windows.Forms.Button();
            this.ImportProxy = new System.Windows.Forms.Button();
            this.btnImportAccount = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lblStatus = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.cboTable = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.picRefresh = new CustomControl.SimpleImageButton();
            this.picEdit = new CustomControl.SimpleImageButton();
            this.picDel = new CustomControl.SimpleImageButton();
            this.lvDBData = new System.Windows.Forms.ListView();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picRefresh)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDel)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 96.56203F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.groupBox2, 1, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 75F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(690, 556);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnImportUseragent);
            this.groupBox1.Controls.Add(this.ImportProxy);
            this.groupBox1.Controls.Add(this.btnImportAccount);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.ForeColor = System.Drawing.SystemColors.Info;
            this.groupBox1.Location = new System.Drawing.Point(15, 10);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(660, 65);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Import";
            // 
            // btnImportUseragent
            // 
            this.btnImportUseragent.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnImportUseragent.Location = new System.Drawing.Point(372, 24);
            this.btnImportUseragent.Name = "btnImportUseragent";
            this.btnImportUseragent.Size = new System.Drawing.Size(153, 30);
            this.btnImportUseragent.TabIndex = 2;
            this.btnImportUseragent.Text = "Import UserAgent...";
            this.btnImportUseragent.UseVisualStyleBackColor = true;
            this.btnImportUseragent.Click += new System.EventHandler(this.btnImportUseragent_Click);
            // 
            // ImportProxy
            // 
            this.ImportProxy.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ImportProxy.Location = new System.Drawing.Point(193, 24);
            this.ImportProxy.Name = "ImportProxy";
            this.ImportProxy.Size = new System.Drawing.Size(153, 30);
            this.ImportProxy.TabIndex = 1;
            this.ImportProxy.Text = "Import Proxy...";
            this.ImportProxy.UseVisualStyleBackColor = true;
            this.ImportProxy.Click += new System.EventHandler(this.ImportProxy_Click);
            // 
            // btnImportAccount
            // 
            this.btnImportAccount.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnImportAccount.Location = new System.Drawing.Point(17, 24);
            this.btnImportAccount.Name = "btnImportAccount";
            this.btnImportAccount.Size = new System.Drawing.Size(153, 30);
            this.btnImportAccount.TabIndex = 0;
            this.btnImportAccount.Text = "Import Account...";
            this.btnImportAccount.UseVisualStyleBackColor = true;
            this.btnImportAccount.Click += new System.EventHandler(this.btnImportAccount_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tableLayoutPanel2);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.ForeColor = System.Drawing.SystemColors.Info;
            this.groupBox2.Location = new System.Drawing.Point(15, 85);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(0, 5, 0, 10);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(10, 5, 10, 10);
            this.groupBox2.Size = new System.Drawing.Size(660, 461);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Manage";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.lvDBData, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(10, 20);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 88.40909F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(640, 431);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(this.panel4);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(640, 50);
            this.panel1.TabIndex = 0;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.lblStatus);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(277, 0);
            this.panel3.Name = "panel3";
            this.panel3.Padding = new System.Windows.Forms.Padding(5, 10, 0, 5);
            this.panel3.Size = new System.Drawing.Size(209, 50);
            this.panel3.TabIndex = 3;
            // 
            // lblStatus
            // 
            this.lblStatus.BackColor = System.Drawing.Color.Transparent;
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblStatus.Location = new System.Drawing.Point(5, 10);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(204, 35);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "Status";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.cboTable);
            this.panel4.Controls.Add(this.label4);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Padding = new System.Windows.Forms.Padding(5, 10, 0, 5);
            this.panel4.Size = new System.Drawing.Size(277, 50);
            this.panel4.TabIndex = 3;
            // 
            // cboTable
            // 
            this.cboTable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTable.FormattingEnabled = true;
            this.cboTable.Location = new System.Drawing.Point(63, 12);
            this.cboTable.Name = "cboTable";
            this.cboTable.Size = new System.Drawing.Size(196, 24);
            this.cboTable.TabIndex = 0;
            this.cboTable.SelectedIndexChanged += new System.EventHandler(this.cboTable_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.label4.Location = new System.Drawing.Point(10, 15);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 16);
            this.label4.TabIndex = 2;
            this.label4.Text = "Table";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.picRefresh);
            this.panel2.Controls.Add(this.picEdit);
            this.panel2.Controls.Add(this.picDel);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(486, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(154, 50);
            this.panel2.TabIndex = 1;
            // 
            // picRefresh
            // 
            this.picRefresh.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.picRefresh.Image = global::WebAuto.Properties.Resources.refreshallpivottable_32x32;
            this.picRefresh.Location = new System.Drawing.Point(14, 9);
            this.picRefresh.Name = "picRefresh";
            this.picRefresh.Size = new System.Drawing.Size(32, 32);
            this.picRefresh.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picRefresh.TabIndex = 0;
            this.picRefresh.TabStop = false;
            this.picRefresh.Click += new System.EventHandler(this.picRefresh_Click);
            // 
            // picEdit
            // 
            this.picEdit.BackColor = System.Drawing.Color.Transparent;
            this.picEdit.Image = global::WebAuto.Properties.Resources.ide_32x32;
            this.picEdit.Location = new System.Drawing.Point(60, 9);
            this.picEdit.Name = "picEdit";
            this.picEdit.Size = new System.Drawing.Size(32, 32);
            this.picEdit.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picEdit.TabIndex = 0;
            this.picEdit.TabStop = false;
            this.picEdit.Click += new System.EventHandler(this.picEdit_Click);
            // 
            // picDel
            // 
            this.picDel.BackColor = System.Drawing.Color.Transparent;
            this.picDel.Image = global::WebAuto.Properties.Resources.trash_32x32;
            this.picDel.Location = new System.Drawing.Point(106, 9);
            this.picDel.Name = "picDel";
            this.picDel.Size = new System.Drawing.Size(32, 32);
            this.picDel.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picDel.TabIndex = 0;
            this.picDel.TabStop = false;
            this.picDel.Click += new System.EventHandler(this.picDel_Click);
            // 
            // lvDBData
            // 
            this.lvDBData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvDBData.FullRowSelect = true;
            this.lvDBData.GridLines = true;
            this.lvDBData.Location = new System.Drawing.Point(0, 50);
            this.lvDBData.Margin = new System.Windows.Forms.Padding(0);
            this.lvDBData.Name = "lvDBData";
            this.lvDBData.Size = new System.Drawing.Size(640, 381);
            this.lvDBData.TabIndex = 0;
            this.lvDBData.UseCompatibleStateImageBehavior = false;
            this.lvDBData.View = System.Windows.Forms.View.Details;
            this.lvDBData.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lvDBData_MouseClick);
            // 
            // ucDB
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ucDB";
            this.Size = new System.Drawing.Size(690, 556);
            this.Load += new System.EventHandler(this.ucDB_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picRefresh)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDel)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnImportAccount;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button ImportProxy;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox cboTable;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel2;
        private SimpleImageButton picEdit;
        private SimpleImageButton picDel;
        private SimpleImageButton picRefresh;
        private System.Windows.Forms.ListView lvDBData;
        private System.Windows.Forms.Button btnImportUseragent;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label lblStatus;
    }
}
