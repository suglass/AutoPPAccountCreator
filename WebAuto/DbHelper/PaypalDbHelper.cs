using DbHelper.DbBase;
using DBHelper.DbBase;
using ResourcesInApp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAuto;
using WebAuto.Utils;

namespace DbHelper
{
    public class PaypalDbHelper : DbBaseHelper
    {

        public PaypalDbHelper(DbConnection p_connection) : base(p_connection)
        {

        }
        #region "Basic Data Conversion"
        public static DateTime str_2_time(string s)
        {
            DateTime time = DateTime.ParseExact(s, ConstEnv.TIME_FORMAT, CultureInfo.InvariantCulture);
            return time;
        }
        public static string time_2_str(DateTime time)
        {
            string s = time.ToString(ConstEnv.TIME_FORMAT);
            return s;
        }
        #endregion
        #region "Account Table"
        public PaypalAccount get_account_by_mail(string mail)
        {
            DataTable dt = select("SELECT * FROM {0} WHERE mail = '{1}';", ConstEnv.paypal_accounts_table_name, mail);
            if (dt == null || dt.Rows.Count == 0)
                return null;
            List<PaypalAccount> accounts = TableSerialize<PaypalAccount>.dt_2_list(dt);
            if (accounts.Count > 1)
                throw new Exception("mail duplicated in account table.");
            return accounts[0];
        }
        public PaypalAccount get_account_by_id(int id)
        {
            DataTable dt = select("SELECT * FROM {0} WHERE id = '{1}';", ConstEnv.paypal_accounts_table_name, id);
            if (dt == null || dt.Rows.Count == 0)
                return null;
            List<PaypalAccount> accounts = TableSerialize<PaypalAccount>.dt_2_list(dt);
            if (accounts.Count > 1)
                throw new Exception("mail duplicated in account table.");
            return accounts[0];
        }

        public string get_account_str_by_id(int id)
        {
            string output = string.Empty;
            DataTable dt = select("SELECT * FROM {0} WHERE id = '{1}';", ConstEnv.paypal_accounts_table_name, id);
            if (dt == null || dt.Rows.Count == 0)
                return null;
            if (dt.Rows.Count > 2)
                throw new Exception(string.Format("Duplicated user agent. account_id = {0}", id));

            for (int i = 0; i < dt.Columns.Count; i++)
                output += dt.Rows[0][i].ToString() + Environment.NewLine;
            return output;
        }
        public List<PaypalAccount> get_account_table_by_group(string group)
        {
            DataTable dt = select("SELECT * FROM {0} WHERE usergroup = '{1}';", ConstEnv.paypal_accounts_table_name, group);
            if (dt == null)
                return null;
            List<PaypalAccount> accounts = TableSerialize<PaypalAccount>.dt_2_list(dt);
            return accounts;
        }
        public DataTable get_unregistered_account_table()
        {
            DataTable dt = select("SELECT * FROM {0} WHERE usergroup = '{1}' AND is_registered = '{2}';", ConstEnv.paypal_accounts_table_name, MainApp.g_setting.group_name, ConstEnv.ACCOUNT_UNREGISTERED);
            return dt;
        }
        public List<PaypalAccount> get_unregistered_account_list()
        {
            DataTable dt = get_unregistered_account_table();
            return TableSerialize<PaypalAccount>.dt_2_list(dt);
        }
        public DataTable get_registered_account_table()
        {
            DataTable dt = select("SELECT * FROM {0} WHERE usergroup = '{1}' AND is_registered = '{2}';", ConstEnv.paypal_accounts_table_name, MainApp.g_setting.group_name, ConstEnv.ACCOUNT_REGISTERED);
            return dt;
        }
        public List<PaypalAccount> get_registered_account_list()
        {
            DataTable dt = get_registered_account_table();
            if (dt == null || dt.Rows.Count == 0)
                return null;
            return TableSerialize<PaypalAccount>.dt_2_list(dt);
        }
        public void set_account_as_registered(int account_id, int status)
        {
            DataTable dt = select("SELECT * FROM {0} WHERE id = '{1}';", ConstEnv.paypal_accounts_table_name, account_id);
            if (dt != null && dt.Rows.Count == 1)
            {
                update("UPDATE {0} SET is_registered = '{2}' WHERE id = '{1}';", ConstEnv.paypal_accounts_table_name, account_id, status);
            }
        }
        #endregion "Account Table"
        
        public int get_random_user_agent_id()
        {
            DataTable dt = select("SELECT * FROM {0};", ConstEnv.useragent_table_name);

            if (dt == null || dt.Rows.Count == 0)
                return -1;

            return int.Parse(dt.Rows[new Random().Next(0, dt.Rows.Count - 1)]["id"].ToString());         
        }
        public string get_user_agent_from_id(int user_agent_id)
        {
            string user_agent = "";

            DataTable dt = select("SELECT * FROM {0} WHERE id = '{1}';", ConstEnv.useragent_table_name, user_agent_id);
            if (dt == null || dt.Rows.Count == 0)
                return user_agent;

            user_agent = dt.Rows[0]["useragent"].ToString();
            return user_agent;
        }
        public string get_user_agent(int account_id)
        {
            string user_agent = "";

            DataTable dt = select("SELECT * FROM {0} WHERE account_id = {1};", ConstEnv.account_context_table_name, account_id);
            if (dt == null || dt.Rows.Count == 0)
                return user_agent;
            if (dt.Rows.Count > 2)
                throw new Exception(string.Format("Duplicated user agent. account_id = {0}", account_id));

            int user_agent_id = int.Parse(dt.Rows[0]["useragent_id"].ToString());

            dt = select("SELECT * FROM {0} WHERE id = {1};", ConstEnv.useragent_table_name, user_agent_id);
            user_agent = dt.Rows[0]["useragent"].ToString();

            return user_agent;
        }
        public void set_account_resolution(int account_id, string resolution)
        {
            DataTable dt = select("SELECT * FROM {0} WHERE account_id = '{1}';", ConstEnv.account_context_table_name, account_id);
            if (dt != null && dt.Rows.Count == 1)
            {
                update("UPDATE {0} SET screen_resolution = '{2}' WHERE account_id = '{1}';", ConstEnv.account_context_table_name, account_id, resolution);
            }
        }
        
        public string get_account_resolution(int account_id)
        {
            DataTable dt = select("SELECT screen_resolution FROM {0} WHERE account_id = '{1}';", ConstEnv.account_context_table_name, account_id);
            if (dt == null && dt.Rows.Count == 0)
                return "";
            if (dt.Rows.Count > 2)
                throw new Exception(string.Format("Duplicated screen resolution. account_id = {0}", account_id));
            return dt.Rows[0]["screen_resolution"].ToString();
        }
        #region "Proxy Server"
        public ProxyServer get_proxy_server_by_url_port(string url, int port)
        {
            DataTable dt = select("SELECT * FROM {0} WHERE url = '{1}' AND port = '{2}';", ConstEnv.proxy_server_table_name, url, port);
            if (dt == null || dt.Rows.Count == 0)
                return null;
            if (dt.Rows.Count == 2)
                throw new Exception(string.Format("Duplicated proxy server. url = {0}, port = {1}", url, port));
            return TableSerialize<ProxyServer>.from_dt(dt);
        }
        public List<ProxyServer> get_unused_proxy_list()
        {
            DataTable dt = select($"SELECT * FROM {ConstEnv.proxy_server_table_name} WHERE in_use = '{ConstEnv.PROXY_FREE}' AND dead = '0' ;");
            List<ProxyServer> unused_proxy_servers = TableSerialize<ProxyServer>.dt_2_list(dt);

            return unused_proxy_servers;
        }
        public List<ProxyServer> get_active_proxy_table()
        {
            DataTable dt = load_table(ConstEnv.proxy_server_table_name);
            if (dt == null)
                return null;
            List<ProxyServer> proxy_servers = TableSerialize<ProxyServer>.dt_2_list(dt);
            if (proxy_servers == null)
                return null;

            for (int i = proxy_servers.Count - 1; i >= 0; i--)
            {
                ProxyServer server = proxy_servers[i];
                if (server.dead != 0)
                {
                    proxy_servers.RemoveAt(i);
                    continue;
                }
                if (is_proxy_group_expired(server.proxy_group_id))
                {
                    set_proxy_as_dead(server.id);
                    proxy_servers.RemoveAt(i);
                    continue;
                }
            }
            return proxy_servers;
        }

        public string get_proxy_type(int proxy_id)
        {
            string type = "";
            DataTable dt = select($"SELECT t1.type from proxy_group as t1 INNER JOIN proxy_server as t2 on t1.id = t2.proxy_group_id AND t2.id = {proxy_id};");
            if (dt == null || dt.Rows.Count == 0)
                return "";
            if (dt.Rows.Count == 2)
                throw new Exception(string.Format("Duplicated proxy server. proxy server id = {0}", proxy_id));
            type = dt.Rows[0]["type"].ToString();
            return type;
        }

        public void set_proxy_as_dead(int server_id)
        {
            DataTable dt = select("SELECT * FROM {0} WHERE id = '{1}';", ConstEnv.proxy_server_table_name, server_id);
            if (dt != null && dt.Rows.Count == 1)
            {
                update("UPDATE {0} SET dead = '{2}' WHERE id = '{1}';", ConstEnv.proxy_server_table_name, server_id, 1);
            }
        }

        public ProxyServer get_proxy(int id)
        {
            DataTable dt = select("SELECT * FROM {0} WHERE id = '{1}';", ConstEnv.proxy_server_table_name, id);
            if (dt == null || dt.Rows.Count == 0)
                return null;
            if (dt.Rows.Count == 2)
                throw new Exception(string.Format("Duplicated proxy server. id = {0}", id));
            return TableSerialize<ProxyServer>.from_dt(dt);
        }
        #endregion "Proxy Server"
        #region "ProxyGroup"
        public List<ProxyGroup> get_proxy_group_table()
        {
            DataTable dt = load_table(ConstEnv.proxy_group_table_name);
            if (dt == null)
                return null;
            List<ProxyGroup> proxy_groups = TableSerialize<ProxyGroup>.dt_2_list(dt);
            return proxy_groups;
        }
        public ProxyGroup get_proxy_group_by_name(string name)
        {
            DataTable dt = select("SELECT * FROM {0} WHERE name = '{1}';", ConstEnv.proxy_group_table_name, name);
            if (dt == null || dt.Rows.Count == 0)
                return null;
            ProxyGroup group = TableSerialize<ProxyGroup>.from_dt(dt);
            return group;
        }
        public void add_proxy_group(ProxyGroup group, bool is_new)
        {
            DataTable dt = TableSerialize<ProxyGroup>.to_dt(group);
            if (is_new)
                insert_table(ConstEnv.proxy_group_table_name, dt);
            else
                update_table(ConstEnv.proxy_group_table_name, dt);
        }
        public bool is_proxy_group_expired(int id)
        {
            DataTable dt = select("SELECT * FROM {0} WHERE id = '{1}';", ConstEnv.proxy_group_table_name, id);
            if (dt == null || dt.Rows.Count == 0)
                return true;
            ProxyGroup group = TableSerialize<ProxyGroup>.from_dt(dt);
            DateTime expired_date = PaypalDbHelper.str_2_time(group.expired_date);
            if (DateTime.Now >= expired_date)
                return true;

            return false;
        }
        #endregion "ProxyGroup"
        #region "Cookie"
        public void add_cookies(int account_id, List<string> cookie_str)
        {
            string cookie = Cookie.cookie_array_to_string(cookie_str);

            DataTable dt = select("SELECT * FROM {0} WHERE account_id = '{1}';", ConstEnv.cookie_table_name, account_id);

            if (dt == null || dt.Rows.Count == 0)
            {
                insert("INSERT INTO {0} (account_id, cookie) VALUES ('{1}', '{2}');", ConstEnv.cookie_table_name, account_id, cookie);
            }
            else
            {
                update("UPDATE {0} SET cookie = '{2}' WHERE account_id = '{1}';", ConstEnv.cookie_table_name, account_id, cookie);
            }
        }
        public List<string> get_cookies(int account_id)
        {
            List<string> cookkie_str = null;

            DataTable dt = select("SELECT * FROM {0} WHERE account_id = '{1}';", ConstEnv.cookie_table_name, account_id);
            if (dt != null && dt.Rows.Count == 1)
            {
                string v = dt.Rows[0]["cookie"].ToString();
                cookkie_str = Cookie.cookie_string_to_array(v);
            }
            else
            {
                cookkie_str = new List<string>();
            }

            return cookkie_str;
        }
        #endregion "Cookie"
        #region "Account Context"
        public void set_account_proxy(int account_id, int proxy_id)
        {
            DataTable dt = select("SELECT * FROM {0} WHERE account_id = '{1}';", ConstEnv.account_context_table_name, account_id);
            if (dt != null && dt.Rows.Count == 1)
            {
                update("UPDATE {0} SET proxy_id = '{2}' WHERE account_id = '{1}';", ConstEnv.account_context_table_name, account_id, proxy_id);
            }
            else
            {
                insert("INSERT INTO {0} (account_id, proxy_id) VALUES ('{1}', '{2}');", ConstEnv.account_context_table_name, account_id, proxy_id);
            }
        }
        public int get_account_proxy(int account_id)
        {
            int proxy_id = -1;
            DataTable dt = select("SELECT proxy_id FROM {0} WHERE account_id = '{1}';", ConstEnv.account_context_table_name, account_id);
            if (dt != null && dt.Rows.Count == 1)
            {
                proxy_id = int.Parse(dt.Rows[0]["proxy_id"].ToString());
            }
            return proxy_id;
        }
        #endregion "Account Context"

        public bool add_account_to_status_table(int account_id, string created_time)
        {
            DataTable dt = select("SELECT * FROM {0} WHERE account_id = '{1}';", ConstEnv.account_status_table_name, account_id);
            if (dt != null && dt.Rows.Count > 0)
                return false;
            insert("INSERT INTO {0} (account_id, level, transaction_count, created_time, status) VALUES ('{1}', '{2}', '{3}', '{4}', '{5}');", ConstEnv.account_status_table_name, account_id, 0, 0, created_time, 0);
            return true;
        }

        public bool add_account_to_context_table(int account_id, int proxy_id, int user_agent_id, string screen_res)
        {
            DataTable dt = select("SELECT * FROM {0} WHERE account_id = '{1}';", ConstEnv.account_context_table_name, account_id);
            if (dt != null && dt.Rows.Count > 0)
                return false;
            insert("INSERT INTO {0} (account_id, proxy_id, useragent_id, screen_resolution) VALUES ('{1}', '{2}', '{3}', '{4}');", ConstEnv.account_context_table_name, account_id, proxy_id, user_agent_id, screen_res);
            return true;
        }
        public void get_expired_proxy_simple_info(int id, out string name, out string expired_date)
        {
            try
            {
                DataTable dt = select($"SELECT * FROM {ConstEnv.proxy_group_table_name} WHERE id = {id} ;");
                if (dt == null || dt.Rows.Count == 0)
                    throw new Exception("no proxy group");
                if (dt.Rows.Count != 1)
                    throw new Exception("proxy group conflicts.");

                name = dt.Rows[0]["name"].ToString();
                expired_date = dt.Rows[0]["expired_date"].ToString();
            }
            catch (Exception exception)
            {
                name = "";
                expired_date = "";
            }
        }
        public void delete_todo_task(int id)
        {
            delete($"DELETE FROM {ConstEnv.to_do_table_name} WHERE id = {id} ;");
        }
        #region "Monitor"
        /// <summary>
        /// Monitor agent status if agent can be upgrade or it is blocked.
        /// </summary>
        public void monitor_agent_status_for_level_1()
        {
            string query = $"SELECT t1.*, t2.usergroup FROM {ConstEnv.account_status_table_name} as t1 LEFT JOIN {ConstEnv.paypal_accounts_table_name} as t2 ";
            query += $"ON t1.account_id = t2.id AND t1.level = '0' AND t2.is_registered = '{ConstEnv.ACCOUNT_REGISTERED}' AND ";
            query += $"( (t2.usergroup='C') OR ";
            query += $"((t2.usergroup='A' OR t2.usergroup='B') AND t1.transaction_count >= t2.transaction_count_for_preparing) );";
            DataTable dt = select(query);
            if (dt == null || dt.Rows.Count == 0)
                return;

            foreach (DataRow row in dt.Rows)
            {
                bool level_up = false;
                DateTime created_time = str_2_time(row["created_time"].ToString());
                TimeSpan span = DateTime.Now - created_time;

                int account_id = int.Parse(row["account_id"].ToString());
                string group = row["usergroup"].ToString();
                if (group == "C")
                {
                    if (span.Days >= MainApp.g_setting.level_1_remain_days_for_group_C)
                        level_up = true;
                }
                else if (group == "A" || group == "B")
                {
                    if (span.Days >= MainApp.g_setting.level_1_remain_days_for_group_A_or_B)
                    {
                        // Check sending amount.

//                         query = $"SELECT SUM(t1.amount) as send_sum FROM {ConstEnv.transaction_history_table_name} as t1 WHERE from_account_id = '{account_id}' ;";
//                         dt = select(query);
//                         if (dt == null || dt.Rows == null || dt.Rows.Count == 0)
//                             continue;
//                         string x = dt.Rows[0]["send_sum"].ToString();
//                         if (x == "")
//                             continue;
//                         int sum = int.Parse(dt.Rows[0]["send_sum"].ToString());
//                         if (MainApp.g_setting.level_1_min_send_amount <= sum && sum <= MainApp.g_setting.level_1_max_send_amount)
                        level_up = true;
                    }
                }
                if (level_up)
                {
                    // To Do : Should check agent's status?

                    update("UPDATE {0} SET level = '{2}' WHERE id = '{1}';", ConstEnv.account_status_table_name, account_id, 1);
                }
            }
        }
        /// <summary>
        /// Monitor agent status if agent can be upgrade to level 2 or it is blocked.
        /// </summary>
        public void monitor_agent_status_for_level_2()
        {
            string query = $"SELECT t1.*, t2.usergroup FROM {ConstEnv.account_status_table_name} as t1 LEFT JOIN {ConstEnv.paypal_accounts_table_name} as t2 ";
            query += $"ON t1.account_id = t2.id WHERE t1.level = '1' AND ";
            query += $"( t2.usergroup = 'A' AND t2.money_balance >= t2.level_2_amount );";
            DataTable dt = select(query);
            if (dt == null || dt.Rows.Count == 0)
                return;

            foreach (DataRow row in dt.Rows)
            {
                int account_id = int.Parse(row["account_id"].ToString());
                update("UPDATE {0} SET level = '{2}' WHERE id = '{1}';", ConstEnv.account_status_table_name, account_id, 2);
            }
        }
        /// <summary>
        /// Monitor every mail from paypal to check if it is blocked.
        /// If it is blocked, it must be added into ToDo table.
        /// </summary>
        public void monitor_mail()
        {
            string query = $"SELECT t1.*, t2.mail, t2.mail_password FROM {ConstEnv.account_status_table_name} as t1 LEFT JOIN {ConstEnv.paypal_accounts_table_name} as t2 ";
            query += $"ON t1.account_id = t2.id ";
            query += $"WHERE t2.is_registered = '{ConstEnv.ACCOUNT_REGISTERED}' AND ";
            query += $"( t1.status = '{ConstEnv.ACCOUNT_STATUS_NORMAL}' );";
            DataTable dt = select(query);
            if (dt == null || dt.Rows.Count == 0)
                return;

            foreach (DataRow row in dt.Rows)
            {
                int account_id = int.Parse(row["account_id"].ToString());
                string mail = row["mail"].ToString();
                string password = row["mail_password"].ToString();

                MailChecker mail_checker = new MailChecker(mail, password);
                int state = mail_checker.check_email_block_info();
                if (state == ConstEnv.ACCOUNT_MAIL_STATUS_BLOCK)
                {
                    update($"UPDATE {ConstEnv.paypal_accounts_table_name} SET is_registered = '{ConstEnv.ACCOUNT_BLOCKED}' WHERE id = '{account_id}' ;");

/*
                    query = $"SELECT * FROM {ConstEnv.to_do_table_name} WHERE object_id = '{account_id}' AND type = '{ConstEnv.TODO_TYPE_MAIL_BLOCKED}' ;";
                    DataTable dt_todo = select(query);
                    if (dt_todo == null || dt_todo.Rows.Count == 0)
                    {
                        string time = time_2_str(DateTime.Now);
                        insert($"INSERT INTO {ConstEnv.to_do_table_name} (object_id, type, create_time) VALUES ('{account_id}', '{ConstEnv.TODO_TYPE_MAIL_BLOCKED}', '{time}') ;");

                        MainApp.log_todo($"{ConstEnv.ToDo_Type[ConstEnv.TODO_TYPE_MAIL_BLOCKED]}, Mail : {mail}");
                    }
*/
                }
            }
        }
        /// <summary>
        /// Monitor proxy servers to check if it is expired.
        /// If it is blocked, it must be added into ToDo table.
        /// </summary>
        public void monitor_proxy_server_expired()
        {
            // Extract proxy group not in todo table.

            string query = $"SELECT t1.* FROM {ConstEnv.proxy_group_table_name} as t1 LEFT JOIN {ConstEnv.to_do_table_name} as t2 ";
            query += $"ON t2.object_id = t1.id AND t2.type = '{ConstEnv.TODO_TYPE_PROXY_EXPIRED}' WHERE t2.object_id IS NULL ; ";
            DataTable dt = select(query);
            if (dt == null || dt.Rows.Count == 0)
                return;

            foreach (DataRow row in dt.Rows)
            {
                int group_id = int.Parse(row["id"].ToString());
                DateTime expired_time = str_2_time(row["expired_date"].ToString());
                TimeSpan span = expired_time - DateTime.Now;

                if (span.TotalHours < MainApp.g_setting.proxy_expire_deadline_hours)
                {
                    string time = time_2_str(DateTime.Now);
                    insert($"INSERT INTO {ConstEnv.to_do_table_name} (object_id, type, create_time) VALUES ('{group_id}', '{ConstEnv.TODO_TYPE_PROXY_EXPIRED}', '{time}') ;");

                    MainApp.log_todo($"{ConstEnv.ToDo_Type[ConstEnv.TODO_TYPE_PROXY_EXPIRED]}, Name : {row["name"].ToString()}");
                }
            }
        }
        #endregion "Monitor"

        public string get_group_name_by_id(int account_id)
        {
            DataTable dt = select($"SELECT usergroup from {ConstEnv.paypal_accounts_table_name} WHERE id = {account_id};");
            if (dt == null || dt.Rows.Count == 0)
                return "";
            if (dt.Rows.Count >= 2)
                throw new Exception(string.Format("Duplicated proxy id. id = {0}", account_id));
            return dt.Rows[0]["usergroup"].ToString();
        }
        public PaypalAccount get_account_to_send(PaypalAccount sender_account, int to_level)
        {
            DataTable dt = select($"SELECT proxy_id, useragent_id, screen_resolution from account_context WHERE account_id = {sender_account.id}");
            if (dt == null || dt.Rows.Count == 0)
                return null;
            if (dt.Rows.Count == 2)
                throw new Exception(string.Format("Duplicated proxy id. id = {0}", sender_account.id));
            int sender_proxy_id = int.Parse(dt.Rows[0]["proxy_id"].ToString());
            int sender_user_agent_id = int.Parse(dt.Rows[0]["useragent_id"].ToString());
            string sender_screen_resolution = dt.Rows[0]["screen_resolution"].ToString();

            dt = select($"SELECT isp from proxy_server WHERE id = {sender_proxy_id}");
            if (dt == null || dt.Rows.Count == 0)
                return null;
            if (dt.Rows.Count == 2)
                throw new Exception(string.Format("Duplicated proxy id. id = {0}", sender_account.id));
            string sender_proxy_isp = dt.Rows[0][0].ToString();

            DataTable dt1 = select($"SELECT t1.account_id from account_context as t1 INNER JOIN proxy_server as t2 on t1.proxy_id = t2.id AND t2.isp != '{sender_proxy_isp}' AND t1.screen_resolution != '{sender_screen_resolution}' AND t1.useragent_id != {sender_user_agent_id};");
            if (dt1 == null || dt1.Rows.Count == 0)
            {
                dt1 = select($"SELECT t1.account_id from account_context as t1 INNER JOIN proxy_server as t2 on t1.proxy_id = t2.id AND t2.isp != '{sender_proxy_isp}' AND t1.screen_resolution != '{sender_screen_resolution}';");
                if (dt1 == null || dt1.Rows.Count == 0)
                    return null;
            }

            List<int> available_receiver_id_list = new List<int>();
            int available_count = dt1.Rows.Count;
            for (int i = 0; i < available_count; i ++)
            {
                int receiver_account_id = int.Parse(dt1.Rows[i][0].ToString());

                if (get_level_by_id(receiver_account_id) == to_level)
                    continue;

                if (to_level == 1)
                {
                    if (get_group_name_by_id(receiver_account_id) != sender_account.usergroup)
                        continue;
                }
                else
                {
                    if (get_group_name_by_id(receiver_account_id) == sender_account.usergroup)
                        continue;
                }
                

                dt = select($"SELECT * from {ConstEnv.transaction_history_table_name} WHERE from_account_id = {sender_account.id} AND to_account_id = {receiver_account_id};");
                if (dt != null && dt.Rows.Count > 0)
                    continue;

                dt = select($"SELECT * from {ConstEnv.transaction_history_table_name} WHERE from_account_id = {receiver_account_id} AND to_account_id = {sender_account.id};");
                if (dt != null && dt.Rows.Count > 0)
                    continue;

                int trans_count_for_preparing = get_trans_count_for_preparing(receiver_account_id);
        
                int real_trans_count = get_real_trans_count(receiver_account_id);

                if (trans_count_for_preparing == -1 || real_trans_count == -1)
                    continue;

                if (trans_count_for_preparing <= real_trans_count)
                    continue;

                available_receiver_id_list.Add(receiver_account_id);
            }

            if (available_receiver_id_list.Count == 0)
                return null;

            int output_account_id = available_receiver_id_list[0];
            for (int i = 1; i < available_receiver_id_list.Count; i ++)
            {
                int rest_count_Acc = get_trans_count_for_preparing(output_account_id) - get_real_trans_count(output_account_id);
                int rest_count = get_trans_count_for_preparing(available_receiver_id_list[i]) - get_real_trans_count(available_receiver_id_list[i]);

                if (rest_count_Acc < rest_count)
                    output_account_id = available_receiver_id_list[i];
            }

            return get_account_by_id(output_account_id);
        }

        public int get_real_trans_count(int account_id)
        {
            DataTable dt = select($"SELECT transaction_count from {ConstEnv.account_status_table_name} WHERE account_id = {account_id};");
            if (dt == null || dt.Rows.Count == 0)
                return -1;
            if (dt.Rows.Count >= 2)
                throw new Exception(string.Format("Duplicated account id. id = {0}", account_id));

            return int.Parse(dt.Rows[0]["transaction_count"].ToString());
        }
        public int get_trans_count_for_preparing(int account_id)
        {
            DataTable dt = select($"SELECT transaction_count_for_preparing from {ConstEnv.paypal_accounts_table_name} WHERE id = {account_id};");
            if (dt == null || dt.Rows.Count == 0)
                return -1;
            if (dt.Rows.Count >= 2)
                throw new Exception(string.Format("Duplicated account id. id = {0}", account_id));

            return int.Parse(dt.Rows[0][0].ToString());
        }

        public string get_account_created_time(int account_id)
        {
            DataTable dt = select($"SELECT created_time from {ConstEnv.account_status_table_name} WHERE account_id = {account_id};");
            if (dt == null || dt.Rows.Count == 0)
                return null;
            if (dt.Rows.Count >= 2)
                throw new Exception(string.Format("Duplicated account id. id = {0}", account_id));

            return dt.Rows[0][0].ToString();
        }
        
        public List<PaypalAccount> get_receiver_account(int account_id)
        {
            DataTable dt = select($"SELECT to_account_id from {ConstEnv.transaction_history_table_name} WHERE from_account_id = {account_id};");
            if (dt == null || dt.Rows.Count == 0)
                return null;

            List<PaypalAccount> result = new List<PaypalAccount>();
            foreach (DataRow row in dt.Rows)
            {
                result.Add(get_account_by_id(int.Parse(row["to_account_id"].ToString())));
            }
            return result;
        }

        public List<PaypalAccount> get_sender_account(int account_id)
        {
            DataTable dt = select($"SELECT from_account_id from {ConstEnv.transaction_history_table_name} WHERE to_account_id = {account_id};");
            if (dt == null || dt.Rows.Count == 0)
                return null;

            List<PaypalAccount> result = new List<PaypalAccount>();
            foreach (DataRow row in dt.Rows)
            {
                result.Add(get_account_by_id(int.Parse(row["from_account_id"].ToString())));
            }
            return result;
        }

        public List<int> get_account_list_by_level(int level)
        {
            DataTable dt = select($"SELECT account_id from account_status WHERE level = {level}");

            if (dt == null || dt.Rows.Count == 0)
                return null;
            List<int> accounts = new List<int>();
            foreach(DataRow row in dt.Rows)
            {
                if (is_blocked(int.Parse(row["account_id"].ToString())))
                    continue;
                if (get_group_name_by_id(int.Parse(row["account_id"].ToString())) != MainApp.g_setting.group_name)
                    continue;
                accounts.Add(int.Parse(row["account_id"].ToString()));
            }
            return accounts;
        }

        public void increase_trans_count(int account_id)
        {
            DataTable dt = select("SELECT transaction_count FROM {0} WHERE account_id = {1};", ConstEnv.account_status_table_name, account_id);
            if (dt != null && dt.Rows.Count == 1)
            {
                int last_count = int.Parse(dt.Rows[0][0].ToString());
                update("UPDATE {0} SET transaction_count = {2} WHERE account_id = '{1}';", ConstEnv.account_status_table_name, account_id, last_count + 1);
            }
        }

        public void set_real_money_balance(int account_id, double real_money_balance)
        {
            DataTable dt = select("SELECT money_balance FROM {0} WHERE id = {1};", ConstEnv.paypal_accounts_table_name, account_id);
            if (dt != null && dt.Rows.Count == 1)
            {                
                update("UPDATE {0} SET money_balance = {2} WHERE id = {1};", ConstEnv.paypal_accounts_table_name, account_id, real_money_balance);
            }
        }

        public int get_level_by_id(int account_id)
        {
            DataTable dt = select($"SELECT level from {ConstEnv.account_status_table_name} WHERE account_id = {account_id};");
            if (dt == null || dt.Rows.Count == 0)
                return -1;
            if (dt.Rows.Count >= 2)
                throw new Exception(string.Format("Duplicated account id. id = {0}", account_id));

            return int.Parse(dt.Rows[0][0].ToString());
        }

        public void add_transaction_history(int from_account_id, int to_account_id, double amount, DateTime created_time, string transaction_id)
        {            
            insert("INSERT INTO {0} (time, from_account_id, to_account_id, amount, from_transaction_id) VALUES ('{1}', {2}, {3}, {4}, '{5}');", ConstEnv.transaction_history_table_name, time_2_str(created_time), from_account_id, to_account_id, amount, transaction_id);
            increase_trans_count(from_account_id);
            increase_trans_count(to_account_id);
        }

        public bool check_duplicated_transaction(int from_account_id, int to_account_id)
        {
            DataTable dt = select($"SELECT * from {ConstEnv.transaction_history_table_name} WHERE from_account_id = {from_account_id} AND to_account_id = {to_account_id};");
            if (dt != null && dt.Rows.Count > 0)
                return true;

            dt = select($"SELECT * from {ConstEnv.transaction_history_table_name} WHERE from_account_id = {to_account_id} AND to_account_id = {from_account_id};");
            if (dt != null && dt.Rows.Count > 0)
                return true;

            return false;
        }

        public bool is_blocked(int account_id)
        {
            DataTable dt = select($"SELECT is_registered from {ConstEnv.paypal_accounts_table_name} WHERE id = {account_id};");
            if (dt == null || dt.Rows.Count == 0)
                return false;                                              // un registered account
            if (dt.Rows.Count >= 2)
                throw new Exception(string.Format("Duplicated account id. id = {0}", account_id));
            if (int.Parse(dt.Rows[0]["is_registered"].ToString()) == ConstEnv.ACCOUNT_BLOCKED)
                return true;
            else
                return false;
        }

        public void set_proxy_in_use(int proxy_id)
        {
            update($"UPDATE {ConstEnv.proxy_server_table_name} SET in_use = {ConstEnv.PROXY_IN_USE} WHERE id = {proxy_id};");
        }
    }
}
