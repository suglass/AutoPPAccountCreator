using DbHelper.DbBase;
using ResourcesInApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAuto.Utils;
using WebAuto.WebHelper;

namespace WebAuto.PayPal_Auto
{
    public partial class AutoPro : IWebHelper
    {
        public string frame_name;
        public string xpath;
        public bool flag;
        public int timeout;
        public int retry;
        public MailChecker m_clsMailChecker;
        public bool m_proxy_dead_flag;
        public double m_real_money_balance;
        public string m_sender_transaction_id;

        public int m_process_result;

        public AutoPro(WorkerParam param)
        {            
            m_clsMailChecker = new MailChecker(param.account.mail, param.account.mail_password);
            retry = 0;
            m_real_money_balance = 0;
            frame_name = "";
            xpath = "";
            flag = true;
            timeout = MainApp.g_setting.delay_time * 1000;

            m_proxy_dead_flag = false;
            m_must_terminate = false;
            m_param = param;
            
            m_process_result = ConstEnv.CREATE_STARTED;

//             int _ScreenWidth = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
//             int _ScreenHeight = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;

            // calc layout
            int cnt = MainApp.g_window_cnt;
//             m_size.Width = _ScreenWidth / 2;
//             m_size.Height = _ScreenHeight;
//             m_location.X = 0;
//             m_location.Y = 0;

            MainApp.log_info($"X = #{m_location.X}, Y = #{m_location.Y}");

            m_incognito = false;
            m_dis_js = false;
            m_dis_webrtc = false;
        }

        public async Task<bool> work_flow_create_Acc()
        {
            try
            {
                if (!await Start(1))
                {
                    MainApp.log_error("Chrome starting failed");
                    return false;
                }

                if (m_must_terminate)
                {
                    return false;
                }

                if (!await Navigate(MainApp.g_setting.first_url))
                {
                    MainApp.log_error($"#{m_param.account.mail}: navi to first url {MainApp.g_setting.first_url} failed.");
                    m_proxy_dead_flag = true;
                    return false;
                }

                if (m_must_terminate)
                {
                    return false;
                }

                string xpath = "//input[@id='paypalAccountData_firstName']";
                if (!await WaitToPresentByPath(xpath, MainApp.g_setting.delay_time * 1000))
                {
                    MainApp.log_error($"#{m_ID} - email component not present");
                    m_proxy_dead_flag = true;
                    return false;
                }
                MainApp.log_info($"first url navigated successfully.");

                m_process_result = ConstEnv.CREATE_PROXY_SUCCESS;

                if (m_must_terminate)
                {
                    return false;
                }

                if (m_param.account.mail != "" && !await create_Account())
                {
                    MainApp.log_error("Registration failed");
                    //Driver.Close();
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                MainApp.log_error(ex.Message + "\n" + ex.StackTrace);
                //Driver.Close();
                return false;
            }
        }

        public async Task<bool> work_flow_login()
        {
            try
            {
                if (!await Start(2))
                {
                    MainApp.log_error("Chrome starting failed");
                    return false;
                }

                if (m_must_terminate)
                {
                    return false;
                }

                if (!await Navigate(MainApp.g_setting.signin_url))
                {
                    MainApp.log_error($"Navi to first url {MainApp.g_setting.signin_url} failed.");
                    return false;
                }

                if (await WaitUrlSame(MainApp.g_setting.dashboard_url))
                    return true;

                if (m_must_terminate)
                {
                    return false;
                }

                xpath = "//input[@id='email' and @name='login_email']";
                if (!await WaitToPresentByPath(xpath, MainApp.g_setting.delay_time * 1000))
                {
                    MainApp.log_error($"#{m_ID} - password component for log in is not presented.");
                    //Driver.Close();
                    return false;
                }
                MainApp.log_info($"first url navigated successfully.");

                if (m_must_terminate)
                {
                    return false;
                }

                if (m_param.account.mail != "" && !await LogIn())
                {
                    MainApp.log_error("Registration failed");
                    //Driver.Close();
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                MainApp.log_error(ex.Message + "\n" + ex.StackTrace);
                //Driver.Close();
                return false;
            }
        }
    }
}
