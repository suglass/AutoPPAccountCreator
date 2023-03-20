using Newtonsoft.Json;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebAuto.Utils;

namespace WebAuto.WebHelper
{
    partial class IWebHelper
    {
        public async Task<string> Get_site_key()
        {
            string site_key = "";
            try
            {
//                 foreach (IWebElement elem in WebDriver.FindElementsByTagName("div"))
//                 {
//                     site_key = elem.GetAttribute("data-sitekey");
//                     if (site_key != null)
//                     {
//                         MainApp.log_info($"Site key is found from div tags : {site_key}");
//                         return site_key;
//                     }                        
//                 }

                string frame_name = Get_self_name();
                MainApp.log_info($"In Get site key, the frame name is : {frame_name}");
                if (frame_name != "recaptcha")
                    WebDriver.SwitchTo().Frame("recaptcha");
                int retry_num = 0;
                while (site_key == "" && retry_num <= 30)
                {
                    retry_num += 1;
                    foreach (IWebElement elem in WebDriver.FindElementsByTagName("iframe"))
                    {
                        try
                        {
                            MainApp.log_info($"Checking iframe tags.");
                            int pos = elem.GetAttribute("src").IndexOf("https://www.google.com/recaptcha/api2/anchor?ar=1&k=");
                            if (pos > -1)
                            {                                
                                site_key = elem.GetAttribute("src").Substring("https://www.google.com/recaptcha/api2/anchor?ar=1&k=".Length);
                                int pos_1 = site_key.IndexOf('&');
                                site_key = site_key.Substring(0, pos_1);
                                MainApp.log_info($"site key : {site_key}");
                                break;
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    await TaskDelay(1000);
                }
            }
            catch (Exception e)
            {
                MainApp.log_error($"Error catched : {e.Message}");
            }
            return site_key;
        }
        public async Task<string> Get_ID_from_site_key(string site_key)
        {
            try
            {
                string page_url = WebDriver.Url;
                string in_url = $"https://2captcha.com/in.php?key=86f75378dfe8e6bfad50e5ea37c61182&method=userrecaptcha&googlekey={site_key}&pageurl={page_url}";
                var w = new WebClient();
                string response_string = w.DownloadString(in_url);

                string[] fields = response_string.Split('|');
                string id = "";
                id = fields[1];
                return id;
            }
            catch (Exception e)
            {
                MainApp.log_error($"Error catched : {e.Message}");
            }
            return null;
        }
        public async Task<string> Get_Id(Dictionary<string, string> dict)
        {
            try
            {
                string in_url = "https://2captcha.com/in.php";

                HttpWebRequest request;
                request = (HttpWebRequest)HttpWebRequest.Create(in_url);

                var json_data = JsonConvert.SerializeObject(dict);

                request.Method = "POST";
                request.ContentType = "application/json";
                request.ContentLength = json_data.Length;                
                request.UserAgent = Str_Utils.GetRandomUserAgent();
                request.Accept = "*/*";

                var bytes_data = Encoding.ASCII.GetBytes(json_data);

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(bytes_data, 0, bytes_data.Length);
                }

                HttpWebResponse response = null;
                response = (HttpWebResponse)request.GetResponse();
                string response_string = new StreamReader(response.GetResponseStream()).ReadToEnd();

                string[] fields = response_string.Split('|');
                string id = "";
                id = fields[1];
                return id;
            }
            catch (Exception ex)
            {
                MainApp.log_info($"Error catched : {ex.Message}");
                return null;
            }
        }
        public async Task<string> Get_captcha_string_from_Id(string id)
        {
            try
            {
                string str_data_res = "";
                var w = new WebClient();
                w.Headers.Add(HttpRequestHeader.UserAgent, Str_Utils.GetRandomUserAgent());
                //w.Proxy = new WebProxy
                string res_url = "https://2captcha.com/res.php?key=86f75378dfe8e6bfad50e5ea37c61182&action=get&id=";

                Stopwatch sub_wt = new Stopwatch();
                sub_wt.Start();
                bool flag = true;
                do
                {
                    MainApp.log_info($"Thread #{m_ID} - response started.");
                    if (flag == true)
                    {
                        await Task.Delay(15000);
                        flag = false;
                    }
                    str_data_res = w.DownloadString(res_url + id);
                    int pos = Array.IndexOf(MainApp.g_setting.error_res_list, str_data_res);
                    if (pos > -1)
                    {
                        await Task.Delay(5000);
                    }
                    else
                        break;
                } while (sub_wt.ElapsedMilliseconds < 300000);

                string[] res_fields = str_data_res.Split('|');
                string captcha_string = res_fields[1];

                return captcha_string;
            }
            catch (Exception ex)
            {
                MainApp.log_error($"Error catched in Get_captcha_string_from_Id : {ex.Message}");
                return null;
            }
        }
        public async Task<bool> Try_google_captcha()
        {
            try
            {
                string site_key = await Get_site_key();

                if (site_key == "")
                    return false;

                string id = await Get_ID_from_site_key(site_key);
                MainApp.log_info($"{m_param.account.mail} - Captcha ID : {id}");

                string captcha_Str = await Get_captcha_string_from_Id(id);
                MainApp.log_info($"{m_param.account.mail} - Captcha string : {captcha_Str}");

                string frame_name = Get_self_name();
                if (frame_name != "recaptcha")
                    WebDriver.SwitchTo().Frame("recaptcha");

                string script = $"document.getElementById('g-recaptcha-response').innerHTML='{captcha_Str}'";
                m_js.ExecuteScript(script);
                m_js.ExecuteScript(string.Format("{0}(\"{1}\");", "verifyCallback", captcha_Str));
                return true;
            }
            catch (Exception e)
            {
                MainApp.log_error($"Error catched in Try_google_captcha: {e.Message}");
            }
            return false;
        }

    }
}
