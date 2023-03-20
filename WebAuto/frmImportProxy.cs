using DbHelper;
using DBHelper.DbBase;
using ResourcesInApp;
using System;
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
    public partial class frmImportProxy : Form
    {
        public int new_group_id;
        public frmImportProxy()
        {
            InitializeComponent();
            new_group_id = -1;
        }

        private void chkExpired_CheckedChanged(object sender, EventArgs e)
        {
            timeExpired.Enabled = chkExpired.Checked;
        }

        private void btnSet_Click(object sender, EventArgs e)
        {
            ProxyGroup group = null;
            bool already_exist = false;

            if (txtName.Text == "")
            {
                MessageBox.Show("Please input the name");
                txtName.Focus();
                return;
            }
            if (chkExpired.Checked)
            {
                DateTime expired = new DateTime(timeExpired.Value.Year, timeExpired.Value.Month, timeExpired.Value.Day, 0, 0, 0);
                DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                if (expired < today)
                {
                    MessageBox.Show("The expired day is earlier than today.");
                    return;
                }
            }
            group = MainApp.g_db.get_proxy_group_by_name(txtName.Text);
            if (group != null)
            {
                already_exist = true;
                if (MessageBox.Show(string.Format("The {0} proxy group has already existed.\nDo you want overwrite it?\n\n[NOTE]It may happens a unexpected problem.", txtName.Text), "Question", MessageBoxButtons.YesNo) != DialogResult.Yes)
                    return;
            }
            else
            {
                group = new ProxyGroup();

                group.name = txtName.Text;
            }
            group.type = (rdoHttp.Checked) ? ConstEnv.PROXY_TYPE_HTTP : ((rdoSocks4.Checked) ? ConstEnv.PROXY_TYPE_SOCKS4 : ConstEnv.PROXY_TYPE_SOCKS5);
            group.expired_date = (chkExpired.Checked) ? PaypalDbHelper.time_2_str(timeExpired.Value) : ConstEnv.INVALID_TIME_STR;
            group.seller_url = txtUrl.Text;
            group.seller_user = txtUserName.Text;
            group.seller_password = txtPassword.Text;

            try
            {
                MainApp.g_db.add_proxy_group(group, !already_exist);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Failed to add new group.\n" + exception.Message);
                return;
            }

            new_group_id = MainApp.g_db.get_proxy_group_by_name(group.name).id;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void frmImportProxy_Load(object sender, EventArgs e)
        {
            txtName.Text = "host1plus server";
            txtUserName.Text = "aaa";
            txtPassword.Text = "aaa";
            txtUrl.Text = "www.host1plus.com";
            chkExpired.Checked = true;
            timeExpired.Value = new DateTime(2019, 8, 20);
        }
    }
}
