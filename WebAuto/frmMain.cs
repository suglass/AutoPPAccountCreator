using Resolution;
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

namespace WebAuto
{
    public partial class frmMain : Form
    {
        private bool m_is_closing = false;
        private ManualResetEvent sync_db_monitor_event = new ManualResetEvent(false);
        private ManualResetEvent sync_db_monitor_mail = new ManualResetEvent(false);
        private ManualResetEvent sync_db_monitor_proxy = new ManualResetEvent(false);

        private ucTask m_ucTask;
        private ucDB m_ucDB;
        private ucSettings m_ucSettings;
        public frmMain()
        {
            InitializeComponent();

            btnAuto.SetImages("auto");
            btnDB.SetImages("db");
            btnSetting.SetImages("setting");
            btnLog.SetImages("log");
            btnAuto.Pushed = true;
        }
        public void show_log_window()
        {
            MainApp.g_log_frm.Show();
            MainApp.g_log_frm.Activate();
        }
        public void log(string msg, string logtype)
        {
            if (logtype != "todo")
            {
                MainApp.g_full_log += "\n" + msg;
                if (MainApp.g_log_frm != null)
                    MainApp.g_log_frm.update_log();
            }
            else
            {
                m_ucTask.update_log(msg);
            }
            update_status(msg);
        }
        public void update_status(string msg)
        {
            this.InvokeOnUiThreadIfRequired(() =>
            {
                lblInstantLog.Text = msg;
            });
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            m_ucTask = new ucTask();
            m_ucTask.Parent = panelMain;
            m_ucTask.Dock = DockStyle.Fill;

            m_ucDB = new ucDB();
            m_ucDB.Parent = panelMain;
            m_ucDB.Dock = DockStyle.Fill;
            m_ucDB.Hide();

            m_ucSettings = new ucSettings();
            m_ucSettings.Parent = panelMain;
            m_ucSettings.Dock = DockStyle.Fill;
            m_ucSettings.Hide();

            this.FormClosing += new FormClosingEventHandler(frmMain_Closing);

            MainApp.g_setting.resolutions = CResolution.GetAvailableResolutionsStrings();

            // Monitor agent level up.

            new Thread(() =>
            {
                do
                {
                    if (m_is_closing)
                        break;

                    MainApp.g_db.monitor_agent_status_for_level_1();
                    MainApp.g_db.monitor_agent_status_for_level_2();
                } while (!sync_db_monitor_event.WaitOne(new TimeSpan(1, 0, 0))); // per 1 hour.

            }).Start();

            // Monitor mail.

            new Thread(() =>
            {
                do
                {
                    if (m_is_closing)
                        break;

                    MainApp.g_db.monitor_mail();

                } while (!sync_db_monitor_mail.WaitOne(new TimeSpan(0, 10, 0))); // per 10 min.

            }).Start();

            // Monitor proxy expired.

            new Thread(() =>
            {
                do
                {
                    if (m_is_closing)
                        break;

                    MainApp.g_db.monitor_proxy_server_expired();

                } while (!sync_db_monitor_proxy.WaitOne(new TimeSpan(6, 0, 0))); // per 6 hours.

            }).Start();
        }

        private void frmMain_Closing(object sender, FormClosingEventArgs e)
        {
            MainApp.close_log_window();

            m_is_closing = true;
            sync_db_monitor_event.Set();
            sync_db_monitor_mail.Set();
            sync_db_monitor_proxy.Set();
        }

        private void btnAuto_Click(object sender, EventArgs e)
        {
            btnAuto.Pushed = true;
            btnDB.Pushed = false;
            btnSetting.Pushed = false;

            m_ucTask.Show();
            m_ucDB.Hide();
            m_ucSettings.Hide();
        }

        private void btnDB_Click(object sender, EventArgs e)
        {
            btnAuto.Pushed = false;
            btnDB.Pushed = true;
            btnSetting.Pushed = false;

            m_ucTask.Hide();
            m_ucDB.Show();
            m_ucSettings.Hide();
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            btnAuto.Pushed = false;
            btnDB.Pushed = false;
            btnSetting.Pushed = true;

            m_ucTask.Hide();
            m_ucDB.Hide();
            m_ucSettings.Show();
        }

        private void btnLog_Click(object sender, EventArgs e)
        {
            MainApp.show_log_window();
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            MainApp.g_setting.Save();
        }
    }
}
