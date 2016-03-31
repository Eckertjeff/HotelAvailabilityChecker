using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System.Threading;
using System.Net.Mail;
using System.Net;
using OpenQA.Selenium.PhantomJS;

namespace PAXHotelFinder
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                var driverService = PhantomJSDriverService.CreateDefaultService();
                driverService.HideCommandPromptWindow = true; // Disables verbose phantomjs output
                IWebDriver driver = new PhantomJSDriver(driverService);
                //IWebDriver driver = new FirefoxDriver();
                driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));
                driver.Navigate().GoToUrl("https://compass.onpeak.com/e/43JLP16");
                Console.WriteLine("Accessing OnPeak.");
                if (driver.Title.ToString() == "PAX East 2016 - Compass Reservation System®")
                {
                    IWebElement category = driver.FindElement(By.CssSelector(".dataSelectButton"));
                    category.Click();
                    IWebElement dropdown = driver.FindElement(By.CssSelector(".dataSelectContent > ul > li:nth-child(2)"));
                    dropdown.Click();
                    IWebElement ViewHotelsButton = driver.FindElement(By.CssSelector(".categorySelectButton"));
                    ViewHotelsButton.Click();

                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    try
                    {
                        driver.FindElement(By.CssSelector("div.datePicker:nth-child(1) > label > input")).Click();
                    }
                    catch (Exception ex)
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(5));
                        try
                        {
                            driver.FindElement(By.CssSelector("div.datePicker:nth-child(1) > label > input")).Click();
                        }
                        catch (Exception exc)
                        {
                            driver.Navigate().Refresh();
                            continue;
                        }
                    }

                    driver.FindElement(By.CssSelector(".ui-datepicker-calendar > tbody:nth-child(2) > tr:nth-child(4) > td:nth-child(5) > a:nth-child(1)")).Click();

                    driver.FindElement(By.CssSelector("div.datePicker:nth-child(2) > label > input")).Click();
                    driver.FindElement(By.CssSelector(".ui-datepicker-calendar > tbody:nth-child(2) > tr:nth-child(5) > td:nth-child(1) > a:nth-child(1)")).Click();
                    Thread.Sleep(TimeSpan.FromSeconds(5));

                    driver.FindElement(By.CssSelector("#viewport > div.hotelsView.\\2e viewContainer > div > div.filterMapGridWrapper > div.mapOrGridWrapper > div.HotelListWidget > div.hotelListDiv > div.hotelCardHolder > div > div.hotel_id_19967")).Click();

                    if (driver.Url.ToString().Contains("#hotelInfo/19967")) //4552 dt 19967 ele
                    {
                        Console.WriteLine("THERE'S A THING!");
                        SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587)
                        {
                            Credentials = new NetworkCredential("USERNAME", "PASSWORD"),
                            EnableSsl = true
                        };
                        smtp.Send("SENDER", "RECIPIENT", "PAX Hotel Available", "There's a PAX Hotel available! \n https://compass.onpeak.com/e/43JLP16");
                        break;
                    }
                    else
                    {
                        Console.WriteLine(DateTime.Now.ToShortTimeString() + " Hotel Not Available, Trying again...");
                        driver.Quit();
                    }
                }
            }
            Console.ReadLine();
        }
    }
}
