
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

/*
 *  ClassName: Rightmove
 *  Description: This is the class that has the main function. 
 *  
 */


namespace Rightmove
{

    public class Program
    {
        static void Main(string[] args)
        {
            Process[] processes = Process.GetProcessesByName("Scraping-Rightmove");
            if (processes.Count() > 1)
            {
                MessageBox.Show("Scraping-Rightmove.exe is already running now. So it will be closed.");
                return;
            }
            KillProcessAndChildren(_browserName);

            var filepath = Path.GetDirectoryName(Application.ExecutablePath) + _defaultFileName;
            if (File.Exists(filepath))
                File.Delete(filepath);

            _streamWriter = new StreamWriter(filepath, true, Encoding.Unicode);
            RoomInfo roominfo = new RoomInfo();
            roominfo.Address = "ADDRESS";
            roominfo.Price = "PRICE";
            roominfo.AddOn = "ADD ON";
            roominfo.LetAvailableDate = "LET AVAILABLE DATE";
            roominfo.LetType = "LET TYPE";
            roominfo.FurnishType = "FURNISH TYPE";
            roominfo.PropertyType = "PROPERTY TYPE";
            roominfo.BedRoom = "BED ROOM";
            roominfo.BathRoom = "BATH ROOM";
            roominfo.Size = "SIZE";
            roominfo.MarkedBy = "MARKED BY";
            _streamWriter.WriteLine(roominfo.GetRoomInfo());
            _streamWriter.Flush();

            ProcScraping();
        }

        private static void ProcScraping()
        {

            var options = new FirefoxOptions
            {
                PageLoadStrategy = PageLoadStrategy.Eager,
                Profile = new FirefoxProfile(),
            };

            options.Profile.AcceptUntrustedCertificates = true;
            options.Profile.AssumeUntrustedCertificateIssuer = false;
            options.Profile.SetPreference("general.useragent.override", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3205.0 Safari/537.36");
            //options.AddArgument("--headless");


            FirefoxDriverService service = FirefoxDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;

            FirefoxDriver ieDriver = null;
            try
            {
                ieDriver = new FirefoxDriver(service, options);
                {
                    ieDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
                    ieDriver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(40);
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("It requires Firefox browser.\nPlease install and then restar.\nError=" + err.Message);
                return;
            }

            try
            {
                if (!Search.SearchToRent("W2", ieDriver))
                    throw new Exception("Search option is incorrect. Please check them.");

                if (!Search.FindProperties(ieDriver))
                    throw new Exception("Search option is incorrect. Please check them.");
                               
                var Res = new MoveResource();
                {
                    Res.GettingResources(ieDriver, _streamWriter);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("Maybe the network has problem. It will restart after 1hr.\nError = " + exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            ieDriver.Quit();
        }

        private static void KillProcessAndChildren(string pname)
        {
            SelectQuery query = new SelectQuery("Select * From Win32_Process Where Name=\"" + pname + "\"");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection moc = searcher.Get();

            if (moc.Count <= 0)
                return;

            foreach (ManagementObject mo in moc)
            {
                int ncount = 3;
                while (ncount > 0)
                {
                    try
                    {
                        Process.GetProcessById(Convert.ToInt32(mo["ProcessID"])).Kill();
                        break;
                    }
                    catch
                    {
                        Thread.Sleep(300);
                    }

                    ncount--;
                }
            }

        }
        private static string _defaultFileName = "\\Scraping_Result.csv";
        private static string _browserName = "firefox.exe";
        private static StreamWriter _streamWriter = null;

        public static string WebSite_URL = "https://www.rightmove.co.uk/";

    }
}
