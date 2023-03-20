using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbHelper.DbBase
{
    class Cookie
    {
        public int id;
        public string cookie;

        public Cookie()
        {
            id = 0;
            cookie = "";
        }
        public static string cookie_array_to_string(List<string> arr)
        {
            string ret = "";

            foreach (string x in arr)
            {
                if (ret == "")
                    ret = x;
                else
                    ret += "\n::#myspliter#::\n" + x;
            }

            return ret;
        }
        public static List<string> cookie_string_to_array(string x)
        {
            List<string> arr = new List<string>();

            string[] a = x.Split(new string[] { "\n::#myspliter#::\n" }, StringSplitOptions.None);
            foreach (string s in a)
                arr.Add(s);

            return arr;
        }
    }
}
