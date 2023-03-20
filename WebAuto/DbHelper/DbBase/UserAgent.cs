using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBHelper.DbBase
{
    public class UserAgent
    {
        public int id;
        public string useragent;
        public string system;
        public string browser;
        public UserAgent()
        {
            id = 0;
            useragent = "";
            system = "";
            browser = "";
        }
    }
}
