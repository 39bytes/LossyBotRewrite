using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

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

            IJavaScriptExecutor js = driver as IJavaScriptExecutor;
            js.ExecuteScript($"document.getElementById(\"Text\").innerHTML = \"{text}\";"); //Set the textbox text to whatever
            js.ExecuteScript("CoolText.doPost();");

            await Task.Delay(1500);

            var image = driver.FindElement(By.Id("PreviewImage"));
            string url = image.GetAttribute("src");

            byte[] data = await Globals.httpClient.GetByteArrayAsync(url);

            string filename = url.Split('/').Last(); //get the cooltext filename
            await File.WriteAllBytesAsync(filename, data);

            return filename;
        }
    }
}
