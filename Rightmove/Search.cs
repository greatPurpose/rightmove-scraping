using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Threading;

/*
 *  ClassName: Login
 *  Description: 
 *   It is the class to login with pasword and username.
 *  
 */

namespace Rightmove
{
    public class Search
    {
        public static bool SearchToRent( string findstr, IWebDriver webDriver)
        {
            var rcnt = 2;
            for( int i = 0; i < rcnt; i++)
            {
                try
                {
                    webDriver.Navigate().GoToUrl(Program.WebSite_URL);
                    Thread.Sleep(1000);

                    webDriver.FindElement(By.Id(@"searchLocation")).SendKeys(findstr);
                    Thread.Sleep(100);

                    webDriver.FindElement(By.Id(@"rent")).Click();
                    Thread.Sleep(3000);
                    return true;
                }
                catch
                { }
            }

            return false;
        }

        public static bool FindProperties(IWebDriver webDriver)
        {
            try
            {
                SelectElement selectElement = new SelectElement(webDriver.FindElement(By.Id(@"maxDaysSinceAdded")));
                selectElement.SelectByValue("14");

                webDriver.FindElement(By.Id(@"submit")).Click();
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
