using DbHelper;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using ResourcesInApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAuto.PayPal_Auto
{
    partial class AutoPro
    {        
        public bool result;
        public async Task<bool> create_Account()
        {
            try
            {
                MainApp.log_info($"#{m_ID} - Registration started. account = {m_param.account.mail}");

                while (true)
                {
                    retry = 0;
                    xpath = "//input[@id='paypalAccountData_firstName']";
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
                    flag = await CreAcc_First_Page();
                    if (flag)
                        break;
                    else
                    {
                        retry += 1;
                        if (retry == 2)
                            return false;
                    }
                }

                while (true)
                {
                    retry = 0;
                    xpath = "//input[@id='paypalAccountData_phone' and @type='tel']";
                    MainApp.log_info($"{m_param.account.mail} - In second page.");
                    MainApp.log_info($"{m_param.account.mail} - Repeat {retry}.");

                    flag = await Wait_for_Nextstep_By_Path(xpath, timeout);
                    if (!flag)
                        return false;

                    if (m_must_terminate)
                    {
                        return false;
                    }

                    MainApp.log_info($"{m_param.account.mail} - Second page is correct.");
                    flag = await CreAcc_Second_Page();
                    if (flag)
                        break;
                    else
                    {
                        retry += 1;
                        if (retry == 2)
                            return false;
                    }
                }
                /////////////////////////////////////////////////////////////////////////////
                // 2-1. Waiting url for clicking button.
                /////////////////////////////////////////////////////////////////////////////

                string url_3rd_page = "https://www.paypal.com/welcome/signup/#/intent_selection";
                xpath = "//button[@data-automation-id='send_receive_sell' and @value='send_money_intent']";

                flag = await WaitUrlSame(url_3rd_page, timeout);
                if (!flag)
                    return false;
                //                 flag = await WaitToPresentByPath(xpath, 3000);
                //                 if (!flag)
                //                     return false;

                if (m_must_terminate)
                {
                    return false;
                }

                await TaskDelay(5000);
                MainApp.log_info($"{m_param.account.mail} - Successfully went into third page.");

                m_process_result = ConstEnv.CREATE_INPUT_SUCCESS;
                /////////////////////////////////////////////////////////////////////////////
                // 3. Go to dashboard using url exchange.
                /////////////////////////////////////////////////////////////////////////////

                await Navigate(MainApp.g_setting.dashboard_url);
                //WebDriver.Navigate().GoToUrl(MainApp.g_setting.dashboard_url);
                if (m_must_terminate)
                {
                    return false;
                }
                await TaskDelay(5000);
                MainApp.log_info($"{m_param.account.mail} - Went to dashboard.");
                if (m_must_terminate)
                {
                    return false;
                }
                await TaskDelay(5000);

                /////////////////////////////////////////////////////////////////////////////
                // 4. Mail verification.
                /////////////////////////////////////////////////////////////////////////////

                string verify_url = m_clsMailChecker.check_email_verify_url();
                MainApp.log_info($"{m_param.account.mail} - Email url : ");
                if (verify_url == "")
                    return false;
                MainApp.log_info($"{m_param.account.mail} - Verify URL - {verify_url}");

                if (m_must_terminate)
                {
                    return false;
                }

                await TaskDelay(5000);
                await Navigate(verify_url);

                m_process_result = ConstEnv.CREATE_MAIL_SUCCESS;

                /////////////////////////////////////////////////////////////////////////////
                // 5. After verify, log in page.
                /////////////////////////////////////////////////////////////////////////////
                xpath = "//input[@id='password' and @name='login_password']";

                if (!await WaitToPresentByPath(xpath, timeout))
                    return false;
                
                await TryEnterText_by_xpath(xpath, m_param.account.paypal_password, "value", 3000, true);
                xpath = "//button[@name='btnLogin' and @type='submit']";
                await TryClick(By.XPath(xpath), 1);

                if (m_must_terminate)
                {
                    return false;
                }
                await TaskDelay(3000);

                /////////////////////////////////////////////////////////////////////////////
                m_process_result = ConstEnv.CREATE_FINAL_SUCCESS;

                MainApp.log_info($"#{m_ID} - {m_param.account.mail} - Registration success!");
                m_cookies = ConvertSeleniumCookieToCookieContainer(WebDriver.Manage().Cookies);

                m_lst_cookies = Convert_SeleniumCookie_To_StringList(WebDriver.Manage().Cookies);

                if (m_must_terminate)
                {
                    return false;
                }

                await Task.Delay(7000);
                return true;
            }
            catch (Exception ex)
            {
                MainApp.log_info(string.Format("Catch Exception: {0}", ex.Message));
                return false;
            }
        }

        public async Task<bool> CreAcc_First_Page()
        {
            try
            {
                xpath = "//input[@id='paypalAccountData_firstName']";           // input the first name
                await TryEnterText_by_xpath(xpath, m_param.account.first_name, "value", 3000, true);

                xpath = "//input[@id='paypalAccountData_lastName']";            // input the last name
                await TryEnterText_by_xpath(xpath, m_param.account.last_name, "value", 3000, true);

                xpath = "//input[@id='paypalAccountData_email']";               // input the email address
                await TryEnterText_by_xpath(xpath, m_param.account.mail, "value", 3000, true);
                /*await TryEnterText_All(By.XPath(xpath), m_param.account.mail, "value", 3000);*/

                if (m_must_terminate)
                {
                    return false;
                }

                result = await WaitToPresent(By.Id("captchaComponent"), 2000) && await WaitToVisible(By.Id("captchaComponent"), 2000);
                if (result == true)
                {
                    if (m_must_terminate)
                    {
                        return false;
                    }

                    MainApp.log_info($"{m_param.account.mail} : Captcha is appeared.");
                    flag = await Try_google_captcha();
                    if (flag == true)
                    {
                        MainApp.log_info($"{m_param.account.mail} : Captcha is bypassed.");
                    }
                    else
                    {
                        MainApp.log_info($"{m_param.account.mail} : Captcha is failed.");
                        return false;
                    }

                    string frame_name = Get_self_name();
                    MainApp.log_info($"{m_param.account.mail} : Frame name is : .");
                    /*
                                        if (frame_name != "")
                                            WebDriver.SwitchTo().DefaultContent();
                    */
                    await TaskDelay(2000);
                }
                else
                    MainApp.log_info($"{m_param.account.mail} : Captcha is not appeared.");

                if (m_must_terminate)
                {
                    return false;
                }

                xpath = "//input[@id='paypalAccountData_password']";            // input the password                
                //await TryEnterText_by_xpath(xpath, m_param.account.paypal_password, "value", 3000, false);
                //await TryEnterText_All(By.XPath(xpath), m_param.account.paypal_password, "value", 3000);
                if (IsElementPresent(By.XPath(xpath)) && await IsElementVisible(By.XPath(xpath)))
                    await TryEnterText_by_xpath(xpath, m_param.account.paypal_password, "value", 3000, true);
                else
                {
                    Actions action = new Actions(WebDriver);
                    action.MoveToElement(WebDriver.FindElement(By.XPath(xpath))).Perform();
                    action.SendKeys(WebDriver.FindElement(By.XPath(xpath)), m_param.account.paypal_password);
                }


                result = await WaitToPresent(By.Id("captchaComponent"), 2000) && await WaitToVisible(By.Id("captchaComponent"), 2000);
                if (result == true)
                {                    
                    if (m_must_terminate)
                    {
                        return false;
                    }
                    MainApp.log_info($"{m_param.account.mail} : Captcha is appeared.");
                    flag = await Try_google_captcha();
                    if (flag == true)
                    {
                        MainApp.log_info($"{m_param.account.mail} : Captcha is bypassed.");
                    }
                    else
                    {
                        MainApp.log_info($"{m_param.account.mail} : Captcha is failed.");
                        return false;
                    }
                    
                    string frame_name = Get_self_name();
                    MainApp.log_info($"{m_param.account.mail} : Frame name is : .");
                    //                     if (frame_name != "")
                    //                         WebDriver.SwitchTo().DefaultContent();
                    await TaskDelay(2000);
                }
                else
                    MainApp.log_info($"{m_param.account.mail} : Captcha is not appeared.");

                xpath = "//input[@id='paypalAccountData_confirmPassword']";     // input the confirm password

                if (IsElementPresent(By.XPath(xpath)) && await IsElementVisible(By.XPath(xpath)))
                    await TryEnterText_by_xpath(xpath, m_param.account.paypal_password, "value", 3000, true);
                else
                {
                    Actions action = new Actions(WebDriver);
                    action.MoveToElement(WebDriver.FindElement(By.XPath(xpath))).Perform();
                    action.SendKeys(WebDriver.FindElement(By.XPath(xpath)), m_param.account.paypal_password);
                }
                //await TryEnterText_All(By.XPath(xpath), m_param.account.paypal_password, "value", 3000);
                
                xpath = "//button[@type='submit' and @value='name_address']";   // submit button, link text = "Weiter", data-automation-id="page_submit"
                //await TryClick(By.XPath(xpath), 1);
                await TryClick_All(xpath);

                if (m_must_terminate)
                {
                    return false;
                }

                result = await WaitToPresent(By.Id("captchaComponent"), 2000) && await WaitToVisible(By.Id("captchaComponent"), 2000);
                if (result == true)
                {
                    MainApp.log_info($"{m_param.account.mail} : Captcha is appeared.");
                    flag = await Try_google_captcha();
                    if (flag == true)
                    {
                        MainApp.log_info($"{m_param.account.mail} : Captcha is bypassed.");
                    }
                    else
                    {
                        MainApp.log_info($"{m_param.account.mail} : Captcha is failed.");
                        return false;
                    }

                    string frame_name = Get_self_name();
                    MainApp.log_info($"{m_param.account.mail} : Frame name is : .");
                    await TaskDelay(2000);
                    /*
                                        if (frame_name != "")
                                            WebDriver.SwitchTo().DefaultContent();
                    */
                    xpath = "//button[@type='submit' and @value='name_address']";   // submit button, link text = "Weiter", data-automation-id="page_submit"
                    //await TryClick(By.XPath(xpath), 1);
                    await TryClick_All(xpath);
                }
                else
                    MainApp.log_info($"{m_param.account.mail} : Captcha is not appeared.");

                return true;
            }
            catch(Exception ex)
            {
                MainApp.log_info($"#{m_ID} : Error in first page : {ex.Message}");
                return false;
            }
        }

        public async Task<bool> CreAcc_Second_Page()
        {
            try
            {
                xpath = "//input[@id='paypalAccountData_phone' and @type='tel']";   // input phone number
                await TryEnterText_by_xpath(xpath, m_param.account.tel, "value", 3000, true);

                xpath = "//input[@id='paypalAccountData_addressSuggest']";                // input address
                if (!await TryEnterText_by_xpath(xpath, m_param.account.street, "value", 3000, true))
                {
                    xpath = "//input[@id='paypalAccountData_address1']";                // input address1
                    await TryEnterText_by_xpath(xpath, m_param.account.street, "value", 3000, true);
                }
                //                 xpath = "//input[@id='paypalAccountData_address2']";                // input address2
                //                 await TryEnterText_by_xpath(xpath, m_param.account.address2, "value", 3000, true);

                if (m_must_terminate)
                {
                    return false;
                }

                result = await WaitToPresent(By.Id("captchaComponent"), 2000) && await WaitToVisible(By.Id("captchaComponent"), 2000);
                if (result == true)
                {
                    MainApp.log_info($"{m_param.account.mail} : Captcha is appeared.");
                    flag = await Try_google_captcha();
                    if (flag == true)
                    {
                        MainApp.log_info($"{m_param.account.mail} : Captcha is bypassed.");
                    }
                    else
                    {
                        MainApp.log_info($"{m_param.account.mail} : Captcha is failed.");
                        return false;
                    }

                    string frame_name = Get_self_name();
                    MainApp.log_info($"{m_param.account.mail} : Frame name is : .");
                    /*
                                        if (frame_name != "")
                                            WebDriver.SwitchTo().DefaultContent();
                    */
                    await TaskDelay(2000);
                }
                else
                    MainApp.log_info($"{m_param.account.mail} : Captcha is not appeared.");

                xpath = "//input[@id='paypalAccountData_zip']";                     // input PLZ
                //await TryEnterText_by_xpath(xpath, m_param.account.postcode, "value", 3000, true);
                //await TryEnterText_All(By.XPath(xpath), m_param.account.postcode, "value", 3000);
                if (IsElementPresent(By.XPath(xpath)) && await IsElementVisible(By.XPath(xpath)))
                    await TryEnterText_by_xpath(xpath, m_param.account.postcode, "value", 3000, true);
                else
                {
                    Actions action = new Actions(WebDriver);
                    action.MoveToElement(WebDriver.FindElement(By.XPath(xpath))).Perform();
                    action.SendKeys(WebDriver.FindElement(By.XPath(xpath)), m_param.account.postcode);
                }

                xpath = "//input[@id='paypalAccountData_city']";                    // input city
                //await TryEnterText_by_xpath(xpath, m_param.account.city, "value", 3000, true);
                //await TryEnterText_All(By.XPath(xpath), m_param.account.city, "value", 3000);
                if (IsElementPresent(By.XPath(xpath)) && await IsElementVisible(By.XPath(xpath)))
                    await TryEnterText_by_xpath(xpath, m_param.account.city, "value", 3000, true);
                else
                {
                    Actions action = new Actions(WebDriver);
                    action.MoveToElement(WebDriver.FindElement(By.XPath(xpath))).Perform();
                    action.SendKeys(WebDriver.FindElement(By.XPath(xpath)), m_param.account.city);
                }

                if (m_must_terminate)
                {
                    return false;
                }

                xpath = "//input[@id='paypalAccountData_dob']";                     // input date (dd.mm.yyyy)
                DateTime birthday = PaypalDbHelper.str_2_time(m_param.account.birthday);
                //await TryEnterText_by_xpath(xpath, birthday.ToString("ddMMyyyy"), "value", 3000, true);
                //await TryEnterText_All(By.XPath(xpath), birthday.ToString("ddMMyyyy"), "value", 3000);
                if (IsElementPresent(By.XPath(xpath)) && await IsElementVisible(By.XPath(xpath)))
                    await TryEnterText_by_xpath(xpath, birthday.ToString("ddMMyyyy"), "value", 3000, true);
                else
                {
                    Actions action = new Actions(WebDriver);
                    action.MoveToElement(WebDriver.FindElement(By.XPath(xpath))).Perform();
                    action.SendKeys(WebDriver.FindElement(By.XPath(xpath)), birthday.ToString("ddMMyyyy"));
                }

                xpath = "//input[@type='checkbox' and @id='paypalAccountData_tcpa']";   // check the third checkbox
                string status = WebDriver.FindElementByXPath(xpath).GetAttribute("aria-invalid");
                MainApp.log_info($"Status : {status}");
                                
                //await TryClickByPath(xpath, 0);
                await TryClick_All(xpath);
                                    

                xpath = "//button[@value='submit_account_create']";      // link text = "Zustimmen und Konto eröffnen", type = "submit"
                //await TryClickByPath(xpath, 2);
                await TryClick_All(xpath);

                if (m_must_terminate)
                {
                    return false;
                }

                await TaskDelay(3000);

                return true;
            }
            catch(Exception ex)
            {
                MainApp.log_info($"#{m_ID} : Error in second page : {ex.Message}");
                return false;
            }
        }
    }
}
