using DbHelper.DbBase;
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
    public partial class frmSendMoney : Form
    {
        List<PaypalAccount> reg_acc_list;
        AutoPro manual_send_pro;
        public frmSendMoney()
        {
            InitializeComponent();
        }

        private void frmSendMoney_Load(object sender, EventArgs e)
        {
            reg_acc_list = MainApp.g_db.get_registered_account_list();
            foreach (PaypalAccount item in reg_acc_list)
            {
                cboFrom.Items.Add(item.mail);
                cboTo.Items.Add(item.mail);
            }
            if (cboFrom.Items.Count > 0)
            {
                cboFrom.SelectedIndex = 0;
                cboTo.SelectedIndex = 0;
            }

            if (reg_acc_list.Count == 0)
            {
                lbStatus.Text = "No registered accounts.";
                btnSend.Enabled = false;
            }
            else
            {
                lbStatus.Text = "Ready";
                btnSend.Enabled = true;
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string sender_mail = cboFrom.Text;
            string receiver_mail = cboTo.Text;

            if (btnSend.Text == "Send")
            {
                btnSend.Text = "Cancel";
                new Thread((ThreadStart)(async () =>
                {
                    await Task.Delay(100);
                    WorkerParam manual_param = new WorkerParam();
                    manual_param.account = MainApp.g_db.get_account_by_mail(sender_mail);

                    int proxy_id = MainApp.g_db.get_account_proxy(manual_param.account.id);
                    if (proxy_id == -1)
                        return;
                    manual_param.proxy = MainApp.g_db.get_proxy(proxy_id);

                    manual_send_pro = new AutoPro(manual_param);

                    bool success = await manual_send_pro.work_flow_login();

                    PaypalAccount receiver_account = MainApp.g_db.get_account_by_mail(receiver_mail);

                    if (success)
                    {
                        if (manual_send_pro.m_real_money_balance >= MainApp.g_setting.level_1_min_send_amount)
                        {
                            string amount = Str_Utils.GetSendAmount(manual_send_pro.m_real_money_balance - 1);
                            if (await manual_send_pro.Send_Money(receiver_account, amount))
                            {
                                DateTime now = DateTime.Now;
                                MainApp.g_db.add_transaction_history(manual_param.account.id, receiver_account.id, (int)decimal.Parse(amount), now, manual_send_pro.m_sender_transaction_id);
                                MainApp.g_db.set_real_money_balance(manual_param.account.id, manual_send_pro.m_real_money_balance - (int)manual_send_pro.m_real_money_balance + 1);

                                double real_send_amount = get_real_amount((int)manual_send_pro.m_real_money_balance - 1);
                                MainApp.g_db.set_real_money_balance(receiver_account.id, receiver_account.money_balance + real_send_amount);                                
                            }
                        }
                    }
                    else
                        MainApp.log_error($"{manual_param.account.mail} - Login failed.");

                    await manual_send_pro.Quit();
                    btnSend.Text = "Send";
                })).Start();
            }
            else
            {
                manual_send_pro.m_must_terminate = true;
                btnSend.Text = "Send";
            }
        }

        public double get_real_amount(double send_amount)
        {
            double fee = send_amount * MainApp.g_setting.fee_percent / 100 + MainApp.g_setting.fee_fixed;
            return send_amount - Math.Round(fee, 2);
        }
    }
}
