﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WebAuto
{
    public partial class frmDBSetting : Form
    {
        public frmDBSetting()
        {
            InitializeComponent();
        }
        private void frmDBSetting_Load(object sender, EventArgs e)
        {
            txtDBName.Text = MainApp.g_setting.database_name;

            txtHostName.Text = MainApp.g_setting.db_hostname;
            txtPort.Text = MainApp.g_setting.db_port.ToString();
            txtDBUserName.Text = MainApp.g_setting.db_username;
            txtDBPassword.Text = MainApp.g_setting.db_password;

            txtSSHHostName.Text = MainApp.g_setting.db_ssh_hostname;
            txtSSHPort.Text = MainApp.g_setting.db_ssh_port.ToString();
            txtSSHUserName.Text = MainApp.g_setting.db_ssh_username;
            txtSSHPassword.Text = MainApp.g_setting.db_ssh_password;
            txtSSHKeyFile.Text = MainApp.g_setting.db_ssh_keyfile;

            chkSSH.Checked = MainApp.g_setting.db_use_ssh;

            if (txtSSHPort.Text == "")
                txtSSHPort.Text = "22";
        }

        private void chkSSH_CheckedChanged(object sender, EventArgs e)
        {
            txtSSHHostName.Enabled = chkSSH.Checked;
            txtSSHPort.Enabled = chkSSH.Checked;
            txtSSHUserName.Enabled = chkSSH.Checked;
            txtSSHPassword.Enabled = chkSSH.Checked;
            txtSSHKeyFile.Enabled = chkSSH.Checked;
            btnBrowseSSHKey.Enabled = chkSSH.Checked;
        }

        private void btnSetDB_Click(object sender, EventArgs e)
        {
            int port = 0;
            int ssh_port = 0;
            if (!int.TryParse(txtPort.Text, out port))
            {
                MessageBox.Show("Please input the valid port number.");
                return;
            }
            if (chkSSH.Checked && !int.TryParse(txtSSHPort.Text, out ssh_port))
            {
                MessageBox.Show("Please input the valid SSH port number.");
                return;
            }
            if (MainApp.g_setting.database_name == txtDBName.Text &&
                MainApp.g_setting.db_hostname == txtHostName.Text &&
                MainApp.g_setting.db_port == port &&
                MainApp.g_setting.db_username == txtDBUserName.Text &&
                MainApp.g_setting.db_password == txtDBPassword.Text &&
                MainApp.g_setting.db_use_ssh == chkSSH.Checked
                )
            {
                if (chkSSH.Checked == false ||
                    (MainApp.g_setting.db_ssh_hostname == txtSSHHostName.Text &&
                    MainApp.g_setting.db_ssh_port == ssh_port &&
                    MainApp.g_setting.db_ssh_username == txtSSHUserName.Text &&
                    MainApp.g_setting.db_ssh_password == txtSSHPassword.Text &&
                    MainApp.g_setting.db_ssh_keyfile == txtSSHKeyFile.Text
                    ))
                {
                    return;
                }
            }

            MainApp.g_setting.database_name = txtDBName.Text;

            MainApp.g_setting.db_hostname = txtHostName.Text;
            MainApp.g_setting.db_port = port;
            MainApp.g_setting.db_username = txtDBUserName.Text;
            MainApp.g_setting.db_password = txtDBPassword.Text;

            MainApp.g_setting.db_use_ssh = chkSSH.Checked;

            MainApp.g_setting.db_ssh_hostname = txtSSHHostName.Text;
            MainApp.g_setting.db_ssh_port = ssh_port;
            MainApp.g_setting.db_ssh_username = txtSSHUserName.Text;
            MainApp.g_setting.db_ssh_password = txtSSHPassword.Text;
            MainApp.g_setting.db_ssh_keyfile = txtSSHKeyFile.Text;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnBrowseSSHKey_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Open a file containing OpenSSH Key";
            dlg.Filter = "All files|*.*";
            dlg.RestoreDirectory = true;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                txtSSHKeyFile.Text = dlg.FileName;
            }
        }
    }
}
