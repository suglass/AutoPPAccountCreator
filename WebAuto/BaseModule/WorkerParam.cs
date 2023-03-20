using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAuto.BaseModule
{
    public class WorkerParam
    {
        public int wid;
        public string proxy = "";

        public string id = "";
        public string balance = "";
        public string mail = "";
        public string mail_pwd = "";
        public string pp_pwd = "";
        public string first_name = "";
        public string last_name = "";
        public string phone = "";
        public string address1 = "";
        public string address2 = "";
        public string PLZ = "";
        public string city = "";
        public DateTime DOB;

        public string status = "";
        public WorkType type;
    }

    public enum WorkType
    {
        Normal = 0,
    }
}
