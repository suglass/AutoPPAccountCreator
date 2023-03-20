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
    public partial class frmTransSimul : Form
    {
        private int m_level;
        private DataTable m_dt;
        private List<acc_simul_info> m_simul_list;
        private List<trans_his> m_trans_his;
        private int m_amount = 0;
        private int m_charge_num = 0;
        private bool m_must_close;
        private int m_old_trans_his_num;
        private int m_trans_num;
        public frmTransSimul()
        {
            InitializeComponent();
        }

        private void frmCreateAccount_Load(object sender, EventArgs e)
        {
            cboLevel.Items.Add("1");
            cboLevel.Items.Add("2");
            cboLevel.SelectedIndex = 0;

            txtChargeAmount.Text = "";
            txtChargeNum.Text = "";

            panelResult.Visible = false;
            lblResult.Visible = false;

            string query = $"SELECT t1.id, t1.money_balance, t1.mail, t1.usergroup, t1.is_registered, t1.transaction_count_for_preparing, t1.level_2_amount";
            query += $", t2.level, t2.transaction_count, t2.created_time, t2.status";
            query += $" FROM {ConstEnv.paypal_accounts_table_name} as t1 LEFT JOIN {ConstEnv.account_status_table_name} as t2";
            query += $" ON t1.id = t2.account_id";
            query += $" WHERE t1.usergroup = '{MainApp.g_setting.group_name}' AND t1.is_registered = '{ConstEnv.ACCOUNT_REGISTERED}'";
            query += $" AND t2.level = '{m_level - 1}' ;";
            m_dt = MainApp.g_db.select(query);

            display_accounts_before();
            preapre_trans_history_from_db();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (btnStart.Text == "Start")
            {
                lblResult.Text = "";
                panelResult.Visible = false;

                try
                {
                    if (!string.IsNullOrEmpty(txtChargeAmount.Text))
                        m_amount = int.Parse(txtChargeAmount.Text);
                }
                catch (Exception exception)
                {
                    m_amount = 0;
                }

                try
                {
                    if (!string.IsNullOrEmpty(txtChargeNum.Text))
                        m_charge_num = int.Parse(txtChargeNum.Text);
                }
                catch (Exception exception)
                {
                    m_charge_num = 0;
                }

                if (m_charge_num > lvAccounts.Items.Count)
                {
                    MessageBox.Show("The charge number is over than the account number.");
                    return;
                }

                btnReset_Click(null, EventArgs.Empty);

                m_simul_list = new List<acc_simul_info>();

                for (int i = 0; i < m_dt.Rows.Count; i++)
                {
                    DataRow row = m_dt.Rows[i];

                    acc_simul_info item = new acc_simul_info(i, row["mail"].ToString(), int.Parse(row["id"].ToString()), int.Parse(row["money_balance"].ToString()),
                        int.Parse(row["transaction_count"].ToString()), int.Parse(row["transaction_count_for_preparing"].ToString()));
                    m_simul_list.Add(item);
                }

                m_must_close = false;

                new Thread(() => {
                    int ret = simulate();
                    update_status(ret);
                }).Start();

                btnStart.Text = "Cancel";
            }
            else
            {
                m_must_close = true;
                btnStart.Text = "Start";
            }
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        public void update_status(int ret)
        {
            this.InvokeOnUiThreadIfRequired(() =>
            {
                btnStart.Text = "Start";

                if (ret == -1) // cancelled.
                {
                    lblResult.Text = "Cancelled";
                }
                else
                {
                    panelResult.Visible = true;
                    lblResult.Visible = true;
                    rdoAfter.Checked = true;

                    lblResult.Text = $"Fee Formula\n{Math.Round(MainApp.g_setting.fee_percent, 2)} % + {Math.Round(MainApp.g_setting.fee_fixed, 2)}\n\n";

                    if (ret == 0)
                    {
                        lblResult.Text += $"Succeed\nTransaction number:\n{m_trans_num}";
                    }
                    else if (ret == 1)
                    {
                        lblResult.Text += $"Need charge money\nTransaction number:\n{m_trans_num}";
                    }
                    else
                    {
                        lblResult.Text += $"Unknown Result({ret})\nTransaction number:\n{m_trans_num}";
                    }
                }
            });
        }
        /// <summary>
        /// Refresh accounts list view control
        /// [NOTE] : This function MUST be called from Form_Load.
        /// </summary>
        private void display_accounts_before()
        {
            lvAccounts.Clear();

            lvAccounts.Columns.Add("No");
            lvAccounts.Columns.Add("Mail");
            lvAccounts.Columns.Add("Balance");
            lvAccounts.Columns.Add("Transaction Number");

            if (m_dt == null || m_dt.Rows.Count == 0)
                return;

            for (int i = 0; i < m_dt.Rows.Count; i++)
            {
                DataRow row = m_dt.Rows[i];

                ListViewItem lv1 = new ListViewItem((i + 1).ToString());
                lv1.SubItems[0].Name = "No";

                ListViewItem.ListViewSubItem subitem = lv1.SubItems.Add(row["mail"].ToString());
                subitem.Name = "mail";

                subitem = lv1.SubItems.Add(row["money_balance"].ToString());
                subitem.Name = "money_balance";

                subitem = lv1.SubItems.Add(row["transaction_count"].ToString());
                subitem.Name = "transaction_count";

                lvAccounts.Items.Add(lv1);
            }
            lvAccounts.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            lblResult.Text = "";
            panelResult.Visible = false;
            rdoBefore.Checked = true;

            preapre_trans_history_from_db();
            //display_accounts_before();
        }

        private void cboLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboLevel.Text == "1")
                m_level = 1;
            else
                m_level = 2;
            display_accounts_before();
        }
        private void preapre_trans_history_from_db()
        {
            m_trans_his = new List<trans_his>();

            DataTable dt = MainApp.g_db.select($"SELECT from_account_id, to_account_id, amount FROM {ConstEnv.transaction_history_table_name} ;");
            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    m_trans_his.Add(new trans_his(int.Parse(row["from_account_id"].ToString()), int.Parse(row["to_account_id"].ToString()), double.Parse(row["amount"].ToString())));
                }
            }
            m_old_trans_his_num = m_trans_his.Count;
        }
        private bool charge_money()
        {
            // Sort account list as to transaction number and money balance.

            m_simul_list.Sort((x, y) => {
                if (x.transnum == y.transnum && x.balance == y.balance)
                    return 0;
                if (x.transnum != y.transnum)
                    return (x.transnum < y.transnum) ? 1 : -1;
                return (x.balance < y.balance) ? 1 : -1;
            });

            // charge.

            for (int i = 0; i < m_charge_num; i++)
            {
                if (m_must_close)
                    return false;

                MainApp.log_info($"Charge Money.... {m_simul_list[i].mail} : {Math.Round(m_simul_list[i].balance, 2)} > {Math.Round(m_simul_list[i].balance + m_amount, 2)}");

                m_simul_list[i].balance += m_amount;
            }

            return true;
        }
        private bool check_all_accounts_satisfied()
        {
            bool ret = true;
            foreach (acc_simul_info acc in m_simul_list)
            {
                if (acc.transnum < acc.transnum_plan)
                {
                    ret = false;
                    break;
                }
            }
            return ret;
        }
        private int get_sender_id(int receiver_id)
        {
            List<int> candi_list = new List<int>();

            foreach (acc_simul_info acc in m_simul_list)
            {
                if (acc.transnum < acc.transnum_plan && acc.balance >= MainApp.g_setting.level_1_min_send_amount && acc.account_id != receiver_id)
                    candi_list.Add(acc.account_id);
            }
            if (candi_list.Count > 0)
                return candi_list[new Random().Next(0, candi_list.Count - 1)];

            foreach (acc_simul_info acc in m_simul_list)
            {
                if (acc.balance >= MainApp.g_setting.level_1_min_send_amount && acc.account_id != receiver_id)
                    candi_list.Add(acc.account_id);
            }
            if (candi_list.Count > 0)
                return candi_list[new Random().Next(0, candi_list.Count - 1)];

            return -1;
        }
        private bool already_transmitted(int id_1, int id_2)
        {
            foreach (trans_his his in m_trans_his)
            {
                if (his.exists(id_1, id_2))
                    return true;
            }
            return false;
        }
        private int get_receiver_id(int sender_id)
        {
            List<acc_simul_info> new_pair_list = new List<acc_simul_info>();

            // Find available receiver list. He must have no transaction history with sender.

            foreach (acc_simul_info acc in m_simul_list)
            {
                if (acc.account_id != sender_id && !already_transmitted(acc.account_id, sender_id))
                    new_pair_list.Add(acc);
            }
            if (new_pair_list.Count == 0)
                return -1;

            List<int> candi_list = new List<int>();

            foreach (acc_simul_info acc in new_pair_list)
            {
                if (acc.transnum < acc.transnum_plan && acc.balance < MainApp.g_setting.level_1_min_send_amount && acc.account_id != sender_id)
                    candi_list.Add(acc.account_id);
            }
            if (candi_list.Count > 0)
                return candi_list[new Random().Next(0, candi_list.Count - 1)];

            foreach (acc_simul_info acc in new_pair_list)
            {
                if (acc.transnum < acc.transnum_plan && acc.account_id != sender_id)
                    candi_list.Add(acc.account_id);
            }
            if (candi_list.Count > 0)
                return candi_list[new Random().Next(0, candi_list.Count - 1)];

            return new_pair_list[new Random().Next(0, new_pair_list.Count - 1)].account_id;
        }
        private double get_real_recv_money(double send_amount)
        {
            double fee = send_amount * MainApp.g_setting.fee_percent / 100 + MainApp.g_setting.fee_fixed;
            return send_amount - fee;
        }
        private acc_simul_info find_account(int id)
        {
            foreach (acc_simul_info acc in m_simul_list)
            {
                if (acc.account_id == id)
                    return acc;
            }
            return null;
        }
        private void send_money(int sender_id, int receiver_id, double send_amount)
        {
            acc_simul_info sender = find_account(sender_id);
            acc_simul_info receiver = find_account(receiver_id);
            double real_money = get_real_recv_money(send_amount);

            sender.balance -= send_amount;
            sender.transnum++;
            receiver.balance += real_money;
            receiver.transnum++;

            MainApp.log_info($"   Sent. Amount = {Math.Round(send_amount, 2)}, Real = {Math.Round(real_money, 2)}");
            MainApp.log_info($"                Balance           TransNum");
            MainApp.log_info($"   sender       {Math.Round(find_account(sender_id).balance, 2)}   {find_account(sender_id).transnum}/{find_account(receiver_id).transnum_plan}");
            MainApp.log_info($"   receiver     {Math.Round(find_account(receiver_id).balance, 2)} {find_account(receiver_id).transnum}/{find_account(receiver_id).transnum_plan}");

            m_trans_his.Add(new trans_his(sender_id, receiver_id, send_amount));
        }
        private int simulate()
        {
            int ret = -1;
            int sender_id;
            int receiver_id = -1;
            double send_amount;

            m_trans_num = 0;

            if (!charge_money())
                return -1;

            for (int i = 0; !m_must_close; i++)
            {
                MainApp.log_info($"...............{i + 1} th START");

                if (check_all_accounts_satisfied())
                {
                    MainApp.log_info("#### Congratulations!!!! ALL Account SATISFIED.");
                    ret = 0;
                    break;
                }

                sender_id = get_sender_id(receiver_id);
                if (sender_id == -1)
                {
                    sender_id = get_sender_id(-1);
                    if (sender_id == -1)
                    {
                        MainApp.log_info("#### Can not find proper sender. Need charge money.");
                        ret = 1;
                        break;
                    }
                }

                receiver_id = get_receiver_id(sender_id);
                if (receiver_id == -1)
                {
                    MainApp.log_info("#### Can not find proper receiver.");
                    ret = 2;
                    break;
                }

                send_amount = find_account(sender_id).balance - 1;
                send_amount = Math.Min(send_amount, MainApp.g_setting.level_1_max_send_amount);

                m_trans_num++;

                MainApp.log_info($"   try sending. Amount = {send_amount}");
                MainApp.log_info($"                Mail\t\t\t\t\t\tBalance\t\tTransNum");
                MainApp.log_info($"   sender       {find_account(sender_id).mail}\t{Math.Round(find_account(sender_id).balance, 2)}\t\t\t{find_account(sender_id).transnum}/{find_account(sender_id).transnum_plan}");
                MainApp.log_info($"   receiver     {find_account(receiver_id).mail}\t{Math.Round(find_account(receiver_id).balance, 2)}\t\t\t{find_account(receiver_id).transnum}/{find_account(receiver_id).transnum_plan}");

                send_money(sender_id, receiver_id, send_amount);

                MainApp.log_info($"...............{i + 1} th END");
            }
            MainApp.log_info("#### Thread FINISHED.");

            if (!m_must_close)
            {
                MainApp.log_info("************** ACCOUNTS *******************");
                foreach (acc_simul_info acc in m_simul_list)
                {
                    MainApp.log_info($"......{acc.mail}\t\t{Math.Round(acc.balance, 2)}\t\t{acc.transnum} / {acc.transnum_plan}");
                }
                MainApp.log_info("*******************************************");

                MainApp.log_info("************** HISTORY *******************");
                for (int i = m_old_trans_his_num; i < m_trans_his.Count; i++)
                {
                    trans_his his = m_trans_his[i];
                    MainApp.log_info($"......{Math.Round(his.amount, 2)}\t\t{find_account(his.send_id).mail}\t >> {find_account(his.recv_id).mail}");
                }
                MainApp.log_info("*******************************************");

                if (m_must_close)
                    return -1;
            }

            return ret;
        }
        void display_accounts_after()
        {
            lvAccounts.Clear();

            lvAccounts.Columns.Add("No");
            lvAccounts.Columns.Add("Mail");
            lvAccounts.Columns.Add("Balance");
            lvAccounts.Columns.Add("Transaction Number");

            int i = 0;
            foreach (acc_simul_info acc in m_simul_list)
            {
                ListViewItem lv1 = new ListViewItem((++i).ToString());
                lv1.SubItems[0].Name = "No";

                ListViewItem.ListViewSubItem subitem = lv1.SubItems.Add(acc.mail);
                subitem.Name = "mail";

                subitem = lv1.SubItems.Add(Math.Round(acc.balance, 2).ToString());
                subitem.Name = "money_balance";

                subitem = lv1.SubItems.Add($"{acc.transnum.ToString()} / {acc.transnum_plan.ToString()}");
                subitem.Name = "transaction_count";

                lvAccounts.Items.Add(lv1);
            }
            lvAccounts.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }
        void display_transaction_history()
        {
            lvAccounts.Clear();

            lvAccounts.Columns.Add("No");
            lvAccounts.Columns.Add("Sender");
            lvAccounts.Columns.Add("Receiver");
            lvAccounts.Columns.Add("Send Amount");

            for (int i = m_old_trans_his_num, k = 0; i < m_trans_his.Count; i++)
            {
                trans_his his = m_trans_his[i];

                ListViewItem lv1 = new ListViewItem((++k).ToString());
                lv1.SubItems[0].Name = "No";

                ListViewItem.ListViewSubItem subitem = lv1.SubItems.Add(find_account(his.send_id).mail);
                subitem.Name = "Sender";

                subitem = lv1.SubItems.Add(find_account(his.recv_id).mail);
                subitem.Name = "Receiver";

                subitem = lv1.SubItems.Add(Math.Round(his.amount, 2).ToString());
                subitem.Name = "Send Amount";

                lvAccounts.Items.Add(lv1);
            }
            lvAccounts.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }
        private void rdoBefore_CheckedChanged(object sender, EventArgs e)
        {
            display_accounts_before();
        }

        private void rdoAfter_CheckedChanged(object sender, EventArgs e)
        {
            display_accounts_after();
        }

        private void rdoHistory_CheckedChanged(object sender, EventArgs e)
        {
            display_transaction_history();
        }
    }
    public class acc_simul_info
    {
        public int account_id;
        public int listview_idx;
        public double balance;
        public int transnum;
        public int transnum_plan;
        public string mail;

        public acc_simul_info(int _listview_idx, string _mail, int _account_id, double _balance, int _transnum, int _transnum_plan)
        {
            mail = _mail;
            account_id = _account_id;
            listview_idx = _listview_idx;
            balance = _balance;
            transnum = _transnum;
            transnum_plan = _transnum_plan;
        }
    }
    public class trans_his
    {
        public int send_id;
        public int recv_id;
        public double amount;

        public trans_his(int _send_id, int _recv_id, double _amount)
        {
            send_id = _send_id;
            recv_id = _recv_id;
            amount = _amount;
        }
        public bool exists(int _send_id, int _recv_id)
        {
            return (((send_id == _send_id) && (recv_id == _recv_id)) || ((send_id == _recv_id) && (recv_id == _send_id)));
        }
        public bool exists(trans_his other)
        {
            if (other == null)
                return false;

            return (((send_id == other.send_id) && (recv_id == other.recv_id)) || ((send_id == other.recv_id) && (recv_id == other.send_id)));
        }
    }
}
