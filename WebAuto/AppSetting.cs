using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace WebAuto
{
    public class AppSettings<T> where T : new()
    {
        private const string DEFAULT_FILENAME = "settings.ini";

        public void Save(string fileName = DEFAULT_FILENAME)
        {
            try
            {
                File.WriteAllText(fileName, (new JavaScriptSerializer()).Serialize(this));
            }
            catch (Exception e)
            {
                Console.WriteLine("## App Setting Saving Failed : " + e.Message);
            }
        }

        public static void Save(T pSettings, string fileName = DEFAULT_FILENAME)
        {
            File.WriteAllText(fileName, (new JavaScriptSerializer()).Serialize(pSettings));
        }

        public static T Load(string fileName = DEFAULT_FILENAME)
        {
            try
            { 
                T t = new T();
                if (File.Exists(fileName))
                    t = (new JavaScriptSerializer()).Deserialize<T>(File.ReadAllText(fileName));
                else
                    return default(T);
                return t;
            }
            catch (Exception e)
            {
                Console.WriteLine("## App Setting Loading Failed : " + e.Message);
                return default(T);
            }
        }
    }

    public class UserSetting : AppSettings<UserSetting>
    {
        public string first_url = "https://www.paypal.com/welcome/signup/#/email_password";
        public string dashboard_url = "https://www.paypal.com/myaccount/summary/";
        public string signin_url = "https://www.paypal.com/de/signin";
        public string transaction_url = "https://www.paypal.com/myaccount/transactions/details/";
        public string url_to_send = "https://www.paypal.com/myaccount/transfer/?from=Header";
        public int delay_time = 60;
        public string result_file_name = "result.txt";
        public string[] error_res_list = new string[]
        { "ERROR_ZERO_CAPTCHA_FILESIZE", "ERROR_WRONG_USER_KEY", "ERROR_KEY_DOES_NOT_EXIST",
            "ERROR_ZERO_BALANCE", "ERROR_PAGEURL", "ERROR_NO_SLOT_AVAILABLE", "ERROR_TOO_BIG_CAPTCHA_FILESIZE",
            "ERROR_WRONG_FILE_EXTENSION", "ERROR_IMAGE_TYPE_NOT_SUPPORTED", "CAPCHA_NOT_READY" };
        public Dictionary<string, string> MailServer_Info = new Dictionary<string, string>
        {
            {"@web.de", "imap.web.de|993"},
            {"@gmx.de", "imap.gmx.net|993"}
        };

		public string[] random_resolutions = new string[]
        {
            "1920x1080",
            "1440x900",
            "1366x768",
            "1280x1024",
            "1280x960"
        };

        public List<string> resolutions;

        public int level_1_remain_days_for_group_A_or_B = 7;
        public int level_1_remain_days_for_group_C = 14;
        public int level_1_min_send_amount = 10;
        public int level_1_max_send_amount = 30;

        public int proxy_expire_deadline_hours = 24;

        public string group_name = "A";

        public string database_name = "autoweb";

        public string db_hostname = "localhost";
        public int db_port = 3306;
        public string db_username = "root";
        public string db_password = "";
        public bool db_use_ssh = false;
        public string db_ssh_hostname = "";
        public int db_ssh_port = 22;
        public string db_ssh_username = "";
        public string db_ssh_password = "";
        public string db_ssh_keyfile = "";

        public double fee_percent = 2.49;
        public double fee_fixed = 0.35;
    }
}
