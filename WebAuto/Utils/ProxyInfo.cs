using DbHelper.DbBase;
using Newtonsoft.Json;
using ResourcesInApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebAuto.Utils
{
    public class ProxyInfo
    {
        public string ass;
        public string city;
        public string country;
        public string countryCode;
        public string isp;
        public string lat;
        public string lon;
        public string org;
        public string query;
        public string region;
        public string regionName;
        public string status;
        public string timezone;
        public string zip;

        public ProxyInfo(string assp, string cityp, string countryp, string countrycodep, string ispp, string latp, string lonp, string orgp, string queryp, string regionp, string regionNamep, string statusp, string timezonep, string zipp)
        {
            ass = assp;
            city = cityp;
            country = countryp;
            countryCode = countrycodep;
            isp = ispp;
            lat = latp;
            lon = lonp;
            org = orgp;
            query = queryp;
            region = regionp;
            regionName = regionNamep;
            status = statusp;
            timezone = timezonep;
            zip = zipp;
        }
        private static string extract_proxy_server_ip_from_url(string url)
        {
            string ip = "";

            Match match = Regex.Match(url, @"^(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])$");
            if (match.Success)
                ip = match.Value;

            return ip;
        }
        public static bool get_proxy_info(string url, out string country, out string city, out string isp)
        {
            try
            {
                country = "";
                city = "";
                isp = "";

                string ip = extract_proxy_server_ip_from_url(url);
                if (ip == "")
                    return false;
                string ip_api_url = string.Format(ConstEnv.PROXY_IP_API_FORMAT, ip, ConstEnv.PROXY_IP_API_KEY);
                MainApp.log_info($"get_proxy_info : ip_api_url = {ip_api_url}");
                var w = new WebClient();
                string response_json = w.DownloadString(ip_api_url);
                MainApp.log_info($"get_proxy_info : response_json = {response_json}");
                ProxyInfo proxy_info = JsonConvert.DeserializeObject<ProxyInfo>(response_json);
                country = proxy_info.country;
                city = proxy_info.city;
                isp = proxy_info.isp;
                MainApp.log_info($"get_proxy_info : country = {country}, city = {city}, isp = {isp}");
            }
            catch (Exception exception)
            {
                Console.WriteLine(string.Format("Exception Error ({0}): {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, exception.Message));

                country = "";
                city = "";
                isp = "";
                return false;
            }
            return true;
        }
    }
}
