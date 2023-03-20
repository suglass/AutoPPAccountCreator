using ResourcesInApp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBHelper.DbBase
{
    public class ProxyGroup
    {
        public int id;
        public string name;
        public string type;
        public string expired_date;
        public string seller_url;
        public string seller_user;
        public string seller_password;

        public ProxyGroup()
        {
            id = 0;
            name = "";
            type = ConstEnv.PROXY_TYPE_HTTP;
            expired_date = ConstEnv.INVALID_DATE_STR;
            seller_url = "";
            seller_user = "";
            seller_password = "";
        }
        public static List<ProxyGroup> dt_2_list(DataTable dt)
        {
            List<ProxyGroup> groups = new List<ProxyGroup>();

            foreach (DataRow row in dt.Rows)
            {
                ProxyGroup group = new ProxyGroup();

                group.id = int.Parse(row["id"].ToString());
                group.name = row["name"].ToString();
                group.type = row["type"].ToString();
                group.expired_date = row["expired_date"].ToString();
                group.seller_url = row["seller_url"].ToString();
                group.seller_user = row["seller_user"].ToString();
                group.seller_password = row["seller_password"].ToString();

                groups.Add(group);
            }

            return groups;
        }
    }
}
