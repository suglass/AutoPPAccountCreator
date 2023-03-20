using WebAuto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DbHelper;
using System.Threading;

namespace WebAuto
{
    static class MainApp
    {
        public static System.Object g_locker = new object();
        public static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static frmMain g_main_frm = null;
        public static int g_window_cnt = 0;

        public static UserSetting g_setting = new UserSetting();
        public static PaypalDbHelper g_db;
        public static DbConnection g_db_connection;

        public static frmLog g_log_frm = null;
        public static bool g_show_log_frm = false;
        public static string g_full_log = "";

        [STAThread]
        static void Main()
        {
            g_setting = UserSetting.Load();
            if (g_setting == null)
                g_setting = new UserSetting();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            while (true)
            {
                try
                {
                    g_db_connection = new DbConnection(g_setting.database_name, g_setting.db_hostname, g_setting.db_port, g_setting.db_username, g_setting.db_password,
                        g_setting.db_use_ssh, g_setting.db_ssh_hostname, g_setting.db_ssh_port, g_setting.db_ssh_username, g_setting.db_ssh_password, g_setting.db_ssh_keyfile);
                    g_db_connection.Open();
                    if (g_db_connection.is_opened)
                    {
                        g_db = new PaypalDbHelper(g_db_connection);
                        break;
                    }
                }
                catch (Exception exception)
                {
                    log_error($"Can not start program. Make sure MySQL server information or if it is running.\nMessage : {exception.Message}");
                }

                if (MessageBox.Show($"Can not start program. Make sure MySQL server information or if it is running.\nDo you want set MySQL server information now?", "DB Settings", MessageBoxButtons.YesNo) != DialogResult.Yes)
                    Environment.Exit(0);

                frmDBSetting dlg = new frmDBSetting();
                if (dlg.ShowDialog() != DialogResult.OK)
                    Environment.Exit(0);
            }
            g_setting.Save();

            g_main_frm = new frmMain();
            Application.Run(g_main_frm);
        }
        public static void show_log_window()
        {
            if (!g_show_log_frm)
            {
                Thread thread = new Thread(() =>
                {
                    g_log_frm = new frmLog();

                    g_log_frm.ShowDialog();
                });
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                g_show_log_frm = true;
            }
            else
            {
                if (g_log_frm != null)
                {
                    g_log_frm.Invoke(new Action(() => { g_log_frm.Activate(); }));
                }
            }
        }
        public static void close_log_window()
        {
            if (g_log_frm != null)
            {
                g_log_frm.Invoke(new Action(() => { g_log_frm.Close(); }));
            }
        }
        public static void log(string msg, string logtype, bool msgbox = false)
        {
            lock (g_locker)
            {
                try
                {
                    if (logtype == "error")
                        logger.Error(msg);
                    else
                        logger.Info(msg);

                    if (msgbox)
                        MessageBox.Show(msg);

                    msg = DateTime.Now.ToString("dd.MM.yyyy_hh:mm:ss ") + msg;
                    g_main_frm.log(msg, logtype);
                }
                catch (Exception ex)
                {

                }
            }
        }

        public static void log_info(string msg, bool msgbox = false)
        {
            log(msg, "info", msgbox);
        }

        public static void log_error(string msg, bool msgbox = false)
        {
            log(msg, "error", msgbox);
        }
        public static void log_todo(string msg)
        {
            log(msg, "todo", false);
        }

    }
}