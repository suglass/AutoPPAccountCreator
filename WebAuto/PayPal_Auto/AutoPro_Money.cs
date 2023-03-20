using DbHelper.DbBase;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAuto.Utils;

namespace WebAuto.PayPal_Auto
{
    partial class AutoPro
    {
        public async Task<double> Get_Real_Money()
        {
            double real_money = -1;
            try
            {
                if (WebDriver.Url != MainApp.g_setting.dashboard_url)
                {
                    flag = await Navigate(MainApp.g_setting.dashboard_url);
                    if (!flag)
                        return -1;
                }

                await TaskDelay(2000);

                xpath = "//div[@class='cw_tile-currencyContainer']";
                if (!await WaitToPresentByPath(xpath, 10000))
                    return 0;

                string str_balance = WebDriver.FindElementByXPath(xpath).Text;
                MainApp.log_info($"{m_param.account.mail} - Money : {str_balance}");

                str_balance = str_balance.Substring(0, str_balance.IndexOf("EUR"));
                str_balance = str_balance.Replace(" ", "");
                string[] parts = str_balance.Split(',');
                int onoma, minor;
                int.TryParse(parts[0], out onoma);
                int.TryParse(parts[1], out minor);
                real_money = onoma + (double)minor / Math.Pow(10, parts[1].Length);
            }
            catch (Exception e)
            {
                MainApp.log_error($"{m_param.account.mail} - Error catched in Get_Real_Money : {e.Message}");
            }
            return real_money;
        }

        public async Task<bool> Send_Money(PaypalAccount to_Acc, string amount)             // after login
        {
            flag = await Send_Money_inputmail(to_Acc);
            if (!flag)
                return false;

            MainApp.log_info($"{m_param.account.mail} - Input mail page was passed successfully.");
            await TaskDelay(2000);

            flag = await Send_Money_inputamount(amount);
            if (!flag)
                return false;

            MainApp.log_info($"{m_param.account.mail} - Input amount page was passed successfully.");
            await TaskDelay(2000);

            flag = await Send_Money_success();
            if (!flag)
                return false;

            MainApp.log_info($"{m_param.account.mail} - Success alert page was passed successfully.");
            await TaskDelay(2000);

            int release_flag = await Send_Money_Release(to_Acc);

            switch (release_flag)
            {
                case 0:
                    MainApp.log_info($"{m_param.account.mail} - Release failed.");
                    break;
                case 1:
                    MainApp.log_info($"{m_param.account.mail} - Direct Released.");
                    break;
                case 2:
                    MainApp.log_info($"{m_param.account.mail} - Manual Released.");
                    break;
            }

            MainApp.log_info($"{m_param.account.mail} - Release page was passed successfully.");
            await TaskDelay(2000);

            return true;
        }

        public async Task<bool> Send_Money_inputmail(PaypalAccount to_Acc)
        {
            try
            {
                retry = 0;

                while (true)
                {
                    MainApp.log_info($"{m_param.account.mail} - Go to page to send money.");

                    xpath = "//input[@id='fn-sendRecipient']";

                    flag = await Navigate_Find_by_XPath(MainApp.g_setting.url_to_send, xpath, timeout);

                    if (!flag)
                    {
                        MainApp.log_info($"{m_param.account.mail} - Go to page to send money failed.");
                        retry++;
                        if (retry == 2)
                            return false;
                        MainApp.log_info($"{m_param.account.mail} - Go to page to send money retry.");
                        continue;
                    }
                    await TaskDelay(2000);

                    // input the email which send to.             
                    await TryEnterText_by_xpath(xpath, to_Acc.mail, "value", 3000, true);
                    await TaskDelay(2000);

                    xpath = "//button[@type='submit']";                                     // submit button click
                    await TryClickByPath(xpath, 1);
                    await TaskDelay(2000);

                    xpath = "//button[@name='goodsPayment']";
                    if (!await WaitToPresentByPath(xpath, 10000))
                        return false;
                    await TryClick_All(xpath);
                    await TaskDelay(2000);

                    return true;
                }
            }
            catch (Exception ex)
            {
                MainApp.log_info($"#{m_ID} : Error in page to input mail : {ex.Message}");
                return false;
            }
        }

        public async Task<bool> Send_Money_inputamount(string amount)
        {
            try
            {
                string url = "https://www.paypal.com/myaccount/transfer/homepage/buy/preview";
                flag = await WaitUrlSame(url, timeout);
                if (!flag)
                    return false;

                await TaskDelay(2000);

                retry = 0;
                xpath = "//input[@name='amount' and @id='fn-amount']";
                MainApp.log_info($"{m_param.account.mail} - In page to input amount.");
                MainApp.log_info($"{m_param.account.mail} - Repeat {retry}.");

                flag = await WaitToPresentByPath(xpath, 3000);
                if (!flag)
                    return false;

                MainApp.log_info($"{m_param.account.mail} - Page to input amount is correct.");

                await TryEnterText_by_xpath(xpath, amount, "value", 3000, true);
                await TaskDelay(3000);

                string script = $"document.getElementById('noteField').innerHTML='{Str_Utils.GetRandomNote()}.'";  // input note.
                m_js.ExecuteScript(script);
                await TaskDelay(3000);

                xpath = "//button[@data-nemo='continue']";
                await TryClick_All(xpath);
                await TaskDelay(2000);

                xpath = "//button[@data-nemo='send']";
                flag = await WaitToPresentByPath(xpath, 10000);
                if (!flag)
                    return false;
                flag = await TryClick_All(xpath);
                await TaskDelay(2000);

                return true;
            }
            catch (Exception ex)
            {
                MainApp.log_error($"{m_param.account.mail} - Error catched in Send_Money_inputamount : {ex.Message}");
            }
            return false;
        }

        public async Task<bool> Send_Money_success()
        {
            try
            {
                string url = "https://www.paypal.com/myaccount/transfer/homepage/buy/success";
                flag = await WaitUrlSame(url, timeout);
                if (!flag)
                    return false;

                xpath = "//a[@data-nemo='buy-send-more-money']";
                flag = await WaitToPresentByPath(xpath, 10000);
                if (!flag)
                    return false;

                MainApp.log_info($"{m_param.account.mail} - Sent money successfully.");
                await TaskDelay(2000);

                return true;
            }
            catch (Exception ex)
            {
                MainApp.log_error($"{m_param.account.mail} - Error catched in success page : {ex.Message}");
            }
            return false;
        }

        public async Task<int> Send_Money_Release(PaypalAccount to_Acc)
        {
            try
            {
                //PaypalAccount receiver = MainApp.g_db.get_receiver_account(m_param.account.id, 1);
                string full_name = to_Acc.first_name + " " + to_Acc.last_name;
                MainApp.log_info($"{m_param.account.mail} - sent to {full_name}");

                m_sender_transaction_id = m_clsMailChecker.check_email_sender_transCode(to_Acc.mail, full_name);
                MainApp.log_info($"{m_param.account.mail} - transaction code : ");

                await TaskDelay(2000);
                string ID_url = MainApp.g_setting.transaction_url + m_sender_transaction_id;

                if (!await Navigate(ID_url))
                    return 0;                                                                   // failed

                xpath = "//a[@class='confirmReceiptBtn']";

                flag = await WaitToPresentByPath(xpath, 5000);

                if (!flag)
                {
                    if (!await WaitToPresent(By.PartialLinkText("Empfang bestätigen")))
                        return 1;                                                               // direct released. ( status 3 )
                    else
                    {
                        await TryClick(By.PartialLinkText("Empfang bestätigen"), 1);
                        await TaskDelay(2000);
                    }
                }
                else
                {
                    //await TryClickByPath(xpath, 1);
                    await TryClick_All(xpath);
                    await TaskDelay(2000);
                }

                retry = 0;
                while (true)
                {
                    WebDriver.SwitchTo().Window(WebDriver.WindowHandles.Last());
                    string certify_url = WebDriver.Url;

                    xpath = "//input[@name='receipt_yes' and @type='submit']";

                    await TryClick_All(xpath);
                    await TaskDelay(2000);

                    if (await WaitUrlChange(certify_url, 15000))
                        break;
                    else
                    {
                        WebDriver.Close();
                        WebDriver.SwitchTo().Window(WebDriver.WindowHandles.First());

                        retry++;
                        if (retry == 2)
                            return 0;

                        xpath = "//a[@class='confirmReceiptBtn']";

                        if (!await TryClickByPath(xpath, 1))
                            await TryClick(By.PartialLinkText("Empfang bestätigen"), 1);
                        await TaskDelay(2000);
                    }
                }

                return 2;                                                                       // released. ( from status 1 or 2 )

            }
            catch (Exception ex)
            {
                MainApp.log_error($"{m_param.account.mail} - Error in Send_Money_Release : {ex.Message}");
            }
            return 0;
        }

        /*public async Task<void> Get_Status()
        {
            try
            {
                m_real_money_balance = await Get_Real_Money();
                if (m_real_money_balance != -1)
                {
                    MainApp.g_db.set_real_money_balance(m_param.account.id, m_real_money_balance);
                }


            }
            catch(Exception ex)
            {

            }
        }*/

    }
}
