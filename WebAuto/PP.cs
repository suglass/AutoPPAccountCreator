using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Cookie = System.Net.Cookie;
using Size = System.Drawing.Size;
using WebAuto;
using WebAuto.BaseModule;
using WebAuto.WebHelper;
using WebAuto.Utils;

namespace WebAuto
{
    public class PP : IWebHelper
    {
                
        
        

        public async Task<bool> Login_Buildup()
        {
            try
            {

                return false;
            }
            catch(Exception e)
            {
                MainApp.log_error($"#{m_ID} - {m_param.mail} - Error catched : {e.Message}");
                return false;
            }
        }        
    }
}
