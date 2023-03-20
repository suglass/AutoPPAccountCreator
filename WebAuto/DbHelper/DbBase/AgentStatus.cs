using ResourcesInApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBHelper.DbBase
{
    public class AgentStatus
    {
        public int id;
        public int account_id;
        public int level;
        int transaction_count;
        string created_time;
        int status;
        public AgentStatus()
        {
            id = 0;
            account_id = 0;
            level = 0;
            transaction_count = 0;
            created_time = ConstEnv.INVALID_DATE_STR;
            status = 0;
        }
    }
}
