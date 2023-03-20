using DbHelper;
using DbHelper.DbBase;
using MaterialSkin;
using MaterialSkin.Controls;
using Newtonsoft.Json;
using Resolution;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebAuto.PayPal_Auto;
using WebAuto.Utils;

namespace WebAuto
{
    public partial class MainFrm : MaterialForm
    {
        MaterialSkinManager skinman;
        List<PaypalAccount> m_account_list = new List<PaypalAccount>();
        List<ProxyServer> m_proxy_list = new List<ProxyServer>();
        

        string group;

        List<WorkerParam> m_reg_param_list = new List<WorkerParam>();
        List<WorkerParam> m_unreg_param_list = new List<WorkerParam>();

        AutoPro g_process;

        Dictionary<int, int> acc_proxy_dict = new Dictionary<int, int>();
        int m_thread_num = 0;
        int m_last_used_proxy_idx = 0;
        bool m_must_close = true;
        PaypalDbHelper m_db;
        DbConnection m_db_connection;
        public MainFrm()
        {
            InitializeComponent();
            skinman = MaterialSkinManager.Instance;
            skinman.AddFormToManage(this);
            skinman.Theme = MaterialSkinManager.Themes.DARK;
            skinman.ColorScheme = new ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900, Primary.Blue200, Accent.Teal200, TextShade.WHITE);
            MainApp.g_setting.resolutions = CResolution.GetAvailableResolutionsStrings();
        }

         private void btn_open_acc_Click(object sender, EventArgs e)
         {
 
         }
        

        private void MainFrm_Load(object sender, EventArgs e)
        {
            m_db_connection = new DbConnection("autopaypal");
            m_db_connection.Open();
            m_db = new PaypalDbHelper(m_db_connection);

            m_account_list.Clear();
            m_account_list = m_db.get_account_table();

            m_proxy_list.Clear();
            m_proxy_list = m_db.get_proxy_table();

            for (int i = 0; i < m_account_list.Count; i++)
            {
                WorkerParam param = new WorkerParam();
                if (m_account_list[i].is_registered == 0)
                {
                    param.account = m_account_list[i];
                    m_unreg_param_list.Add(param);
                }
                else
                {
                    param.account = m_account_list[i];
                    m_reg_param_list.Add(param);
                }
            }
            group = 'A'.ToString();

            MainApp.log_info($"first url {MainApp.g_setting.first_url}");
            MainApp.log_info($"delat time #{MainApp.g_setting.delay_time} (s)");

            MainApp.g_setting.resolutions = CResolution.GetAvailableResolutionsStrings();

        }

        public void update_log(string log)
        {
            this.InvokeOnUiThreadIfRequired(() =>
            {
                txt_last_log.Text = log;
                txt_log.Text = log + "\n" + txt_log.Text;
            });
        }

        private void btn_open_proxy_Click(object sender, EventArgs e)
        {
        }
        
        private void set_proxy_as_dead(ProxyServer proxy)
        {            
            for (int i = 0; i < m_proxy_list.Count; i++)
            {
                if (m_proxy_list[i] == proxy)
                {
                    m_proxy_list[i].dead = 1;
                }
            }
        }

        private async void btn_Create_All_unreg_Accounts()
        {
            List<PaypalAccount> m_unreg_account_list = new List<PaypalAccount>();
            m_unreg_account_list = m_db.get_account_list_from_Group_by_reg(group, 0);
            if (m_unreg_account_list.Count == 0)
            {
                MessageBox.Show("No unregisterized account info.");
                return;
            }

            List<ProxyServer> unused_proxy_list = new List<ProxyServer>();
            unused_proxy_list = m_db.get_unused_proxy_table();
            if (unused_proxy_list.Count == 0)
            {
                MessageBox.Show("No unused and active proxy.");
                return;
            }

            if (unused_proxy_list.Count < m_unreg_account_list.Count)
            {
                MessageBox.Show("Active and Unused Proxy count is less than Accounts count.");
                return;
            }

            m_last_used_proxy_idx = -1;
                        
            m_must_close = false;
            
            foreach (PaypalAccount item in m_unreg_account_list)
            {
                m_last_used_proxy_idx ++;

                WorkerParam param = null;
                if (!m_must_close)
                    break;                
                
                param.account = item;
                param.proxy = unused_proxy_list[m_last_used_proxy_idx];

                g_process = new AutoPro(param);


                MainApp.log_info($"{g_process.m_param.proxy} and account {g_process.m_param.account.mail}");

                bool success = await g_process.work_flow_create_Acc();
                if (success)
                {
                    m_db.add_cookies(param.account.id, g_process.m_lst_cookies);
                    m_db.set_account_useragent(param.account.id, g_process.m_useragent);
                    m_db.set_account_proxy(param.account.id, param.proxy.id);
                    m_db.set_account_to_REG(param.account.id);

                    await g_process.Quit();
                    break;
                }
                else
                {
                    m_last_used_proxy_idx++;

                    if (g_process.m_proxy_dead_flag)
                    {
                        m_db.set_proxy_as_dead(param.proxy.id);
                        break;
                    }

                    if (param.proxy == null)
                    {
                        await g_process.Quit();
                        break;
                    }
                }
                   
                await g_process.Quit();
                   
            }
        }

        private void Update_Reg_List(int id)
        {

        }
        private void Update_Unreg_List(int id)
        {

        }
        

        private void MainFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            m_must_close = true;
            g_process.m_must_terminate = true;
            MainApp.g_setting.Save();
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            m_must_close = true;
            g_process.m_must_terminate = true;
            MainApp.log_info("All browsers are closed.");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private WorkerParam take_from_login_send_param_list()           // only unregisterized accounts
        {
            WorkerParam param = null;

            lock (m_unreg_param_list)
            {
                if (m_unreg_param_list.Count > 0)
                {
                    param = m_unreg_param_list[0];
                    m_unreg_param_list.RemoveAt(0);

                    param.proxy = take_proxy_server_from_list();
                    if (param.proxy == null) // no active proxy.
                        return null;
                }
            }
            return param;
        }

        private async void btn_launch_Click(object sender, EventArgs e)
        {
            MainApp.log_info("Launching browsers started for Log in.");
            
            if (m_unreg_param_list.Count == 0)
            {
                MessageBox.Show("No unregisterized account info.");
                return;
            }
            
            m_must_close = true;

            // Init proxy info.

            m_last_used_proxy_idx = 3;
            
            m_must_close = false;            
            
            WorkerParam param = new WorkerParam();
            

            //while (!m_must_close && (param = take_from_login_send_param_list()) != null)
            param.account = m_account_list[0];

            int retry = 0;
            try
            {
                while (!m_must_close)
                {
                    param.proxy = m_proxy_list[m_last_used_proxy_idx];
                    AutoPro process = new AutoPro(param);
                    g_process = process;
                    

                    bool success = await g_process.work_flow_login_send();
                    if (success)
                    {                                    

                        await g_process.Quit();


                        break;
                    }
                    else
                    {
                        // change proxy.

                        set_proxy_as_dead(param.proxy);
                        m_db.set_proxy_as_dead(param.proxy.id);

                        //param.proxy = take_proxy_server_from_list();
                        m_last_used_proxy_idx++;
                        retry++;
                        if (retry == 2)
                            break;
                        if (param.proxy == null)
                        {
                            await g_process.Quit();
                            break;
                        }
                    }
                    await g_process.Quit();
                }
                
            }
            catch(Exception ex)
            {

            }
            
        }

        private void add_proxy_info(int proxy_id)
        {
            ProxyServer proxy = get_Proxy_from_List(proxy_id);
            string ip = "";
            if (proxy.type == "http")
                ip = proxy.username.Substring(proxy.username.IndexOf("ip") + 3);
            else if (proxy.type == "socks4")
                ip = proxy.username;
            else if (proxy.type == "socks5")
                ip = proxy.username;
            string ip_api_url;
            ip_api_url = $"http://pro.ip-api.com/json/{ip}?key=c6mMEh6hb586CcK";
            var w = new WebClient();
            string response_json = w.DownloadString(ip_api_url);
            ProxyInfo proxy_info = JsonConvert.DeserializeObject<ProxyInfo>(response_json);
        }

        private PaypalAccount get_Acc_from_List(int acc_id)
        {
            for (int i = 0; i < m_account_list.Count; i++)
                if (m_account_list[i].id == acc_id)
                    return m_account_list[i];
            return null;
        }
        private void set_Acc_to_reg(int acc_id)
        {
            for (int i = 0; i < m_account_list.Count; i++)
                if (m_account_list[i].id == acc_id)
                {
                    m_account_list[i].is_registered = 1;
                    break;
                }
        }
        private ProxyServer get_Proxy_from_List(int proxy_id)
        {
            for (int i = 0; i < m_proxy_list.Count; i++)
                if (m_proxy_list[i].id == proxy_id)
                    return m_proxy_list[i];
            return null;
        }
    }
}
