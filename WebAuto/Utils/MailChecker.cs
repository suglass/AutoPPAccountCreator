using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Imap;
using System.Text.RegularExpressions;
using MailKit;
using MailKit.Search;
using ResourcesInApp;

namespace WebAuto.Utils
{
    public class MailChecker
    {
        public ImapClient client = new ImapClient();
        
        private string mailServer, login, password;
        private int port;
        private bool ssl;

        public MailChecker(string addr, string pwd)
        {
            login = addr;
            password = pwd;
        }
        public bool connect(string mailServer, int port, bool ssl, string login, string password)
        {
            try
            {
                MainApp.log_info($"connecting .... server = {mailServer}, port = {port}");
                this.mailServer = mailServer;
                this.port = port;
                this.ssl = ssl;
                this.login = login;
                this.password = password;

                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                client.Connect(mailServer, port, MailKit.Security.SecureSocketOptions.Auto);
                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(login, password);
                MainApp.log_info("connected");
            }
            catch (Exception ex)
            {
                MainApp.log_error(ex.Message);
                return false;
            }
            return true;
        }

        public bool disconnect()
        {
            try
            {
                client.Disconnect(true);
                MainApp.log_info("disconnected");
            }
            catch (Exception ex)
            {
                MainApp.log_error(ex.Message);
                return false;
            }
            return true;
        }
        public string html2text(string htmltext)
        {
            Regex reg1 = new Regex("<[^>]*>");
            Regex reg2 = new Regex("<style>[^<]*</style>");
            htmltext = reg2.Replace(htmltext, "");
            return reg1.Replace(htmltext, "");
        }

     
        public string extract_email_verification_url_from_body(MimeKit.MimeMessage message)
        {
            string url = "";
            string base64 = "";
            string[] lines;

            try
            {
                MainApp.log_info($"message.BodyParts.Count() = {message.BodyParts.Count()}");

                if (message.BodyParts.Count() != 2)
                    return url;

                foreach (var part in message.BodyParts)
                {
                    MainApp.log_info($"part.ContentType.ToString() = {part.ContentType.ToString()}");

                    if (part.ContentType.ToString() != "Content-Type: text/html; charset=\"UTF-8\"")
                    {
                        continue;
                    }

                    string part_str = part.ToString();
                    lines = part_str.Split('\n');

                    MainApp.log_info($"lines.Length = {lines.Length}");

                    int k = 0;
                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (lines[i] == "\r")
                        {
                            k++;

                            if (k == 1)
                                continue;

                            break;
                        }
                        if (k == 0)
                            continue;

                        base64 += (base64.Length == 0) ? lines[i] : "\n" + lines[i];
                    }
                    break;
                }

                MainApp.log_info($"base64 = {base64}");

                if (base64 == "")
                    return url;

                byte[] decodedBytes = Convert.FromBase64String(base64);
                string decodedText = Encoding.UTF8.GetString(decodedBytes);

                //add_log("decodedText = {0}", decodedText);

                lines = decodedText.Split('\n');

                foreach (string line in lines)
                {
                    if (line.Contains("E-Mail-Adresse bestätigen"))
                    {
                        MainApp.log_info($"find = {line}");
                        url = line.Substring(line.IndexOf("href=\"") + 6);
                        url = url.Substring(0, url.Length - "\">E-Mail-Adresse bestätigen</a></td>".Length);
                        MainApp.log_info($"result = {url}");
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MainApp.log_error(ex.Message);
                url = "";
            }

            return url;
        }
        public string extract_email_transaction_code_from_body(MimeKit.MimeMessage message, string receiver_mail, string receiver_fullname)
        {
            string transaction_id = "";
            string base64 = "";
            string[] lines;

            try
            {
                MainApp.log_info($"message.BodyParts.Count() = {message.BodyParts.Count()}");

                if (message.BodyParts.Count() != 1)
                    return transaction_id;

                foreach (var part in message.BodyParts)
                {
                    MainApp.log_info($"part.ContentType.ToString() = {part.ContentType.ToString()}");

                    if (part.ContentType.ToString() != "Content-Type: text/html; charset=\"UTF-8\"")
                    {
                        continue;
                    }

                    string part_str = part.ToString();
                    lines = part_str.Split('\n');

                    MainApp.log_info($"lines.Length = {lines.Length}");

                    int k = 0;
                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (lines[i] == "\r")
                        {
                            k++;

                            if (k == 1)
                                continue;

                            break;
                        }
                        if (k == 0)
                            continue;

                        base64 += (base64.Length == 0) ? lines[i] : "\n" + lines[i];
                    }
                    break;
                }

                MainApp.log_info($"base64 = {base64}");

                if (base64 == "")
                    return transaction_id;

                byte[] decodedBytes = Convert.FromBase64String(base64);
                string decodedText = Encoding.UTF8.GetString(decodedBytes);

                //add_log("decodedText = {0}", decodedText);

                lines = decodedText.Split('\n');

                bool flag = false;
                foreach (string line in lines)
                {
                    if (line.IndexOf(receiver_mail, StringComparison.InvariantCultureIgnoreCase) > -1 || line.IndexOf(receiver_fullname, StringComparison.InvariantCultureIgnoreCase) > -1)
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                    return transaction_id;

                foreach (string line in lines)
                {
                    if (line.IndexOf("Transaktionscode:", StringComparison.InvariantCultureIgnoreCase) > -1)
                    {
                        MainApp.log_info($"find = {line}");
                        transaction_id = line.Substring(line.IndexOf("Transaktionscode:", StringComparison.InvariantCultureIgnoreCase) + 17);
                        transaction_id = transaction_id.Substring(0, transaction_id.IndexOf("</td>"));
                        transaction_id = transaction_id.Replace(" ", string.Empty);
                        MainApp.log_info($"result = {transaction_id}");
                        break;
                    }
                }

            }
            catch (Exception ex)
            {
                MainApp.log_error(ex.Message);
                transaction_id = "";
            }

            return transaction_id;
        }
        public string extract_email_verification_url(int delay_time = 300000)
        {
            string url = "";
            int repeat_count;
            MimeKit.MimeMessage message;

            try
            {
                for (repeat_count = 0; repeat_count < delay_time; repeat_count += 1000, System.Threading.Thread.Sleep(1000))
                {
                    DateTime time_to = DateTime.Now;
                    DateTime time_from = time_to - new TimeSpan(1, 0, 0, 0);
                    var inbox = client.Inbox;
                    inbox.Open(FolderAccess.ReadWrite);

                    SearchQuery search_query = SearchQuery.SubjectContains("Bestätigen Sie Ihre E-Mail-Adresse").And(SearchQuery.FromContains("service@paypal.de"));
                    //SearchQuery search_query1 = SearchQuery.DeliveredAfter(time_from).And(SearchQuery.DeliveredBefore(time_to));
                    //search_query = search_query.And(search_query1);
                    var results = inbox.Search(search_query);
                    int received = results.Count;

                    if (received == 0)
                    {
                        continue;
                    }

                    // Get last date.

                    UniqueId last_id = new UniqueId();
                    DateTimeOffset last_time = new DateTimeOffset(new DateTime(2000, 1, 1));

                    foreach (var uniqueId in results)
                    {
                        message = inbox.GetMessage(uniqueId);

                        if (message.Date > last_time)
                        {
                            last_time = message.Date;
                            last_id = uniqueId;
                        }
                    }

                    // Extract mail address from body

                    message = inbox.GetMessage(last_id);

                    url = extract_email_verification_url_from_body(message);
                    if (url != "")
                        break;
                }

            }
            catch (Exception ex)
            {
                MainApp.log_error($"Exception Error ({System.Reflection.MethodBase.GetCurrentMethod().Name}): {ex.Message}");
            }

            return url;
        }
        public string extract_email_sender_transactioncode(string receiver_mail, string receiver_fullname, int delay_time = 300000)
        {
            string transaction_code = "";
            int repeat_count;
            MimeKit.MimeMessage message;

            try
            {
                for (repeat_count = 0; repeat_count < delay_time; repeat_count += 1000, System.Threading.Thread.Sleep(1000))
                {
                    DateTime time_to = DateTime.Now;
                    DateTime time_from = time_to - new TimeSpan(1, 0, 0, 0);
                    var inbox = client.Inbox;
                    inbox.Open(FolderAccess.ReadWrite);

                    SearchQuery search_query = SearchQuery.SubjectContains("Sie haben eine Zahlung gesendet").And(SearchQuery.FromContains("service@paypal.de"));
                    //SearchQuery search_query1 = SearchQuery.DeliveredAfter(time_from).And(SearchQuery.DeliveredBefore(time_to));
                    //search_query = search_query.And(search_query1);
                    var results = inbox.Search(search_query);
                    int received = results.Count;

                    if (received == 0)
                    {
                        continue;
                    }

                    // Get last date.

                    UniqueId last_id = new UniqueId();
                    DateTimeOffset last_time = new DateTimeOffset(new DateTime(2000, 1, 1));

                    foreach (var uniqueId in results)
                    {
                        message = inbox.GetMessage(uniqueId);

                        if (message.Date > last_time)
                        {
                            last_time = message.Date;
                            last_id = uniqueId;
                        }
                    }

                    // Extract mail address from body

                    message = inbox.GetMessage(last_id);

                    transaction_code = extract_email_transaction_code_from_body(message, receiver_mail, receiver_fullname);
                    if (transaction_code != "")
                        break;
                }

            }
            catch (Exception ex)
            {
                MainApp.log_error($"Exception Error ({System.Reflection.MethodBase.GetCurrentMethod().Name}): {ex.Message}");
            }

            return transaction_code;
        }

        public bool extract_email_block_info()
        {
            bool is_blocked = false;
            MimeKit.MimeMessage message;

            try
            {                
                DateTime time_to = DateTime.Now;
                DateTime time_from = time_to - new TimeSpan(1, 0, 0, 0);
                var inbox = client.Inbox;
                inbox.Open(FolderAccess.ReadWrite);

                SearchQuery search_query = SearchQuery.SubjectContains("Ihr PayPal-Konto erfordert Ihre Mitarbeit").And(SearchQuery.FromContains("service@paypal.com"));

                var results = inbox.Search(search_query);
                int received = results.Count;

                if (received == 0)
                {
                    return is_blocked;
                }

                // Get last date.

                UniqueId last_id = new UniqueId();
                DateTimeOffset last_time = new DateTimeOffset(new DateTime(2000, 1, 1));

                foreach (var uniqueId in results)
                {
                    message = inbox.GetMessage(uniqueId);

                    if (message.Date > last_time)
                    {
                        last_time = message.Date;
                        last_id = uniqueId;
                    }
                }

                // Extract mail address from body

                message = inbox.GetMessage(last_id);

                is_blocked = extract_email_block_info_from_body(message);

            }
            catch (Exception ex)
            {
                MainApp.log_error($"Exception Error ({System.Reflection.MethodBase.GetCurrentMethod().Name}): {ex.Message}");
            }

            return is_blocked;
        }
        public bool extract_email_block_info_from_body(MimeKit.MimeMessage message)
        {
            bool is_blocked = false;
            string base64 = "";
            string[] lines;

            try
            {
                MainApp.log_info($"message.BodyParts.Count() = {message.BodyParts.Count()}");
                
                foreach (var part in message.BodyParts)
                {
                    MainApp.log_info($"part.ContentType.ToString() = {part.ContentType.ToString()}");

                    if (part.ContentType.ToString() != "Content-Type: text/html; charset=\"UTF-8\"")
                    {
                        continue;
                    }

                    string part_str = part.ToString();
                    lines = part_str.Split('\n');

                    MainApp.log_info($"lines.Length = {lines.Length}");

                    int k = 0;
                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (lines[i] == "\r")
                        {
                            k++;

                            if (k == 1)
                                continue;

                            break;
                        }
                        if (k == 0)
                            continue;

                        base64 += (base64.Length == 0) ? lines[i] : "\n" + lines[i];
                    }
                    break;
                }

                MainApp.log_info($"base64 = {base64}");

                if (base64 == "")
                    return false;

                byte[] decodedBytes = Convert.FromBase64String(base64);
                string decodedText = Encoding.UTF8.GetString(decodedBytes);

                //add_log("decodedText = {0}", decodedText);

                lines = decodedText.Split('\n');

                bool flag1 = false;
                bool flag2 = false;

                foreach (string line in lines)
                {
                    if (line.Contains("Ihr PayPal-Konto ist vorübergehend eingeschränkt."))
                    {
                        flag1 = true;
                        break;
                    }
                }

                foreach (string line in lines)
                {
                    if (line.Contains("Warum haben wir Ihr Konto eingeschränkt?"))
                    {
                        flag2 = true;
                        break;
                    }
                }
                is_blocked = flag1 || flag2;
                return is_blocked;

            }
            catch (Exception ex)
            {
                MainApp.log_error(ex.Message);
                is_blocked = false;
            }

            return is_blocked;
        }
        public string check_email_verify_url()
        {
            try
            {
                string[] fields = Get_Server_Port(login).Split('|');
                mailServer = fields[0];
                port = Int32.Parse(fields[1]);

                if (!connect(mailServer, port, true, login, password))
                {
                    return "";
                }
                
                string url = extract_email_verification_url();
                MainApp.log_error($"Extract_email_verification_url ret = {url}");

                disconnect();
                return url;
            }
            catch (Exception exception)
            {
                MainApp.log_error($"Exception Error ({System.Reflection.MethodBase.GetCurrentMethod().Name}): {exception.Message}");
            }

            return "";
        }
        public string check_email_sender_transCode(string receiver_mail, string receiver_fullname)
        {
            try
            {
                string[] fields = Get_Server_Port(login).Split('|');
                mailServer = fields[0];
                port = Int32.Parse(fields[1]);

                if (!connect(mailServer, port, true, login, password))
                {
                    return "";
                }

                string transaction_code = extract_email_sender_transactioncode(receiver_mail, receiver_mail);
                MainApp.log_error($"Extract_email_transaction_code ret = {transaction_code}");

                disconnect();
                return transaction_code;
            }
            catch (Exception exception)
            {
                MainApp.log_error($"Exception Error ({System.Reflection.MethodBase.GetCurrentMethod().Name}): {exception.Message}");
            }

            return "";
        }
        public int check_email_block_info()
        {
            try
            {
                string[] fields = Get_Server_Port(login).Split('|');
                mailServer = fields[0];
                port = Int32.Parse(fields[1]);

                if (!connect(mailServer, port, true, login, password))
                    return ConstEnv.ACCOUNT_MAIL_STATUS_FAILED;

                bool is_blocked = extract_email_block_info();
                MainApp.log_info($"Block info ret = {is_blocked.ToString()}");

                disconnect();

                if (is_blocked)
                    return ConstEnv.ACCOUNT_MAIL_STATUS_BLOCK;

                return ConstEnv.ACCOUNT_MAIL_STATUS_NORMAL;
            }
            catch (Exception exception)
            {
                MainApp.log_error($"Exception Error ({System.Reflection.MethodBase.GetCurrentMethod().Name}): {exception.Message}");
            }

            return 0;
        }

        public string Get_Server_Port(string mail)
        {
            int pos = mail.IndexOf("@");
            mail = mail.Substring(pos);
            string result = MainApp.g_setting.MailServer_Info[mail];

            return result;
        }        
    }
}
