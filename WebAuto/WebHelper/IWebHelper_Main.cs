using DbHelper.DbBase;
using Microsoft.Win32;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Resolution;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebAuto.Utils;

namespace WebAuto.WebHelper
{
    public partial class IWebHelper
    {
        public ChromeDriver WebDriver;
        public IJavaScriptExecutor m_js;
        public CookieContainer m_cookies;
        public List<string> m_lst_cookies;
        public int m_ID;
        public bool m_incognito = true;
        public bool m_dis_webrtc = false;
        public bool m_dis_cache = false;
        public bool m_dis_js = false;
        public object m_locker;
        public string m_chr_user_data_dir;
        public bool m_must_terminate;

        public int m_useragent_id;
        public string m_resolution;

        public System.Drawing.Point m_location = new System.Drawing.Point(0, 0);
        public System.Drawing.Size m_size = new System.Drawing.Size(0, 0);

        public string m_chr_extension_dir = Environment.CurrentDirectory + "\\ChromeExtension";
        public string m_creat_time;
        
        public object m_chr_data_dir = new object();
        public object m_selen_locker = new object();

        public WorkerParam m_param;

        public IWebHelper()
        {
            WebDriver = null;
            m_locker = new object();
            m_chr_user_data_dir = "";
            m_must_terminate = false;
            m_resolution = "";
        }

        public async Task<bool> Start(int flag)                 // create flag = 1, login flag = 2
        {
            try
            {
                lock (m_chr_data_dir)
                {                    
                    m_chr_user_data_dir = $"ChromeData\\selenium_{m_param.account.id}";
                    if (flag == 1)
                        Directory.CreateDirectory(m_chr_user_data_dir);
                }
                
                try
                {
                    ChromeDriverService defaultService = ChromeDriverService.CreateDefaultService();
                    defaultService.HideCommandPromptWindow = true;
                    ChromeOptions chromeOptions = new ChromeOptions();
                    if (m_incognito)
                    {
                        chromeOptions.AddArguments("--incognito");
                    }

                    chromeOptions.AddArgument("--start-maximized");
                    //chromeOptions.AddArgument("--auth-server-whitelist");
                    chromeOptions.AddArgument("--ignore-certificate-errors");
                    chromeOptions.AddArgument("--ignore-ssl-errors");
                    chromeOptions.AddArgument("--system-developer-mode");
                    chromeOptions.AddArgument("--no-first-run");
                    //chromeOptions.AddArguments("--disk-cache-size=0");
                    chromeOptions.AddArgument("--load-extension=" + m_chr_extension_dir + "\\proxy helper");
                    chromeOptions.AddArgument("--user-data-dir=" + m_chr_user_data_dir);
                    
                    chromeOptions.AddExcludedArgument("enable-automation");
                    chromeOptions.AddArguments("--disable-infobars");
                    chromeOptions.AddArguments("--disable-popup-blocking");

                    chromeOptions.AddArgument("--lang=de-DE");
                    chromeOptions.AddArgument("--lang=de");


                    if (m_dis_webrtc)
                        chromeOptions.AddExtension(m_chr_extension_dir + "\\WebRTC Protect.crx");
                    if (m_dis_cache)
                        chromeOptions.AddExtension(m_chr_extension_dir + "\\CacheKiller.crx");

                    if (m_dis_js)
                        chromeOptions.AddArgument("--load-extension=" + m_chr_extension_dir + "\\jsoff-master");

                    // Change resolution.

                    if (flag == 1)
                    {                        
                        string cur = CResolution.GetCurrentResolutionString();
                        MainApp.log_info($"New resolution is {m_resolution}.");

                        List<string> avail_resolutions = MainApp.g_setting.resolutions.Intersect(MainApp.g_setting.random_resolutions).ToList<string>();
                        if (avail_resolutions.Contains(cur))
                            avail_resolutions.Remove(cur);
                        if (avail_resolutions.Any())
                        {
                            MainApp.log_info("Intersect between resolutions is existed.");
                            m_resolution = avail_resolutions[new Random().Next(0, avail_resolutions.Count - 1)];

                            MainApp.log_info($"Resolution will be changed to {m_resolution}.");
                            if (!CResolution.ChangeResolution(m_resolution))
                            {
                                m_resolution = cur;
                            }

                            MainApp.g_db.set_account_resolution(m_param.account.id, m_resolution);
                        }
                        else
                        {
                            MainApp.log_info("Intersect between resolutions is not existed. Use current resolution.");
                            m_resolution = cur;
                        }
                        MainApp.g_db.set_account_resolution(m_param.account.id, cur);
                    }
                    else if(flag == 2)
                    {                        
                        m_resolution = MainApp.g_db.get_account_resolution(m_param.account.id);
                        if (MainApp.g_setting.resolutions.Contains(m_resolution))
                            CResolution.ChangeResolution(m_resolution);
                    }

                    // Set useragent.

                    if (flag == 1)
                    {
                        m_useragent_id = MainApp.g_db.get_random_user_agent_id();
                        if (m_useragent_id == -1)
                        {
                            MainApp.log_error("No user agents.");
                            return false;
                        }

                        string user_agent = MainApp.g_db.get_user_agent_from_id(m_useragent_id);
                        if (user_agent == "")
                        {
                            MainApp.log_error("User agent is null.");
                            return false;
                        }
                        chromeOptions.AddArgument(string.Format("--user-agent={0}", (object)user_agent));
                    }
                    else if (flag == 2)
                    {
                        string user_agent = MainApp.g_db.get_user_agent(m_param.account.id);
                        chromeOptions.AddArgument(string.Format("--user-agent={0}", (object)user_agent));
                    }

                    chromeOptions.SetLoggingPreference(LogType.Driver, LogLevel.All);
                    chromeOptions.AddAdditionalCapability("useAutomationExtension", false);
                    //chromeOptions.AddArguments("--no-sandbox");
                    //chromeOptions.AddUserProfilePreference("profile.managed_default_content_settings.images", 2);

                    string chr_path = "";

                    string reg = @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe";
                    RegistryKey registryKey;
                    using (registryKey = Registry.LocalMachine.OpenSubKey(reg))
                    {
                        if (registryKey != null)
                            chr_path = registryKey.GetValue("Path").ToString() + @"\chrome.exe";
                    }
                    if (chr_path == "")
                    {
                        if (Environment.Is64BitOperatingSystem)
                            chr_path = "C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe";
                        else
                            chr_path = "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe";

                        if (!System.IO.File.Exists(chr_path))
                        {
                            chr_path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Google\Chrome\Application\chrome.exe";
                        }
                    }

                    if (!System.IO.File.Exists(chr_path))
                    {
                        MainApp.log_error($"#{m_ID} - chrome.exe Not found. Perhaps the Google Chrome browser is not installed on this computer.");
                        return false;
                    }
                    chromeOptions.BinaryLocation = chr_path;

                    try
                    {
                        WebDriver = new ChromeDriver(defaultService, chromeOptions);
                    }
                    catch (Exception ex)
                    {
                        MainApp.log_error($"#{m_ID} - Fail to start chrome.exe. Please make sure any other chrome windows are not opened.\n{ex.Message}");
                        return false;
                    }

                    m_js = (IJavaScriptExecutor)WebDriver;
                    
                    int _ScreenWidth = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
                    int _ScreenHeight = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;

                    m_size.Width = _ScreenWidth;
                    m_size.Height = _ScreenHeight;
                    m_location.X = 0;
                    m_location.Y = 0;

                    WebDriver.Manage().Window.Size = m_size;
                    WebDriver.Manage().Window.Position = m_location;

                    if (m_param.proxy != null && !m_incognito) // regular proxy setting
                    {
                        string ip = m_param.proxy.url;
                        string port = m_param.proxy.port.ToString();
                        string login = m_param.proxy.username;
                        string password = m_param.proxy.password;
                        await Navigate("chrome-extension://mnloefcpaepkpmhaoipjkpikbnkmbnic/options.html");
                        m_js.ExecuteScript("$('#http-host').val(\"" + ip + "\")", Array.Empty<object>());
                        m_js.ExecuteScript("$('#http-port').val(\"" + port + "\")", Array.Empty<object>());
                        m_js.ExecuteScript("$('#https-host').val(\"" + ip + "\")", Array.Empty<object>());
                        m_js.ExecuteScript("$('#https-port').val(\"" + port + "\")", Array.Empty<object>());
                        m_js.ExecuteScript("$('#socks-host').val(\"" + ip + "\")", Array.Empty<object>());
                        m_js.ExecuteScript("$('#socks-port').val(\"" + port + "\")", Array.Empty<object>());
                        m_js.ExecuteScript("$('#username').val(\"" + login + "\")", Array.Empty<object>());
                        m_js.ExecuteScript("$('#password').val(\"" + password + "\")", Array.Empty<object>());
                        if (MainApp.g_db.get_proxy_type(m_param.proxy.id) == "socks5")
                        {
                            m_js.ExecuteScript("var a = document.getElementById(\"socks5\"); a.click();", Array.Empty<object>());
                            MainApp.log_info("Socks5 proxy is set.");
                        }
                        else
                        {
                            m_js.ExecuteScript("var a = document.getElementById(\"socks4\"); a.click();", Array.Empty<object>());
                            MainApp.log_info("Socks4 proxy is set.");
                        }
                        
                        m_js.ExecuteScript("$('#proxy-rule').val(\"singleProxy\");", Array.Empty<object>());
                        m_js.ExecuteScript("save();", Array.Empty<object>());

                        bool is_success = false;
                        while (!is_success)
                        {
                            try
                            {
                                WebDriver.Navigate().GoToUrl("chrome-extension://mnloefcpaepkpmhaoipjkpikbnkmbnic/popup.html");
                                m_js.ExecuteScript("httpProxy();", Array.Empty<object>());
                                is_success = true;
                            }
                            catch (Exception ex)
                            {
                                is_success = false;
                                await Task.Delay(100);
                            }
                        }
                    }
                    //if (!m_incognito && m_dis_js) // regular proxy setting
                    //{
                    //    await Navigate("chrome-extension://jfpdlihdedhlmhlbgooailmfhahieoem/options.html");
                    //}


                    //if (m_incognito == false)
                    //    await remove_all_cookies(); //<- not necessary in incogneto mode

                    MainApp.log_error($"#{m_ID} - Successfully started.");
                    return true;
                }
                catch (Exception ex)
                {
                    MainApp.log_error($"#{m_ID} - Failed to start. Exception:{ex.Message}\n{ex.StackTrace}");
                    try
                    {
                        WebDriver.Quit();
                    }
                    catch
                    {
                        MainApp.log_error($"#{m_ID} - Failed to quit driver. Exception:{ex.Message}");
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                MainApp.log_error($"#{m_ID} - Exception occured while trying to start chrome driver. Exception:{ex.Message}");
            }
            return false;
        }
    }
}
