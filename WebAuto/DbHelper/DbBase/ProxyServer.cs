using ResourcesInApp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbHelper.DbBase
{
    public class ProxyServer
    {
        public int id;
        public int proxy_group_id;
        public string url;
        public int port;
        public string username;
        public string password;
        public int dead;
        public string country;
        public string city;
        public string isp;
        public int in_use;

        public ProxyServer()
        {
            id = 0;
            proxy_group_id = 0;
            url = "";
            port = 0;
            username = "";
            password = "";
            dead = 0;
            country = "";
            city = "";
            isp = "";
            in_use = ConstEnv.PROXY_FREE;
        }
    }
}
