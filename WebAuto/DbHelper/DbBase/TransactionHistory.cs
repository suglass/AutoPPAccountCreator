using ResourcesInApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBHelper.DbBase
{
    public class TransactionHistory
    {
        public int id;
        public string time;
        public int from_account_id;
        public int to_account_id;
        public double amount;
        public string from_transaction_id;
        public string to_transaction_id;

        public TransactionHistory()
        {
            id = 0;
            time = ConstEnv.INVALID_DATE_STR;
            from_account_id = 0;
            to_account_id = 0;
            amount = 0;
            from_transaction_id = "";
            to_transaction_id = "";
        }
    }
}
