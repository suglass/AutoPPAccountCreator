using DbHelper.DbBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAuto.PayPal_Auto
{
    partial class AutoPro
    {
        public async Task<bool> LogIn()
        {
            try
            {
                MainApp.log_info($"#{m_ID} - Login started. account = {m_param.account.mail}");
                string cur_url = WebDriver.Url;
                while (true)
                {
                    retry = 0;
                    xpath = "//input[@id='email' and @name='login_email']";
                    MainApp.log_info($"{m_param.account.mail} - In first page.");
                    MainApp.log_info($"{m_param.account.mail} - Repeat {retry}.");

                    flag = await Wait_for_Nextstep_By_Path(xpath, timeout);
                    if (!flag)
                        return false;

                    if (m_must_terminate)
                    {
                        return false;
                    }

                    MainApp.log_info($"{m_param.account.mail} - First page is correct.");
                    flag = await LogIn_First_Page();
                    if (flag && await WaitUrlChange(cur_url, timeout))                        
                        break;
                    else
                    {
                        retry += 1;
                        if (retry == 2)
                            return false;
                    }
                }
                /////////////////////////////////////////////////////////////////////////////
                // 1-1. After login, getting real money balance.
                /////////////////////////////////////////////////////////////////////////////
                await TaskDelay(5000);
                m_real_money_balance = await Get_Real_Money();
                if (m_real_money_balance != -1)
                {
                    MainApp.g_db.set_real_money_balance(m_param.account.id, m_real_money_balance);
                }
                return true;
            }
            catch (Exception e)
            {
                MainApp.log_error($"{m_param.account.mail} - In LogIn, error catched : {e.Message}");
                return false;
            }

        }
        public async Task<bool> LogIn_First_Page()
        {
            try
            {
                if (!await WaitToPresent("//input[@id='password' and @name='login_password']", 2000))
                {
                    xpath = "//input[@id='email' and @name='login_email']";                   // input the email
                    await TryEnterText_by_xpath(xpath, m_param.account.mail, "value", 3000, true);

                    xpath = "//button[@id='btnNext' and @name='btnNext']";
                    await TryClickByPath(xpath, 1);
                }

                if (m_must_terminate)
                {
                    return false;
                }

                xpath = "//input[@id='password' and @name='login_password']";             // input the password
                await TryEnterText_by_xpath(xpath, m_param.account.paypal_password, "value", 5000, true);

                xpath = "//button[@id='btnLogin' and @name='btnLogin']";                  // submit button click
                await TryClickByPath(xpath, 1);

                return true;
            }
            catch (Exception ex)
            {
                MainApp.log_info($"#{m_ID} : Error in first page : {ex.Message}");
                return false;
            }
        }        
    }
}
