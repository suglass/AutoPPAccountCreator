using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DbHelper.DbBase;
using System.IO;
using System.Globalization;
using ResourcesInApp;
using DBHelper.DbBase;
using WebAuto.Utils;
using System.Threading;
using WebAuto.PayPal_Auto;

namespace WebAuto
{
    public partial class ucDB : UserControl
    {
        public ucDB()
        {
            InitializeComponent();
            picRefresh.SetFocusBackgroundColor(Color.FromArgb(64, 64, 64));
            picEdit.SetFocusBackgroundColor(Color.FromArgb(64, 64, 64));
            picDel.SetFocusBackgroundColor(Color.FromArgb(64, 64, 64));
        }

        private void ucDB_Load(object sender, EventArgs e)
        {
            lvDBData.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            lvDBData.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

            List<string> table_names = MainApp.g_db.get_table_names();
            foreach (string name in table_names)
            {
                if (name != ConstEnv.to_do_table_name)
                    cboTable.Items.Add(name);
            }
            if (cboTable.Items.Count > 0)
                cboTable.SelectedIndex = 0;
        }
        private void load_db_table(string table_name)
        {
            lvDBData.Clear();
            lblStatus.Text = "";

            DataTable dt = MainApp.g_db.load_table(table_name);
            if (dt == null)
            {
                MessageBox.Show("No table.");
                return;
            }

            foreach (DataColumn col in dt.Columns)
                lvDBData.Columns.Add(col.ColumnName, col.ColumnName);

            foreach (DataRow row in dt.Rows)
            {
                ListViewItem lv1 = null;
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (i == 0)
                    {
                        lv1 = new ListViewItem(row[dt.Columns[i].ColumnName].ToString());
                        lv1.SubItems[0].Name = dt.Columns[i].ColumnName;
                    }
                    else
                    {
                        ListViewItem.ListViewSubItem subitem = lv1.SubItems.Add(row[dt.Columns[i].ColumnName].ToString());
                        subitem.Name = dt.Columns[i].ColumnName;
                    }

                    if (i == 0)
                        lv1.Tag = int.Parse(row[dt.Columns[i].ColumnName].ToString());
                }
                lvDBData.Items.Add(lv1);
            }
            lvDBData.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

            update_status(table_name);
        }
        private void update_status(string table_name)
        {
            if (table_name == ConstEnv.proxy_server_table_name)
            {
                int free_num = 0;

                foreach (ListViewItem item in lvDBData.Items)
                {
                    if (item.SubItems["in_use"].Text == ConstEnv.PROXY_FREE.ToString())
                        free_num++;
                }
                lblStatus.Text = $"Free Number : {free_num}";
            }
        }
        private void lvDBData_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && cboTable.Text == "account")
            {
                if (lvDBData.FocusedItem != null && lvDBData.FocusedItem.Bounds.Contains(e.Location) == true)
                {
                    ContextMenu m = new ContextMenu();

                    MenuItem loginMenuItem = new MenuItem("Manual Login");
                    loginMenuItem.Click += delegate (object sender2, EventArgs e2)
                    {
                        LoginAction(sender, e);
                    };// your action here 
                    m.MenuItems.Add(loginMenuItem);

                    m.Show(lvDBData, new Point(e.X, e.Y));
                }
            }
        }
        private void LoginAction(object sender, MouseEventArgs e)
        {
            //id is extra value when you need or delete it
            ListView ListViewControl = sender as ListView;

            if (ListViewControl.SelectedItems.Count > 1)
                return;

            int account_id = int.Parse(ListViewControl.SelectedItems[0].SubItems[0].Text.ToString());
            int is_registered = int.Parse(ListViewControl.SelectedItems[0].SubItems[14].Text.ToString());

            if (is_registered == ConstEnv.ACCOUNT_REGISTERED)
            {
                new Thread((ThreadStart)(async () =>
                {
                    await Task.Delay(100);
                    WorkerParam manual_param = new WorkerParam();
                    manual_param.account = MainApp.g_db.get_account_by_id(account_id);

                    int proxy_id = MainApp.g_db.get_account_proxy(account_id);
                    if (proxy_id == -1)
                        return;
                    manual_param.proxy = MainApp.g_db.get_proxy(proxy_id);

                    AutoPro manual_login_pro = new AutoPro(manual_param);

                    bool success = await manual_login_pro.work_flow_login();

                    if (success)
                        MessageBox.Show("Log in successful.");

                })).Start();
            }
            else if (is_registered == ConstEnv.ACCOUNT_BLOCKED)
                MessageBox.Show("This account is blocked.");
            else if (is_registered == ConstEnv.ACCOUNT_REGISTER_ABORTED)
                MessageBox.Show("This account is register aborted.");
            else
                MessageBox.Show("This account is unregistered.");
        }
        private void btnImportAccount_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Open a file containing accounts information";
            dlg.Filter = "CSV files|*.csv";
            dlg.RestoreDirectory = true;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    import_account_from_csv_file(dlg.FileName);
                    MessageBox.Show("Import account successfully.");
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Failed to import account.\n" + exception.Message);
                }

            }
        }
        private void import_account_from_csv_file(string csv_path)
        {
            DataTable dt = TableSerialize<PaypalAccount>.create_empty_dt();
            string[] lines = File.ReadAllLines(csv_path);
            Random random = new Random();

            foreach (string line in lines)
            {
                string[] vals = line.Split(',');
                if (vals.Length != 10)
                    throw new Exception("Invalid file format.");

                PaypalAccount account = MainApp.g_db.get_account_by_mail(vals[0]);
                if (account != null)
                {
                    throw new Exception(string.Format("{0} has already existed.", vals[0]));
                }

                DataRow row = dt.NewRow();
                int i = 0;

                row["money_balance"] = 0;
                row["mail"] = vals[i++];
                row["mail_password"] = vals[i++];
                row["paypal_password"] = vals[i++];
                row["first_name"] = vals[i++];
                row["last_name"] = vals[i++];
                row["birthday"] = vals[i++];
                row["street"] = vals[i++];
                row["city"] = vals[i++];
                row["postcode"] = vals[i++];
                row["tel"] = vals[i++];
                row["usergroup"] = MainApp.g_setting.group_name;
                row["country"] = "";
                row["is_registered"] = 0;
                row["transaction_count_for_preparing"] = random.Next(1, 5).ToString();
                row["level_2_amount"] = random.Next(500, 850).ToString(); ;

                dt.Rows.Add(row);
            }
            MainApp.g_db.insert_table(ConstEnv.paypal_accounts_table_name, dt);
            select_db_table(ConstEnv.paypal_accounts_table_name);
        }
        private void ImportProxy_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Open a file containing proxy information";
            dlg.Filter = "Text files|*.txt";
            dlg.RestoreDirectory = true;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    load_proxy_from_file(dlg.FileName);
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Failed to import proxy.\n" + exception.Message);
                }
            }
        }
        private void load_proxy_from_file(string txt_path)
        {
            DataTable dt = TableSerialize<ProxyServer>.create_empty_dt();
            string[] lines = File.ReadAllLines(txt_path);

            if (lines.Length == 0)
            {
                MessageBox.Show("No data in the file.");
                return;
            }

            try
            {
                foreach (string line in lines)
                {
                    string[] vals = line.Split(':');
                    if (vals.Length != 4)
                    {
                        vals = line.Split(',');
                        if (vals.Length != 4)
                            throw new Exception("Invalid file format.");
                    }

                    string url = vals[0];
                    url.IndexOf("aaa", StringComparison.InvariantCultureIgnoreCase);
                    int port = int.Parse(vals[1]);

                    if (MainApp.g_db.get_proxy_server_by_url_port(url, port) != null)
                        throw new Exception(string.Format("The proxy server has already existed.\nurl = {0}, port = {1}", url, port));
                }

            }
            catch (Exception exception)
            {
                throw new Exception(string.Format("Invalid file format : {0}", exception.Message));
            }

            frmImportProxy dlg = new frmImportProxy();
            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            foreach (string line in lines)
            {
                string[] vals = line.Split(':');
                if (vals.Length != 4)
                {
                    vals = line.Split(',');
                    if (vals.Length != 4)
                        throw new Exception("Invalid file format.");
                }

                DataRow row = dt.NewRow();
                int i = 0;

                row["proxy_group_id"] = dlg.new_group_id;
                row["url"] = vals[i++];
                row["port"] = vals[i++];
                row["username"] = vals[i++];
                row["password"] = vals[i++];

                string country, city, isp;
                ProxyInfo.get_proxy_info(vals[0], out country, out city, out isp);
                row["country"] = country;
                row["city"] = city;
                row["isp"] = isp;
                row["dead"] = 0;

                dt.Rows.Add(row);
            }
            MainApp.g_db.insert_table(ConstEnv.proxy_server_table_name, dt);
            select_db_table(ConstEnv.proxy_server_table_name);
        }

        private void cboTable_SelectedIndexChanged(object sender, EventArgs e)
        {
            load_db_table(cboTable.Text);
        }

        private void picRefresh_Click(object sender, EventArgs e)
        {
            load_db_table(cboTable.Text);
        }
        private void select_db_table(string table_name)
        {
            for (int i = 0; i < cboTable.Items.Count; i++)
            {
                if (cboTable.Items[i].ToString() == table_name)
                {
                    if (cboTable.SelectedIndex != i)
                        cboTable.SelectedIndex = i;
                    else
                        load_db_table(table_name); // to reload.
                    break;
                }
            }
        }

        private void picEdit_Click(object sender, EventArgs e)
        {
            if (lvDBData.SelectedItems.Count != 1)
            {
                MessageBox.Show("Please select one item to edit.");
                return;
            }
            frmEditDataRow dlg = new frmEditDataRow();
            dlg.table_name = cboTable.Text;
            dlg.id = (int)lvDBData.SelectedItems[0].Tag;
            dlg.ShowDialog();
            load_db_table(cboTable.Text);
        }

        private void picDel_Click(object sender, EventArgs e)
        {
            int sel_num = lvDBData.SelectedItems.Count;

            if (sel_num == 0)
            {
                MessageBox.Show("Please select the items to remove.");
                return;
            }
            try
            {
                if (MessageBox.Show($"Do you want delete {sel_num} items?", "Confirm", MessageBoxButtons.YesNo) != DialogResult.Yes)
                    return;
                for (int i = sel_num - 1; i >= 0; i--)
                {
                    int idx = lvDBData.SelectedItems[i].Index;
                    int id = (int)lvDBData.SelectedItems[i].Tag;

                    MainApp.g_db.delete($"DELETE FROM {cboTable.Text} WHERE id = {id} ;");

                    lvDBData.Items.RemoveAt(idx);
                }
                for (int i = 0; i < lvDBData.Items.Count; i++)
                    lvDBData.Items[i].SubItems[0].Text = (i + 1).ToString();

                MessageBox.Show($"Delete {sel_num} item(s) successfully.");
            }
            catch (Exception exception)
            {
                MainApp.log_error($"Exception Error ({System.Reflection.MethodBase.GetCurrentMethod().Name}): {exception.Message}");
                MessageBox.Show($"Failed to Delete item(s). {exception.Message}");
            }
        }
        private void btnImportUseragent_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Open a file containing useragent information";
            dlg.Filter = "CSV files|*.csv";
            dlg.RestoreDirectory = true;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    import_useragent_from_csv_file(dlg.FileName);
                    MessageBox.Show("Import useragent successfully.");
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Failed to import useragent.\n" + exception.Message);
                }

            }
        }
        private void import_useragent_from_csv_file(string csv_path)
        {
            DataTable dt = TableSerialize<UserAgent>.create_empty_dt();
            string[] lines = File.ReadAllLines(csv_path);

            foreach (string line in lines)
            {
                string[] vals = line.Split(',');
                if (vals.Length < 3)
                    throw new Exception("Invalid file format.");

                string system = vals[0];
                string browser = vals[1];
                string ua = line.Substring(system.Length + browser.Length + 2, line.Length - (system.Length + browser.Length + 2));
                ua = ua.Trim('\"');

                DataRow row = dt.NewRow();
                int i = 0;

                row["system"] = system;
                row["browser"] = browser;
                row["useragent"] = ua;

                dt.Rows.Add(row);
            }
            MainApp.g_db.insert_table(ConstEnv.useragent_table_name, dt);
            select_db_table(ConstEnv.useragent_table_name);
        }
    }
}
