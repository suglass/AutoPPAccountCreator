using DbHelper.DbBase;
using DBHelper.DbBase;
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
using WebAuto.Utils;

namespace WebAuto
{
    public partial class frmLevel1 : Form
    {
        private DataTable m_dt = null;
        private List<PaypalAccount> m_level_0_account_list;
        AutoPro m_level_1_process;
        public frmLevel1()
        {
            InitializeComponent();
        }
        private void frmLevel1_Load(object sender, EventArgs e)
        {
            refresh_account_list_view();

            if (lvAccounts.Items.Count == 0)
            {
                MessageBox.Show("There is no registered account.");
                this.Close();
                return;
            }
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            if (m_level_0_account_list == null || m_level_0_account_list.Count == 0)
            {
                MessageBox.Show("There is no accounts to go to level 1.");
                return;
            }

            PaypalAccount sender_account;
            PaypalAccount receiver_account = null;

            if (btnCreate.Text == "Start")
            {
                btnCreate.Text = "Cancel";
                new Thread((ThreadStart)(async () =>
                {
                    await Task.Delay(100);

                    while (true)
                    {
                        if (check_level_0_accounts_trans_count())
                        {
                            MessageBox.Show("All transaction count conditions are OK.");
                            break;
                        }

                        sender_account = get_sender_account(receiver_account);

                        if (sender_account == null)
                        {
                            MessageBox.Show("You have to load money.");
                            break;
                        }

                        receiver_account = get_receive_account(sender_account);

                        if (receiver_account == null)
                        {
                            MessageBox.Show("Can not find receiver account.");
                            break;
                        }
                        
                        WorkerParam level_1_param = new WorkerParam();

                        level_1_param.account = sender_account;

                        int proxy_id = MainApp.g_db.get_account_proxy(level_1_param.account.id);
                        if (proxy_id == -1)
                            return;
                        level_1_param.proxy = MainApp.g_db.get_proxy(proxy_id);

                        m_level_1_process = new AutoPro(level_1_param);

                        bool success = await m_level_1_process.work_flow_login();

                        if (success)
                        {
                            if (m_level_1_process.m_real_money_balance >= MainApp.g_setting.level_1_min_send_amount)
                            {
                                string amount = Str_Utils.GetSendAmount(m_level_1_process.m_real_money_balance - 1);
                                if (await m_level_1_process.Send_Money(receiver_account, amount))
                                {
                                    DateTime now = DateTime.Now;
                                    MainApp.g_db.add_transaction_history(sender_account.id, receiver_account.id, (int)decimal.Parse(amount), now, m_level_1_process.m_sender_transaction_id);
                                    MainApp.g_db.set_real_money_balance(sender_account.id, m_level_1_process.m_real_money_balance - (int)m_level_1_process.m_real_money_balance + 1);


                                    double real_send_amount = get_real_amount((int)m_level_1_process.m_real_money_balance - 1);
                                    MainApp.g_db.set_real_money_balance(receiver_account.id, receiver_account.money_balance + real_send_amount);
                                }
                            }                            
                        }
                        else
                            MainApp.log_error($"{level_1_param.account.mail} - Login failed.");

                        await m_level_1_process.Quit();

                        m_level_0_account_list = get_level_0_account_list();
                        if (m_level_0_account_list == null || m_level_0_account_list.Count == 0)
                            break;
                    }
                       
                    btnCreate.Text = "Start";
                })).Start();
            }
            else
            {
                m_level_1_process.m_must_terminate = true;
                btnCreate.Text = "Start";
            }
        }
        public void update_status(PaypalAccount sender, PaypalAccount receiver)
        {
            this.InvokeOnUiThreadIfRequired(() =>
            {
                for (int j = 0; j < lvAccounts.Items.Count; j ++)
                {
                    int i = -1;    
                    if (lvAccounts.Items[j].SubItems[2].Text == sender.mail)
                    {
                        for (i = 5; i < 10; i++)
                            if (lvAccounts.Items[j].SubItems[i].Text == String.Empty)
                            {
                                lvAccounts.Items[j].SubItems[i].Text = receiver.mail;
                                lvAccounts.Items[j].BackColor = Color.LightBlue;
                                break;
                            }
                        lvAccounts.Items[j].UseItemStyleForSubItems = false;
                        continue;
                    }

                    if (lvAccounts.Items[j].SubItems[2].Text == receiver.mail)
                    {
                        for (i = 5; i < 10; i++)
                            if (lvAccounts.Items[j].SubItems[i].Text == String.Empty)
                            {
                                lvAccounts.Items[j].SubItems[i].Text = sender.mail;
                                lvAccounts.Items[j].BackColor = Color.LightGreen;
                                break;
                            }
                        lvAccounts.Items[j].UseItemStyleForSubItems = false;
                        continue;
                    }                   
                }
                lvAccounts.Invalidate();
                lvAccounts.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            });
            
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
/*
        private void lvAccounts_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (lvAccounts.FocusedItem != null && lvAccounts.FocusedItem.Bounds.Contains(e.Location) == true)
                {
                    ContextMenu m = new ContextMenu();


                    MenuItem infoMenuItem = new MenuItem("Get Info");
                    infoMenuItem.Click += delegate (object sender2, EventArgs e2)
                    {
                        InfoAction(sender, e);
                    };// your action here 
                    m.MenuItems.Add(infoMenuItem);

                    m.Show(lvAccounts, new Point(e.X, e.Y));
                }
            }
        }*/

        /*private void InfoAction(object sender, MouseEventArgs e)
        {
            //id is extra value when you need or delete it
            ListView ListViewControl = sender as ListView;

            if (ListViewControl.SelectedItems.Count > 1)
                return;
            int account_id = int.Parse(ListViewControl.SelectedItems[0].SubItems[2].Text.ToString());
            //PaypalAccount account = MainApp.g_db.get_account_by_id(account_id);
            MessageBox.Show(MainApp.g_db.get_account_str_by_id(account_id));
        }*/
        private void refresh_account_list_view(bool show_all = false)
        {
            lvAccounts.Items.Clear();

            m_dt = MainApp.g_db.get_registered_account_table();
            

            if (m_dt == null || m_dt.Rows.Count == 0)
                return;

            m_level_0_account_list = get_level_0_account_list();
            if (m_level_0_account_list == null || m_level_0_account_list.Count == 0)
                MessageBox.Show("There is no accounts to go to level 1.");
            //List<PaypalAccount> reg_account_list = TableSerialize<PaypalAccount>.dt_2_list(m_dt);
            // add columns.

            if (show_all)
            {
                lvAccounts.Columns.Add("No");
                lvAccounts.Columns.Add("Status");

                foreach (DataColumn col in m_dt.Columns)
                    lvAccounts.Columns.Add(col.ColumnName);
            }
            else
            {
                lvAccounts.Columns.Add("No");                
                lvAccounts.Columns.Add("account_id");
                lvAccounts.Columns.Add("mail");
                lvAccounts.Columns.Add("trans_count_for_preparing");
                lvAccounts.Columns.Add("created_time");
                lvAccounts.Columns.Add("trans_1");
                lvAccounts.Columns.Add("trans_2");
                lvAccounts.Columns.Add("trans_3");
                lvAccounts.Columns.Add("trans_4");
                lvAccounts.Columns.Add("trans_5");
            }

            // add rows.

            int no = 0;
//             foreach (DataRow row in m_dt.Rows)
//             {
//                 ListViewItem lv1 = new ListViewItem();
// 
//                 lv1.SubItems.Add((++no).ToString());
//                 lv1.SubItems.Add("ready");
// 
//                 for (int i = 2; i < lvAccounts.Columns.Count; i++)
//                 {
//                     lv1.SubItems.Add(row[lvAccounts.Columns[i].Text].ToString());
//                 }
//                 lvAccounts.Items.Add(lv1);
//             }

            foreach(PaypalAccount item in m_level_0_account_list)
            {
                ListViewItem lv1 = new ListViewItem((++no).ToString());
                
                lv1.SubItems.Add(item.id.ToString());
                lv1.SubItems.Add(item.mail);
                lv1.SubItems.Add(MainApp.g_db.get_trans_count_for_preparing(item.id).ToString());
                lv1.SubItems.Add(MainApp.g_db.get_account_created_time(item.id));

                List<PaypalAccount> receiver_account_list = MainApp.g_db.get_receiver_account(item.id);
                List<PaypalAccount> sender_account_list = MainApp.g_db.get_sender_account(item.id);

                int count = 0;
                int receiver_count = 0;
                if (receiver_account_list != null && receiver_account_list.Count != 0)
                //                     foreach (PaypalAccount receiver in receiver_account_list)
                //                     {
                //                         lv1.SubItems.Add(receiver.mail);                        
                //                         count++;
                //                         if (count > 5)
                //                             break;
                //                     }
                {
                    receiver_count = receiver_account_list.Count;
                    for (int i = 0; i < receiver_account_list.Count; i++)
                    {
                        lv1.SubItems.Add(receiver_account_list[i].mail);
                        lv1.SubItems[5 + i].BackColor = Color.LightBlue;
                        lv1.UseItemStyleForSubItems = false;
                        count++;
                        if (count > 5)
                            break;
                    }
                }

                int sender_count = 0;
                if(sender_account_list != null && sender_account_list.Count != 0)
                {
                    sender_count = Math.Min(sender_account_list.Count, 5 - receiver_count);
                    for (int i = 0; i < Math.Min(sender_account_list.Count, 5 - receiver_count); i++)
                    {
                        lv1.SubItems.Add(sender_account_list[i].mail);
                        lv1.SubItems[5 + receiver_count + i].BackColor = Color.LightGreen;
                        lv1.UseItemStyleForSubItems = false;
                    }
                }

                for (int i = 0; i < 5 - receiver_count - sender_count; i++)
                    lv1.SubItems.Add(string.Empty);

                lvAccounts.Items.Add(lv1);
            }

            lvAccounts.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        public List<PaypalAccount> get_level_0_account_list()
        {
            List<int> level_0_account_id_list = MainApp.g_db.get_account_list_by_level(0);
            if (level_0_account_id_list == null || level_0_account_id_list.Count == 0)
                return null;

            List<PaypalAccount> level_0_account_list = new List<PaypalAccount>();
            foreach (int item_id in level_0_account_id_list)
            {
                level_0_account_list.Add(MainApp.g_db.get_account_by_id(item_id));
            }
            return level_0_account_list;
        }

        public bool check_level_0_accounts_trans_count()
        {
            foreach(PaypalAccount each in m_level_0_account_list)
            {
                int current_trans_count = MainApp.g_db.get_real_trans_count(each.id);
                if (each.transaction_count_for_preparing > current_trans_count)
                    return false;
            }
            return true;
        }

        public PaypalAccount get_sender_account(PaypalAccount last_receiver)
        {
            List<PaypalAccount> available_list = new List<PaypalAccount>();

            foreach (PaypalAccount each in m_level_0_account_list)
            {
                int current_trans_count = MainApp.g_db.get_real_trans_count(each.id);
                if (each.money_balance > 10 && each.transaction_count_for_preparing > current_trans_count && (last_receiver == null || each.id != last_receiver.id))
                    available_list.Add(each);
            }

            if (available_list.Count != 0)
                return available_list[new Random().Next(0, available_list.Count - 1)];

            foreach (PaypalAccount each in m_level_0_account_list)
            {
                if (each.money_balance > 10 && (last_receiver == null || each.id != last_receiver.id))
                    available_list.Add(each);
            }

            if (available_list.Count != 0)
                return available_list[new Random().Next(0, available_list.Count - 1)];
            else
                return null;
        }

        public PaypalAccount get_receive_account(PaypalAccount sender_account)
        {
            List<PaypalAccount> available_list_temp = new List<PaypalAccount>();

            foreach (PaypalAccount each in m_level_0_account_list)
            {
                if (MainApp.g_db.check_duplicated_transaction(each.id, sender_account.id) == false)
                    available_list_temp.Add(each);
            }

            if (available_list_temp.Count == 0)
                return null;

            List<PaypalAccount> available_list = new List<PaypalAccount>();
            foreach (PaypalAccount each in available_list_temp)
            {
                int current_trans_count = MainApp.g_db.get_real_trans_count(each.id);
                if (each.money_balance < 10 && each.transaction_count_for_preparing > current_trans_count && each.id != sender_account.id)
                    available_list.Add(each);
            }

            if (available_list.Count != 0)
                return available_list[new Random().Next(0, available_list.Count - 1)];

            foreach (PaypalAccount each in available_list_temp)
            {
                int current_trans_count = MainApp.g_db.get_real_trans_count(each.id);
                if (each.transaction_count_for_preparing > current_trans_count && each.id != sender_account.id)
                    available_list.Add(each);
            }

            if (available_list.Count != 0)
                return available_list[new Random().Next(0, available_list.Count - 1)];

            return available_list_temp[new Random().Next(0, available_list_temp.Count - 1)];
        }

        public double get_real_amount(double send_amount)
        {
            double fee = send_amount * MainApp.g_setting.fee_percent / 100 + MainApp.g_setting.fee_fixed;
            return send_amount - Math.Round(fee, 2);
        }
    }
}
