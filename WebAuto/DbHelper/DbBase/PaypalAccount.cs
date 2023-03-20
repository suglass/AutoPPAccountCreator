using ResourcesInApp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbHelper.DbBase
{
    public class PaypalAccount
    {
        public int id;
        public double money_balance;
        public string mail;
        public string mail_password;
        public string paypal_password;
        public string first_name;
        public string last_name;
        public string tel;
        public string postcode;
        public string city;
        public string birthday;
        public string country;
        public string street;
        public string usergroup;
        public int is_registered;
        public int transaction_count_for_preparing;
        public int level_2_amount;

        public PaypalAccount()
        {
            id = 0;
            money_balance = 0;
            mail = "";
            mail_password = "";
            paypal_password = "";
            first_name = "";
            last_name = "";
            tel = "";
            postcode = "";
            city = "";
            birthday = ConstEnv.INVALID_DATE_STR;
            country = "";
            street = "";
            usergroup = "";
            is_registered = 0;
            transaction_count_for_preparing = 1;
            level_2_amount = 500;
        }
    }
}
