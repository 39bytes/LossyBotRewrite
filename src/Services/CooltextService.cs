using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace LossyBotRewrite
{
    public class CooltextService
    {
        IWebDriver driver;
        public CooltextService()
        {
            ChromeDriverService service = ChromeDriverService.CreateDefaultService(AppDomain.CurrentDomain.BaseDirectory);
            service.SuppressInitialDiagnosticInformation = true;
            service.HideCommandPromptWindow = true;

            ChromeOptions options = new ChromeOptions();
            options.AddArguments("headless", "--log-level=3", "--no-sandbox");

            driver = new ChromeDriver(service, options); //look in executing dir
        }

        public async Task<string> GetBurningTextAsync(string text)
        {
            driver.Url = "https://cooltext.com/Logo-Design-Burning";

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            string valueBefore = wait.Until(NotAttribute(By.Id("PreviewImage"), "src", ""));

            IJavaScriptExecutor js = driver as IJavaScriptExecutor;
            js.ExecuteScript($"document.getElementById(\"Text\").innerHTML = \"{text}\";"); //Set the textbox text to whatever
            js.ExecuteScript("CoolText.doPost();");

            string url;

            try
            {
                url = wait.Until(NotAttribute(By.Id("PreviewImage"), "src", valueBefore)); //wait for the url to change before doing anything
            }
            catch(WebDriverTimeoutException)
            {
                return "";
            }

            byte[] data = await Globals.httpClient.GetByteArrayAsync(url);

            string filename = url.Split('/').Last(); //get the cooltext filename
            await File.WriteAllBytesAsync(filename, data);

            return filename;
        }

        static Func<IWebDriver, string> NotAttribute(By locator, string attribute, string notValue)
        {
            return (driver) => {
                try
                {
                    var value = driver.FindElement(locator).GetAttribute(attribute);
                    return value == notValue ? null : value;
                }
                catch (NoSuchElementException)
                {
                    return null;
                }
                catch (StaleElementReferenceException)
                {
                    return null;
                }
            };
        }
    }
}
