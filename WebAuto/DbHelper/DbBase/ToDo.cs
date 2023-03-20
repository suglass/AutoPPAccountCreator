using ResourcesInApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBHelper.DbBase
{
    public class ToDo
    {
        public int id;
        public int object_id;
        public int type;
        public string create_time;
        public ToDo()
        {
            id = 0;
            object_id = 0;
            type = 0;
            create_time = ConstEnv.INVALID_TIME_STR;
        }
    }
}
