using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResourcesInApp
{
    public static class ConstEnv
    {
        public static readonly string TIME_FORMAT = "d.M.yyyy";

        public static readonly string cookie_table_name = "cookie";
        public static readonly string paypal_accounts_table_name = "account";
        public static readonly string proxy_server_table_name = "proxy_server";
        public static readonly string proxy_group_table_name = "proxy_group";
        public static readonly string account_context_table_name = "account_context";
        public static readonly string account_status_table_name = "account_status";
        public static readonly string to_do_table_name = "todo";
        public static readonly string transaction_history_table_name = "transaction_history";
        public static readonly string useragent_table_name = "useragent";

        public static readonly string PROXY_TYPE_HTTP = "http";
        public static readonly string PROXY_TYPE_SOCKS4 = "socks4";
        public static readonly string PROXY_TYPE_SOCKS5 = "socks5";

        public static readonly DateTime INVALID_TIME = new DateTime(2100, 1, 1, 0, 0, 0);
        public static readonly string INVALID_DATE_STR = "1.1.2100";
        public static readonly string INVALID_TIME_STR = "1.1.2100 0:0:0";

        public static readonly string PROXY_IP_API_FORMAT = "http://pro.ip-api.com/json/{0}?key={1}";
        public static readonly string PROXY_IP_API_KEY = "c6mMEh6hb586CcK";

        public static readonly int ACCOUNT_UNREGISTERED = 0;
        public static readonly int ACCOUNT_REGISTER_ABORTED = 1;
        public static readonly int ACCOUNT_REGISTERED = 2;
        public static readonly int ACCOUNT_BLOCKED = 3;

        public static readonly int ACCOUNT_MAIL_STATUS_FAILED = 0;
        public static readonly int ACCOUNT_MAIL_STATUS_NORMAL = 1;
        public static readonly int ACCOUNT_MAIL_STATUS_BLOCK = 2;

        public static readonly int ACCOUNT_STATUS_NORMAL = 0;
        public static readonly int ACCOUNT_STATUS_REQUIRE_RELEASE = 1;
        public static readonly int ACCOUNT_STATUS_BLOCK = 2;

        public static readonly int TODO_TYPE_PROXY_EXPIRED = 1;
        public static readonly int TODO_TYPE_MAIL_BLOCKED = 2;
        public static readonly int TODO_TYPE_CHARGE_LEVEL_1 = 3;
        public static readonly int TODO_TYPE_CHARGE_LEVEL_2 = 4;

        public static readonly int PROXY_FREE = 0;
        public static readonly int PROXY_IN_USE = 1;

        public static readonly int CREATE_STARTED = 0;
        public static readonly int CREATE_PROXY_SUCCESS = 1;
        public static readonly int CREATE_INPUT_SUCCESS = 2;
        public static readonly int CREATE_MAIL_SUCCESS = 3;
        public static readonly int CREATE_FINAL_SUCCESS = 4;

        public static readonly Dictionary<int, string> ToDo_Type = new Dictionary<int, string>
        {
            { TODO_TYPE_PROXY_EXPIRED, "Proxy Expired" },
            { TODO_TYPE_MAIL_BLOCKED, "Mail Blocked" },
            { TODO_TYPE_CHARGE_LEVEL_1, "Charge Level 1 Money" },
            { TODO_TYPE_CHARGE_LEVEL_2, "Charge Level 1 Money" }
        };
    }
}
