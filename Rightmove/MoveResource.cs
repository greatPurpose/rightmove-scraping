using Newtonsoft.Json;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

/*
 *  ClassName: HumanResource
 *  Description: 
 *   It is the class to scrape web pages for human information.
 *   It parses web pages by using HTML and open the talent pages.
 *   And then it saves about human information.
 *  
 */


namespace Rightmove
{
    public class MoveResource
    {
        public MoveResource()
        {
        }
              
        public void GettingResources(IWebDriver webDrv, StreamWriter streamWriter)
        {            
            try
            {
                Thread.Sleep(2000);
                var totalCnt = 0;
                var currentCnt = 0;
                Dictionary<int, string> urls = new Dictionary<int, string>();
                var strTotalCnt = webDrv.FindElement(By.ClassName(@"searchHeader-resultCount")).Text;
                if (strTotalCnt != "")
                    totalCnt = Int32.Parse(strTotalCnt);

                while (currentCnt <  totalCnt)
                {
                    var elements = webDrv.FindElements(By.XPath(@"//div[@class='l-searchResult is-list']"));
                    foreach(var element in elements)
                    {
                        var id = Int32.Parse(element.GetAttribute(@"id").Replace("property-", string.Empty));

                        if (!urls.ContainsKey(id))
                        {
                            urls.Add(id, Program.WebSite_URL + "properties/" + id.ToString());
                            currentCnt++;
                        }
                    }
                    if (currentCnt < totalCnt)
                    {
                        webDrv.FindElement(By.XPath(@"//button[@class='pagination-button pagination-direction pagination-direction--next']")).Click();
                        Thread.Sleep(2000);
                    }
                }

                InvestResource(webDrv, streamWriter, urls);
            }
            catch(Exception except)
            {
                MessageBox.Show(except.Message);
            }
        }


        private static string InvestResource(IWebDriver webDrv, StreamWriter streamWriter, Dictionary<int, string> paths)
        {
            try
            {
                foreach( var path in paths)
                {
                    webDrv.Navigate().GoToUrl(path.Value);
                    Thread.Sleep(2000);

                    var main = webDrv.FindElement(By.TagName(@"main"));
                    var divElements = main.FindElements(By.TagName(@"div"));

                    RoomInfo roominfo = new RoomInfo();
                    roominfo.Address = divElements[2].Text;
                    roominfo.Price = divElements[9].Text.Replace("\r\n", string.Empty);
                    roominfo.AddOn = divElements[14].Text;
                    
                    var strElements = Regex.Replace(main.Text, "<.*?>", string.Empty);
                    var elements = strElements.Split(new string[] { "\r\n" }, StringSplitOptions.None);

                    for (int i = 0; i < 18; i++)
                    {
                        var strElement = elements[i];

                        if ( strElement.Contains(@"Let available date: "))
                        {
                            roominfo.LetAvailableDate = strElement.Replace(@"Let available date: ", string.Empty);
                            continue;
                        }

                        if (strElement.Contains(@"Let type: "))
                        {
                            roominfo.LetType = strElement.Replace(@"Let type: ", string.Empty);
                            continue;
                        }

                        if (strElement.Contains(@"Furnish type: "))
                        {
                            roominfo.FurnishType = strElement.Replace(@"Furnish type: ", string.Empty);
                            continue;
                        }

                        if (strElement == "PROPERTY TYPE")
                        {
                            roominfo.PropertyType = elements[++i];
                            continue;
                        }
                        
                        if (strElement == "BEDROOMS")
                        {                            
                            roominfo.BedRoom = elements[++i].Substring(1) ;
                            continue;
                        }

                        if (strElement == "BATHROOMS")
                        {
                            roominfo.BathRoom = elements[++i].Substring(1);
                            continue;
                        }

                        if (strElement == "SIZE")
                        {
                            roominfo.Size = elements[++i];
                            continue;
                        }
                    }


                    var aside = webDrv.FindElement(By.TagName(@"aside"));
                    roominfo.MarkedBy= aside.FindElements(By.TagName(@"a"))[0].Text;

                    streamWriter.WriteLine(roominfo.GetRoomInfo());
                    streamWriter.Flush();
                }
                
                return "";
            }
            catch (Exception err)
            {
                return null;
            }
        }

        /*
          * Method Name:  SavingHumanResources
          * Paramters:    buffer - string  
          *              StreamWriter - File stream handler to write the human information
          * Description:  It saves the string result in the csv file.
          */
        private void SavingHumanResources(string buffer, StreamWriter streamWriter)
        {
            streamWriter.WriteLine(buffer);
            streamWriter.Flush();
        }

    }
}
