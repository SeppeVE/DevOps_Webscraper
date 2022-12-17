using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Threading;
using OpenQA.Selenium.Safari;
using System.Collections.Generic;
using System.Web;
using OpenQA.Selenium.DevTools.V106.Tethering;
using System.Xml.Linq;

using Newtonsoft.Json;
using System.Linq;
using CaseStudy;

namespace webscraping
{
    class Program
    {
        static void Main(string[] args)
        {
            int userInput = 0;
            do
            {
                userInput = Keuze();
                if (userInput == 1)
                {
                    youtube();
                }
                else if (userInput == 2)
                {
                    jobsite();
                }
                else if (userInput == 3)
                {
                    tweedehands();
                }

            } while (userInput != 4);


        }
        static public int Keuze()
        {
            Console.Clear();
            while (true)
            {
                Console.WriteLine("");
                Console.WriteLine();
                Console.WriteLine("Maak uw Keuze");
                Console.WriteLine("-----------------------------------");
                Console.WriteLine("1. Youtube Video's");
                Console.WriteLine("2. Jobsite (VDAB)");
                Console.WriteLine("3. 2dehands (auto's)");
                Console.WriteLine("4. Afsluiten");
                Console.WriteLine("-----------------------------------");
                Console.WriteLine("\n");
                int result;
                if (Int32.TryParse(Console.ReadLine(), out result))
                    return result;
                else
                    Console.WriteLine("Geef een cijfer in");
            }
        }

        static void youtube()
        {
            Console.WriteLine("\n");
            Console.WriteLine("Geef zoekterm in:");
            string zoekterm = Console.ReadLine();
            var zoektermNew = zoekterm;
            var zoektermNieuw = zoektermNew.Replace(" ", "+");

            IWebDriver driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl("https://www.youtube.com/results?search_query=" + zoektermNieuw + "&sp=CAI%253D");
            Thread.Sleep(300);

            var accepteer = driver.FindElement(By.XPath("//*[@id=\"content\"]/div[2]/div[6]/div[1]/ytd-button-renderer[2]/yt-button-shape/button/yt-touch-feedback-shape/div/div[2]"));
            accepteer.Click();
            Thread.Sleep(200);

            //var video = driver.FindElements(By.XPath("//*[@id=\"contents\"]/ytd-video-renderer"));
            var titleMain = driver.FindElements(By.XPath("//*[@id=\"video-title\"]/yt-formatted-string"));
            var viewsMain = driver.FindElements(By.XPath("//*[@id=\"metadata-line\"]/span[1]"));
            var uploaderMain = driver.FindElements(By.XPath("//*[@id=\"channel-info\"]/a"));
            var linkMain = driver.FindElements(By.XPath("//*[@id=\"video-title\"]"));
            Thread.Sleep(300);

            string[] videos = new string[255];

            for (int i = 0; i < 5; i++)
            {
                try
                {
                    var title = titleMain.ElementAt(i);
                    var views = viewsMain.ElementAt(i);
                    var uploader = uploaderMain.ElementAt(i);
                    var uploadee = uploader.GetAttribute("href");
                    var uploadeeNew = uploadee.Substring(uploadee.LastIndexOf('@'));
                    var link = linkMain.ElementAt(i);
                    var url = link.GetAttribute("href");
                    //Console.WriteLine(title.Text);
                    //Console.WriteLine(views.Text);
                    //Console.WriteLine(uploadeeNew);
                    //Console.WriteLine(url);

                    Upload upload = new Upload()
                    {
                        Title = title.Text,
                        Views = views.Text,
                        Account = uploadeeNew,
                        Url = url
                    };



                    string strResultJson = JsonConvert.SerializeObject(upload);

                    videos = videos.Append(strResultJson).ToArray();

                    Console.WriteLine("\n");
                }
                catch { }


            }
            string videoss = string.Join("", videos);
            File.WriteAllText(@"YouTube.json", videoss);
        }

        static void jobsite()
        {
            Console.WriteLine("\n");
            Console.WriteLine("Geef zoekterm in:");
            string zoekterm = Console.ReadLine();
            var zoektermNew = zoekterm;
            var zoektermNieuw = zoektermNew.Replace(" ", "%20");

            IWebDriver driver = new ChromeDriver();
            driver.Manage().Window.Maximize();

            driver.Navigate().GoToUrl("https://www.vdab.be/vindeenjob/vacatures?trefwoord=" + zoektermNieuw + "&sort=datum");
            Thread.Sleep(200);

            var acceptButton = driver.FindElement(By.XPath("//*[@id=\"cookies\"]/div/div/div[1]/div/div/div[2]/a[1]"));
            acceptButton.Click();
            Thread.Sleep(5000); //VDAB is niet zo snel

            string[] jobs = new string[255];

            for (int i = 0; i < 5; i++)
            {
                string j = i.ToString();

                var pos = "//*[@id=\"vej-list-item- \"]";
                var posNew = pos.Replace(" ", j);

                var vacature = driver.FindElement(By.XPath(posNew));

                var job = vacature.FindElement(By.ClassName("slat-title"));
                Console.WriteLine(job.Text);

                var location = vacature.FindElement(By.ClassName("location"));
                Console.WriteLine(location.Text);

                var extra = vacature.FindElement(By.ClassName("job-type"));
                Console.WriteLine(extra.Text);

                var link = vacature.FindElement(By.ClassName("slat-link"));
                var url = link.GetAttribute("href");
                Console.WriteLine(url);

                Console.WriteLine("\n");

                JobAd jobad = new JobAd()
                {
                    Job = job.Text,
                    Location = location.Text,
                    Extra = extra.Text,
                    Url = url
                };



                string strResultJson = JsonConvert.SerializeObject(jobad);

                jobs = jobs.Append(strResultJson).ToArray();
            }


            string jobss = string.Join("", jobs);
            File.WriteAllText(@"Jobs.json", jobss);
        }

        static void tweedehands()
        {
            
            // driver.Navigate().GoToUrl(" https://www.youtube.com/c/LambdaTest/videos");
            // var accepteer = driver.FindElement(By.XPath("//*[@id='yDmH0d']/c-wiz/div/div/div/div[2]/div[1]/div[3]/div[1]/form[2]/div/div/button/span"));
            // accepteer.Click();

            Console.WriteLine("\n");
            Console.WriteLine("1. Marktplaats");
            Console.WriteLine("2. 2dehands");
            Console.WriteLine("------------------");
            string site = Console.ReadLine();
            
            Console.WriteLine("\n");
            Console.WriteLine("Merk?");
            Console.WriteLine("-----------");
            string merk = Console.ReadLine();

            Console.WriteLine("\n");
            Console.WriteLine("Model?");
            Console.WriteLine("-----------");
            string model = Console.ReadLine();

            IWebDriver driver = new ChromeDriver();

            if (site == "1")
            {
                var originalLink1 = "https://www.marktplaats.nl/l/auto-s/brand/#q:make|Language:all-languages|PriceCentsFrom:200|sortBy:PRICE|sortOrder:INCREASING|searchInTitleAndDescription:true";
                var linkBrand = originalLink1.Replace("brand", merk);
                var linkBrandModel = linkBrand.Replace("make", model);

                driver.Navigate().GoToUrl(linkBrandModel); //https://www.marktplaats.nl/
                Thread.Sleep(200);
            }
            else
            {
                var originalLink1 = "https://www.2dehands.be/l/auto-s/brand/#q:make|Language:all-languages|PriceCentsFrom:200|sortBy:PRICE|sortOrder:INCREASING|searchInTitleAndDescription:true";
                var linkBrand = originalLink1.Replace("brand", merk);
                var linkBrandModel = linkBrand.Replace("make", model);

                driver.Navigate().GoToUrl(linkBrandModel);
                Thread.Sleep(200);
            }

            

            var accepteer = driver.FindElement(By.XPath("//*[@id='gdpr-consent-banner-accept-button']"));
            accepteer.Click();
            Thread.Sleep(200);



            string[] adverts = new string[255];

            var lists = driver.FindElements(By.ClassName("hz-Listing--list-item"));

            foreach (var listing in lists)
            {
                var titel = listing.FindElement(By.ClassName("hz-Listing-title"));
                Console.WriteLine(titel.Text);

                var beschrijving = listing.FindElement(By.ClassName("hz-Listing-description"));
                Console.WriteLine(beschrijving.Text);

                var prijs = listing.FindElement(By.ClassName("hz-text-price-label"));
                Console.WriteLine(prijs.Text);

                var link = listing.FindElement(By.ClassName("hz-Listing-coverLink"));
                var url = link.GetAttribute("href");
                Console.WriteLine(url);
                Console.WriteLine("\n");

                Listing2 listings = new Listing2()
                {
                    Title = titel.Text,
                    Beschrijving = beschrijving.Text,
                    Prijs = prijs.Text,
                    Url = url
                };


                string strResultJson = JsonConvert.SerializeObject(listings);

                adverts = adverts.Append(strResultJson).ToArray();

                Console.WriteLine("\n");
            }
            string ads = string.Join("", adverts);

            if(site == "1")
            {
                File.WriteAllText(@"Marktplaats.json", ads);
            }
            else
            {
                File.WriteAllText(@"2dehands.json", ads);
            }

            
        }

    }
}
