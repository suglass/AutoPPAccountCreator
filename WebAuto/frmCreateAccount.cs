using DbHelper;
using DbHelper.DbBase;
using DBHelper.DbBase;
using ResourcesInApp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebAuto.PayPal_Auto;

namespace WebAuto
{
    public partial class frmCreateAccount : Form
    {
        List<PaypalAccount> m_unreg_account_list;

        AutoPro g_process;
                
        bool m_must_close = true;

        private DataTable m_dt = null;
        public frmCreateAccount()
        {
            InitializeComponent();
        }

        private void frmCreateAccount_Load(object sender, EventArgs e)
        {
            refresh_account_list_view();            
        }
        

        private void btnCreate_Click(object sender, EventArgs e)
        {
            if (btnCreate.Text == "Create")
            {
//                 if (lvAccounts.SelectedItems.Count == 0)
//                 {
//                     MessageBox.Show("Please select the accounts to create.");
//                     return;
//                 }
// 
//                 DataTable sel_dt = TableSerialize<PaypalAccount>.create_empty_dt();
// 
//                 for (int i = 0; i < lvAccounts.SelectedItems.Count; i++)
//                 {
//                     sel_dt.Rows.Add(m_dt.Rows[lvAccounts.SelectedItems[i].Index]);
//                 }
                 //List<PaypalAccount> sel_accounts = TableSerialize<PaypalAccount>.dt_2_list(sel_dt);

                if (m_unreg_account_list == null || m_unreg_account_list.Count == 0)
                {
                    MessageBox.Show("No unregistered account info.");
                    return;
                }

                btnCreate.Text = "Cancel";

                new Thread((ThreadStart)(async () =>
                {
                    // To Do. 
                    update_status(0, "");
                    await Task.Delay(100);


                    List<ProxyServer> unused_proxy_list = new List<ProxyServer>();
                    unused_proxy_list = MainApp.g_db.get_unused_proxy_list();
                    if (unused_proxy_list == null || unused_proxy_list.Count == 0)
                    {
                        MessageBox.Show("No unused and active proxy.");
                        return;
                    }

                    if (unused_proxy_list.Count < m_unreg_account_list.Count)
                    {
                        MessageBox.Show("Active and Unused Proxy count is less than Accounts count. Load more proxies.");
                        return;
                    }

                    int m_last_used_proxy_idx = -1;

                    m_must_close = false;

                    for (int i = 0; i < m_unreg_account_list.Count;)
                    {
                        m_last_used_proxy_idx++;

                        if (m_last_used_proxy_idx >= unused_proxy_list.Count)
                        {
                            MainApp.log_info("Proxy list is not efficient.");
                            break;
                        }

                        WorkerParam param = new WorkerParam();
                        if (m_must_close)
                            break;

                        param.account = m_unreg_account_list[i];
                        param.proxy = unused_proxy_list[m_last_used_proxy_idx];

                        g_process = new AutoPro(param);


                        MainApp.log_info($"{g_process.m_param.proxy} and account {g_process.m_param.account.mail}");

                        bool success = await g_process.work_flow_create_Acc();
                        if (success)
                        {
                            MainApp.g_db.set_account_as_registered(param.account.id, ConstEnv.ACCOUNT_REGISTERED);
                            update_status(param.account.id, "success");

                            DateTime now = DateTime.Now;

                            MainApp.g_db.add_account_to_status_table(param.account.id, PaypalDbHelper.time_2_str(now));
                            MainApp.g_db.add_account_to_context_table(param.account.id, param.proxy.id, g_process.m_useragent_id, g_process.m_resolution);                            
                        }
                        else
                        {
                            if (g_process.m_proxy_dead_flag)
                            {
                                MainApp.g_db.set_proxy_as_dead(param.proxy.id);
                                await g_process.Quit();
                                continue;
                            }
                            update_status(param.account.id, "aborted");
                            MainApp.g_db.set_account_as_registered(param.account.id, ConstEnv.ACCOUNT_REGISTER_ABORTED);
                            
                            // To Do

                        }
                        i++;
                        if (g_process.m_process_result >= ConstEnv.CREATE_INPUT_SUCCESS)
                            MainApp.g_db.set_proxy_in_use(param.proxy.id);
                        //MainApp.g_db.add_cookies(param.account.id, g_process.m_lst_cookies);

                        //MainApp.g_db.set_account_proxy(param.account.id, param.proxy.id);
                        await g_process.Quit();
                    }
                    btnCreate.Text = "Create";
                })).Start();
                                
            }
            else
            {
                m_must_close = true;
                g_process.m_must_terminate = true;
                btnCreate.Text = "Create";
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        public void update_status(int account_id, string status)
        {
            this.InvokeOnUiThreadIfRequired(() =>
            {
                for (int i = 0; i < m_dt.Rows.Count; i++)
                {
                    DataRow row = m_dt.Rows[i];
                    int id = int.Parse(row["id"].ToString());
                    if (id == account_id)
                    {
                        lvAccounts.Items[i].SubItems[1].Text = status;
                        lvAccounts.Invalidate();
                        break;
                    }
                }
                lvAccounts.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            });
        }
        /// <summary>
        /// Refresh accounts list view control
        /// [NOTE] : This function MUST be called from Form_Load.
        /// </summary>
        private void refresh_account_list_view()
        {
            m_dt = MainApp.g_db.get_unregistered_account_table();
            if (m_dt == null || m_dt.Rows.Count == 0)
            {
                MessageBox.Show("There is no unregistered account.");
                this.Close();
                return;
            }
            m_unreg_account_list = TableSerialize<PaypalAccount>.dt_2_list(m_dt);
            if (m_unreg_account_list == null || m_unreg_account_list.Count == 0)
            {
                MessageBox.Show("There is no unregistered account.");
                this.Close();
                return;
            }

            lvAccounts.Columns.Add("No");
            lvAccounts.Columns.Add("Status");
            lvAccounts.Columns.Add("Account_ID");
            lvAccounts.Columns.Add("Mail");
            lvAccounts.Columns.Add("Mail Pass");
            lvAccounts.Columns.Add("PayPal Pass");

            int no = 0;
            foreach (PaypalAccount item in m_unreg_account_list)
            {
                ListViewItem lv1 = new ListViewItem((++no).ToString());
                lv1.SubItems.Add("ready");
                lv1.SubItems.Add(item.id.ToString());
                lv1.SubItems.Add(item.mail);
                lv1.SubItems.Add(item.mail_password);
                lv1.SubItems.Add(item.paypal_password);

                lvAccounts.Items.Add(lv1);
            }
            lvAccounts.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void lvAccounts_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (lvAccounts.FocusedItem != null && lvAccounts.FocusedItem.Bounds.Contains(e.Location) == true)
                {
                    ContextMenu m = new ContextMenu();
//                     MenuItem createMenuItem = new MenuItem("Manual Create");
//                     createMenuItem.Click += delegate (object sender2, EventArgs e2)
//                     {
//                         ActionClick(sender, e);
//                     };// your action here 
//                     m.MenuItems.Add(createMenuItem);

                    MenuItem infoMenuItem = new MenuItem("Get Info");
                    infoMenuItem.Click += delegate (object sender2, EventArgs e2)
                    {
                        InfoAction(sender, e);
                    };// your action here 
                    m.MenuItems.Add(infoMenuItem);

                    m.Show(lvAccounts, new Point(e.X, e.Y));
                }
            }
        }

//         private void ActionClick(object sender, MouseEventArgs e)
//         {
//             //id is extra value when you need or delete it
//             ListView ListViewControl = sender as ListView;
//             if (ListViewControl.SelectedItems.Count > 1)
//                 return;
//             foreach (ListViewItem tmpLstView in ListViewControl.SelectedItems)
//             {
//                 if (tmpLstView.SubItems[1].ToString() == "aborted")
//                 {
// 
//                 }
//                 Console.WriteLine(tmpLstView.SubItems[3]);
//             }
//         }
        private void InfoAction(object sender, MouseEventArgs e)
        {
            //id is extra value when you need or delete it
            ListView ListViewControl = sender as ListView;

            if (ListViewControl.SelectedItems.Count > 1)
                return;
            int account_id = int.Parse(ListViewControl.SelectedItems[0].SubItems[2].Text.ToString());
            //PaypalAccount account = MainApp.g_db.get_account_by_id(account_id);
            MessageBox.Show(MainApp.g_db.get_account_str_by_id(account_id));         
        }
    }
}
