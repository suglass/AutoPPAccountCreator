using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBHelper.DbBase
{
    public class AccountContext
    {
        public int id;
        public int account_id;
        public int proxy_id;
        public int useragent_id;
        public int cookie_id;
        public string screen_resolution;

        public AccountContext()
        {
            id = 0;
            account_id = 0;
            proxy_id = 0;
            useragent_id = 0;
            cookie_id = 0;
            screen_resolution = "";
        }
    }
}
